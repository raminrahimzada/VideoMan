using System.Collections.Generic;

namespace VideoMan
{
    /// <summary>
    /// downloads video from links like https://vidmoly.me/3ke0y6u5rtgo.html
    /// New Way Of Downloading video from this site:
    /// just open embed url like this:
    /// view-source:https://vidmoly.me/embed-3ke0y6u5rtgo.html
    /// just before the vplayer div there will be script tag
    /// copy all code inside script tag
    /// goto https://beautifier.io/ and replace copied code here
    /// you will see detailed video info here :)
    /// </summary>
    public class VidmolyVideoUrlResolver : ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string url)
        {
            var client = DI.Instance.GetInstance<IWebClient>();
            var html = client.DownloadStringFromUrl(url);
            //embed video url in iframeSrc
            var iframeSrc = html.GetStringBetween("<iframe src=\"", "\"");
            if (iframeSrc.StartsWith("//")) iframeSrc = "http:" + iframeSrc;
            //get video id
            var videoId = iframeSrc.RemoveSubString("https://vidmoly.me/")
                .RemoveSubString("https://")
                .RemoveSubString("http://")
                .RemoveSubString("vidmoly.me/embed-")
                .RemoveSubString(".html");

            html = client.DownloadStringFromUrl(iframeSrc);
            var label = html.GetStringBetween("|label|", "|");

            //TODO get Hash & get direct download url instead
            //var hash = "";
            //var newUrl =$"https://vidmoly.me/dl?op=download_orig&id={videoId}&mode=&hash={hash}";
            //html = client.DownloadStringFromUrl(newUrl);
            var videoUrl = $"//144.mokalix.tk/{label}/v.mp4";

            return new[]
            {
                new VideoDownloadInfo()
                {
                    VideoId = videoId,
                    UrlString = videoUrl
                }
            };
        }
    }
}