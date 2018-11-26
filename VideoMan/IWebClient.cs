using System;

namespace VideoMan
{
    public interface IWebClient:IDisposable
    {
        byte[] DownloadDataFromUrl(string url);
        string DownloadStringFromUrl(string url);
    }
}