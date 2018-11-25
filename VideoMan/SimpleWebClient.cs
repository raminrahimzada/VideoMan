using System.Net;
using System.Text;

namespace VideoMan
{
    public class SimpleWebClient : IWebClient
    {
        public byte[] DownloadDataFromUrl(string url)
        {
            return new WebClient().DownloadData(url);
        }

        public string DownloadStringFromUrl(string url)
        {
            var data = DownloadDataFromUrl(url);
            return Encoding.UTF8.GetString(data);
        }
    }
}