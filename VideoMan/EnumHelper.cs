using System;

namespace VideoMan
{
    public static class EnumHelper
    {
        public static VideoMimeTypes ParseMimeType(this string type)
        {
            if (string.IsNullOrEmpty(type)) return VideoMimeTypes.None;
            if (type == "application/x-mpegURL") return VideoMimeTypes.MPEG;

            if (type.Contains("video/mp4")) return VideoMimeTypes.Mp4;
            if (type.Contains("video/webm")) return VideoMimeTypes.WebM;
            if (type.Contains("video/3gpp")) return VideoMimeTypes._3GPP;

            throw new Exception("not implemented yet");
        }
        public static VideoResolutionTypes ParseResolutionType(this string quality)
        {
            if (string.IsNullOrEmpty(quality)) return VideoResolutionTypes.P0;

            switch (quality)
            {
                case "1440p": return VideoResolutionTypes.P1440;
                case "2160p": return VideoResolutionTypes.P2160;
                case "1080p": return VideoResolutionTypes.P1080;
                case "720p": return VideoResolutionTypes.P720;
                case "540p": return VideoResolutionTypes.P540;
                case "480p": return VideoResolutionTypes.P480;
                case "360p": return VideoResolutionTypes.P360;
                case "240p": return VideoResolutionTypes.P240;
                case "144p": return VideoResolutionTypes.P144;

                //quality
                case "hd720": return VideoResolutionTypes.P720;
                case "medium": return VideoResolutionTypes.P480;
                case "small": return VideoResolutionTypes.P240;
            }

            throw new Exception("not implemented yet");
        }
    }
}