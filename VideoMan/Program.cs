namespace VideoMan
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            DI.Instance.Register<IWebClient, SimpleWebClient>();
            
            //var all = new XnXxVideoUrlResolver().Resolve("https://www.xnxx.com/video-p0khp52/_").ToList();

            //var all = new VidmolyVideoUrlResolver().Resolve("https://vidmoly.me/3ke0y6u5rtgo.html").ToList();

            //var all = new YoutubeVideoUrlResolver().Resolve("https://www.youtube.com/watch?v=Fuv59BhKf_Q").ToList();
        }
    }
}
