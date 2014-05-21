using System.Net;

namespace osuTrainer
{
    internal class CustomWebClient : WebClient
    {
        public CustomWebClient()
        {
            this.Headers["UserAgent"] = "osu!Trainer";
        }
    }
}