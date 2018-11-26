
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace VideoMan
{
    public   class YoutubeVideoUrlResolver : ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                return new List<VideoDownloadInfo>();
            }

            var isYoutubeUrl = TryNormalizeYoutubeUrl(videoUrl, out videoUrl,out var videoId);
            if (!isYoutubeUrl)
            {
                return new List<VideoDownloadInfo>();
            }
            var json = LoadJson(videoUrl);
            if (json == null)
            {
                return new List<VideoDownloadInfo>();
            }

            //get title
            var title = json["args"]["title"];
            var videoTitle = title?.ToString() ?? string.Empty;
            var downloadUrls = ExtractDownloadUrls(json);

            downloadUrls = downloadUrls.Select(v =>
            {
                v.VideoId = videoId;
                v.Title = videoTitle;
                return v;
            });

            return downloadUrls;
        }
        


        private static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl,out string videoId)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");
            var query = HttpHelper.ParseQueryString(url);
            if (!query.TryGetValue("v", out videoId))
            {
                normalizedUrl = null;
                return false;
            }
            normalizedUrl = "http://youtube.com/watch?v=" + videoId;

            return true;
        }

        private static IEnumerable<VideoDownloadInfo> ExtractDownloadUrls(JObject json)
        {
            // bugfix: adaptive_fmts is missing in some videos, use url_encoded_fmt_stream_map instead
            var streamMap = json["args"]["adaptive_fmts"] ?? json["args"]["url_encoded_fmt_stream_map"];

            var streamMapString = streamMap?.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new Exception("Video is removed or has an age restriction.");
            }
            var splitByUrls = streamMapString.Split(',');
            var adaptiveFmtSplitByUrls = streamMap.ToString().Split(',');
            splitByUrls = splitByUrls.Concat(adaptiveFmtSplitByUrls).ToArray();

            const string signatureQuery = "signature";
            const string rateBypassFlag = "ratebypass";

            foreach (var s in splitByUrls)
            {
                var queries = HttpHelper.ParseQueryString(s);
                string url;

                if (queries.ContainsKey("s") || queries.ContainsKey("sig"))
                {
                    var signature = queries.ContainsKey("s") ? queries["s"] : queries["sig"];
                    url = $"{queries["url"]}&{signatureQuery}={signature}";
                    var fallbackHost = queries.ContainsKey("fallback_host") ? "&fallback_host=" + queries["fallback_host"] : string.Empty;
                    url += fallbackHost;
                }
                else
                {
                    url = queries["url"];
                }

                url = HttpHelper.UrlDecode(url);
                var parameters = HttpHelper.ParseQueryString(url);

                if (!parameters.ContainsKey(rateBypassFlag))
                    url += $"&{rateBypassFlag}=yes";

                queries.TryGetValue("quality_label", out var quality);
                if (string.IsNullOrEmpty(quality))
                {
                    queries.TryGetValue("quality", out quality);
                }
                queries.TryGetValue("type", out var type);
                if (!string.IsNullOrEmpty(type) && type.Contains("audio/")) continue;

                queries.TryGetValue("size", out var size);
                int w = 0, h = 0;
                if (size != null)
                {
                    var wh = size.Split('x').Select(int.Parse).ToArray();
                    w = wh[0];
                    h = wh[1];
                }
                yield return new VideoDownloadInfo
                {
                    UrlString = url,
                    ResolutionType = quality.ParseResolutionType(),
                    MimeType = type.ParseMimeType(),
                    Width = w,
                    Height = h
                };
            }
        }

        

        

        private static JObject LoadJson(string url)
        {
            var pageSource = App.Client.DownloadStringFromUrl(url);
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            var isUnavailable= pageSource.Contains(unavailableContainer);
            if (isUnavailable)
            {
                throw new Exception("VideoNotAvailable");
            }

            var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

            var m = dataRegex.Match(pageSource);
            if (m.Success)
            {
                var extractedJson = dataRegex.Match(pageSource).Result("$1");
                return JObject.Parse(extractedJson);
            }

            return null;
        }
    }
}
