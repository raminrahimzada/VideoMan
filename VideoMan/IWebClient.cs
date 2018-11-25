namespace VideoMan
{
    public interface IWebClient
    {
        byte[] DownloadDataFromUrl(string url);
        string DownloadStringFromUrl(string url);
    }
}