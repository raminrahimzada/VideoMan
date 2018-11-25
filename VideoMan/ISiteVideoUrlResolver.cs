using System.Collections.Generic;

namespace VideoMan
{
    public interface ISiteVideoUrlResolver
    {
        IEnumerable<VideoDownloadInfo> Resolve(string url);
    }
}