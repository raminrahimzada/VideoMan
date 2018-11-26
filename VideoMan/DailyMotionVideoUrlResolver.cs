using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VideoMan
{
    public class DailyMotionVideoUrlResolver:ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string url)
        {
             var html = App.Client.DownloadStringFromUrl(url);
            var jsonMetaData = html.GetStringBetween("__PLAYER_CONFIG__ = ", ";</script>");
            var jObj = JObject.Parse(jsonMetaData);
            var metadataTemplateUrl = jObj.SelectToken("context").SelectToken("metadata_template_url").ToString();
            var embedder = jObj.SelectToken("context").SelectToken("embedder").ToString();
            var videoId = embedder.RemoveSubString("https://").RemoveSubString("http://").RemoveSubString("www.")
                .RemoveSubString("dailymotion.com/video/");
            metadataTemplateUrl = metadataTemplateUrl.Replace(":videoId", videoId);
            var jsonMetaData2 = App.Client.DownloadStringFromUrl(metadataTemplateUrl);
            jObj = JObject.Parse(jsonMetaData2);
            var filmstripUrl = jObj.SelectToken("filmstrip_url").ToString();
            //no need to this
            // ReSharper disable once UnusedVariable
            var posterUrl = jObj.SelectToken("poster_url").ToString();//small thumbnail
            
            //in seconds
            var duration = jObj.SelectToken("duration").ToString().TryToInt(); 
            var id = jObj.SelectToken("id").ToString(); //check if the same
            var title = jObj.SelectToken("title").ToString() ;

            var posters = jObj.SelectToken("posters");
            var postersDictionary = posters.Cast<JProperty>()
                .ToDictionary(poster => poster.Name.TryToInt(), poster => poster.Value.ToString());

            var qualities = jObj.SelectToken("qualities");
            foreach (var jToken in qualities)
            {
                var poster = (JProperty)jToken;
                var resolution = poster.Name.TryToInt();
                var type = poster.Values().First().First().Values().First().ToString();
                var urlx = poster.Values().First().Skip(1).First().Values().First().ToString();

                postersDictionary.TryGetValue(resolution, out var thumbnailUrl);

                yield return new VideoDownloadInfo
                {
                    VideoId = id,
                    Title = title,
                    UrlString = urlx,
                    Duration = duration,
                    MimeType = type.ParseMimeType(),
                    ResolutionType = (VideoResolutionTypes) resolution,
                    ThumbnailPictureUrlString = thumbnailUrl,
                    ThumbnailSlideUrl = filmstripUrl
                };
            }
        }

       
    }
}