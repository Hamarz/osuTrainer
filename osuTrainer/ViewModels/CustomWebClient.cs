using System.Net;

namespace osuTrainer.ViewModels
{
    public class CustomWebClient : WebClient
    {
        public CustomWebClient()
        {
            Headers["UserAgent"] = "osu!Trainer";
        }
    }
}