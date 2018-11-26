using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VideoMan
{
    //this will be used in downloading m3u8 links
    public class M3U8Stream 
    {
        private readonly string _url;

        private readonly Dictionary<string, byte[]> _internalBuffer;

        private bool _stop;

        public M3U8Stream(string url)
        {
            _internalBuffer = new Dictionary<string, byte[]>();

            _stop = false;
            _url = url;

            
        }

        public async Task StartDownloading(string url)
        {
            using (var client = new HttpClient())
            {
                while (!_stop)
                {
                    var data = await client.GetStringAsync(url);

                    var lines = data.Split('\n');
                    if (lines.Any())
                    {
                        const int defaultTargetDuration = 100;
                        var targetDuration = defaultTargetDuration;

                        var firstLine = lines[0];
                        if (firstLine != "#EXTM3U")
                        {
                            throw new InvalidOperationException(
                                "The provided URL does not link to a well-formed M3U8 playlist.");
                        }

                        for (var i = 1; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (line.StartsWith("#"))
                            {
                                var lineData = line.Substring(1);

                                var split = lineData.Split(':');

                                var name = split[0];
                                var value = split[1];

                                switch (name)
                                {
                                    case "EXT-X-TARGETDURATION":
                                        if (targetDuration == defaultTargetDuration)
                                        {
                                            targetDuration = int.Parse(value);
                                        }
                                        break;

                                    //oh, how sweet. a header for us to entirely ignore. we'll always use cache.
                                    case "EXT-X-ALLOW-CACHE":
                                        break;

                                    case "EXT-X-VERSION":
                                        break;

                                    case "EXT-X-MEDIA-SEQUENCE":
                                        break;

                                    case "EXTINF":
                                        var nextLine = lines[i + 1];
                                        if (!_internalBuffer.ContainsKey(nextLine) && !_stop)
                                        {
                                            var bytes = await client.GetByteArrayAsync(nextLine);
                                            _internalBuffer.Add(nextLine, bytes);
                                        }
                                        break;
                                }
                            }
                        }

                        //wait for a new part of the stream to appear if we're lucky.
                        await Task.Delay(targetDuration * 1000 / 2);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "The provided URL does not contain any data.");
                    }
                }
            }
        }

        public void Dispose()
        {
            _stop = true;
        }

        public async Task<byte[]> ReadAsync(byte[] buffer, uint count)
        {
            var bytesRead = 0u;
            CancellationToken token = CancellationToken.None;
            //keep going until we've read all data.
            while (bytesRead < count && !token.IsCancellationRequested)
            {

                var firstKey = string.Empty;
                while (string.IsNullOrEmpty(firstKey) && !token.IsCancellationRequested)
                {
                    //while we don't have data in the trunk, wait for it.
                    firstKey = _internalBuffer.Keys.FirstOrDefault();
                    await Task.Delay(100, token);
                }

                //did we cancel? exit out.
                if (token.IsCancellationRequested)
                {
                    return buffer;
                }

                //copy the data over.
                var bufferData = _internalBuffer[firstKey];

                var amount = Math.Min(bufferData.Length, count - bytesRead);
                //bufferData.CopyTo(0, buffer, bytesRead, (int) amount);
                //TODO
                bufferData.CopyTo(buffer, 0);
                
                //increment bytes read.
                bytesRead += (uint) amount;


            }

            return buffer;
        }



        public void Seek(ulong position)
        {
            Position = position;
        }
         
     
        public ulong Position { get; private set; }
      
    }
}