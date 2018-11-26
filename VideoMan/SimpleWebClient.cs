using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace VideoMan
{
    public class SimpleWebClient : IWebClient
    {
        public byte[] DownloadDataFromUrl(string url)
        {
            var request = WebRequest.CreateHttp(url);
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            return stream.ReadToEnd();
        }
       
        public string DownloadStringFromUrl(string url)
        {
            var data = DownloadDataFromUrl(url);
            return Encoding.UTF8.GetString(data);
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SimpleWebClient()
        {
            Dispose(false);
        }
    }
}