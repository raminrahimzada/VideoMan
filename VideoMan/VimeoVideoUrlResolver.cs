using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VideoMan
{
    public class VimeoVideoUrlResolver : ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string url)
        {
            var videoId = url.RemoveSubString("https://").RemoveSubString("http://").RemoveSubString("www.")
                .RemoveSubString("vimeo.com/");
            var playerUrl = $"https://player.vimeo.com/video/{videoId}";
            var html = App.Client.DownloadStringFromUrl(playerUrl);
            var configJson = html.GetStringBetween("var config =", "if (!config.request) {")
                .Trim(" \r\n\t;".ToCharArray());
            var config = JObject.Parse(configJson);

            var video = config.SelectToken("video");
            //var heightX = video.SelectToken("height").ToString().TryToInt();
            //var widthX = video.SelectToken("width").ToString().TryToInt();
            var duration = video.SelectToken("duration").ToString().TryToInt();
            var thumbs = video.SelectToken("thumbs");
            var idX = video.SelectToken("id").ToString();
            var title = video.SelectToken("title").ToString();
            var progressive = config.SelectToken("request.files.progressive").ToList();
            foreach (var p in progressive)
            {
                //var profile = p.SelectToken("profile").ToString();
                var width = p.SelectToken("width").ToString().TryToInt();
                var height = p.SelectToken("height").ToString().TryToInt();
                var mime = p.SelectToken("mime").ToString().ParseMimeType();
                var videoUrl = p.SelectToken("url").ToString();
                var id = p.SelectToken("id").ToString();
                var quality = p.SelectToken("quality").ToString().ParseResolutionType();
                yield return new VideoDownloadInfo()
                {
                    VideoId = id,
                    Title = title,
                    ResolutionType = quality,
                    UrlString = videoUrl,
                    Duration = duration,
                    Height = height,
                    Width = width,
                    MimeType = mime,
                };
                var thumb = thumbs.SelectToken(((int)quality).ToString())+string.Empty;
                var t = p;
            }
            yield break;
        }
    }
}