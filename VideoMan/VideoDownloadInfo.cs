namespace VideoMan
{
    public class VideoDownloadInfo
    {
        public string VideoId { get; set; }
        public long Duration { get; set; }
        public string UrlString { get; set; }
        public string ThumbnailPictureUrlString { get; set; }
        public string ThumbnailSlideUrl{ get; set; }
        public VideoResolutionTypes ResolutionType { get; set; }
        public VideoMimeTypes MimeType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
    }
}