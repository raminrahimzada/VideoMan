
namespace VideoMan
{
    // ReSharper disable once InconsistentNaming
    public static class App
    {
        public static IWebClient Client { get; set; }

        public static void RegisterStartup()
        {
            Client=new SimpleWebClient();
        }
    }
}