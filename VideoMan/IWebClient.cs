namespace VideoMan
{
    public interface IWebClient
    {
        byte[] DownloadDataFromUrl(string url);
        string DownloadHtmlFromUrl(string url);
    }
}