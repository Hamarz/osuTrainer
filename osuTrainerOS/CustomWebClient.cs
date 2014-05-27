using System.Net;

namespace osuTrainerOS
{
    internal class CustomWebClient : WebClient
    {
        public CustomWebClient()
        {
            this.Headers["UserAgent"] = "osu!Trainer";
        }
    }
}