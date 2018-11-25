using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace VideoMan
{
    internal static class Program
    {
      static IEnumerable<Uri> LoadPlaylist(Uri source)
        {
            using (var client = new WebClient())
            {
                var processedPlaylists = new HashSet<Uri>();
                var playlists = new Queue<Uri>();
                playlists.Enqueue(source);

                while (playlists.Count != 0)
                {
                    var playlistUri = playlists.Dequeue();
                    if (!processedPlaylists.Add(playlistUri)) continue;

                    var playlistContent = client.DownloadString(playlistUri);
                    var playlistLines = playlistContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in playlistLines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (line[0] == '#') continue;

                        if (!Uri.TryCreate(source, line, out var file))
                        {
                            Console.WriteLine("Invalid line: '{0}'", line);
                            continue;
                        }

                        var extension = Path.GetExtension(file.LocalPath);
                        if (extension.StartsWith(".m3u", StringComparison.OrdinalIgnoreCase))
                        {
                            playlists.Enqueue(file);
                        }
                        else
                        {
                            yield return file;
                        }
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            DI.Instance.Register<IWebClient, SimpleWebClient>();

            //var all = new XnXxVideoUrlResolver().Resolve("https://www.xnxx.com/video-p0khp52/_").ToList();

            //var all = new VidmolyVideoUrlResolver().Resolve("https://vidmoly.me/3ke0y6u5rtgo.html").ToList();

            //var all = new YoutubeVideoUrlResolver().Resolve("https://www.youtube.com/watch?v=Fuv59BhKf_Q").ToList();

            //var all = new DailyMotionVideoUrlResolver().Resolve("https://www.dailymotion.com/video/x5sqawm").ToList();
         
        }
    }
}
