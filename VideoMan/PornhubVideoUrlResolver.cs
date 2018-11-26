using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VideoMan
{
    public class PornhubVideoUrlResolver : ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string url)
        {
            var query = HttpHelper.ParseQueryString(url);
            var videoId = query["viewkey"];
            var rawUrl = $"https://www.pornhub.com/view_video.php?viewkey={videoId}";
            var html = App.Client.DownloadStringFromUrl(rawUrl);
            var duration = html.GetStringBetween("<meta property=\"video:duration\" content=\"", "\"").TryToInt();
            var width = html.GetStringBetween("<meta name=\"twitter:player:width\" content=\"", "\"").TryToInt();
            var height = html.GetStringBetween("<meta name=\"twitter:player:height\" content=\"", "\"").TryToInt();


            var embedUrl = $"https://www.pornhub.com/embed/{videoId}";
            html = App.Client.DownloadStringFromUrl(embedUrl);
            var title = html.GetStringBetween("<title>", "</title>");
            var flashVarsJson = html.GetStringBetween("var flashvars =", "utmSource");
            flashVarsJson = flashVarsJson.Trim(" \t\r\n,".ToCharArray());
            var jObj = JObject.Parse(flashVarsJson);
            var image_url = jObj.SelectToken("image_url").ToString();
            var video_title = jObj.SelectToken("video_title").ToString();//check if the same
            var defaultQuality = jObj.SelectToken("defaultQuality").Values<int>().ToArray();//check if the same
            var mediaDefinitions = jObj.SelectToken("mediaDefinitions");//check if the same

            
            foreach (var mediaDefinition in mediaDefinitions)
            {
                var videoUrl = mediaDefinition.SelectToken("videoUrl").ToString();
                var quality = mediaDefinition.SelectToken("quality").ToString().TryToInt();
                yield return new VideoDownloadInfo 
                {
                    Width = width,
                    Height = height,
                    ResolutionType = (VideoResolutionTypes)quality,
                    UrlString = videoUrl,
                    VideoId = videoId,
                    Title = title,
                    ThumbnailPictureUrlString = image_url,
                    Duration = duration
                };
            }
        }
    }
}