using System.Collections.Generic;

namespace VideoMan
{
    public class XnXxVideoUrlResolver : ISiteVideoUrlResolver
    {
        public IEnumerable<VideoDownloadInfo> Resolve(string url)
        {
            var html = DI.Instance.GetInstance<IWebClient>().DownloadStringFromUrl(url);
            
            // theese not used
            // ReSharper disable   UnusedVariable
            var urlHLSVideo = html.GetStringBetween("html5player.setVideoHLS(\'", "');");
            var thumbSlideMinute = html.GetStringBetween("html5player.setThumbSlideMinute(\'", "\');");
            var thumbImage = html.GetStringBetween("<meta property=\"og:image\" content=\"", "\" />");
            // ReSharper restore UnusedVariable

            var urlLowVideo = html.GetStringBetween("html5player.setVideoUrlLow(\'", "');");
            var urlHighVideo = html.GetStringBetween("html5player.setVideoUrlHigh(\'", "\');");
            

            var thumbUrl = html.GetStringBetween("html5player.setThumbUrl(\'", "\');");
            var thumbUrl169 = html.GetStringBetween("html5player.setThumbUrl169('", "\');");
            var thumbSlide = html.GetStringBetween("html5player.setThumbSlide('", "\');");
            var thumbSlideBig = html.GetStringBetween("html5player.setThumbSlideBig(\'", "\');");
           

            var videoId = html.GetStringBetween("<meta property=\"og:video\" content=\"", "\" />").FromThisToEnd("id_video=");
            var videoTitle = html.GetStringBetween("html5player.setVideoTitle(\'", "\');");
          
            var duration = html.GetStringBetween("<meta property=\"og:duration\" content=\"", "\" />").TryToLong();
            var width = html.GetStringBetween("<meta property=\"og:video:width\" content=\"", "\" />").TryToInt();
            var height = html.GetStringBetween("<meta property=\"og:video:height\" content=\"", "\" />").TryToInt();

            yield return new VideoDownloadInfo
            {
                Duration = duration,
                VideoId = videoId,
                Width=width,
                Height = height,
                Title= videoTitle,
                ThumbnailPictureUrlString = thumbUrl,
                ThumbnailSlideUrl = thumbSlide,
                UrlString = urlLowVideo,
            };

            yield return new VideoDownloadInfo
            {
                Duration = duration,
                VideoId = videoId,
                Width = width,
                Height = height,
                Title = videoTitle,
                ThumbnailPictureUrlString = thumbUrl169,
                ThumbnailSlideUrl = thumbSlideBig,
                UrlString = urlHighVideo,
            };
        }
    }
}