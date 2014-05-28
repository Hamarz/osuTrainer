using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace osuTrainerOS
{
    public class UserTaiko : IUser
    {
        public int User_id { get; set; }

        public string Username { get; set; }

        public int PpRank { get; set; }

        public double Level { get; set; }

        public double PpRaw { get; set; }

        public string Country { get; set; }

        public List<UserBest> BestScores { get; set; }
        private static readonly CustomWebClient client = new CustomWebClient();

        public UserTaiko()
        { }

        public void GetInfo(string nameorid)
        {
            using (var client = new CustomWebClient())
            {
                //standard
                string json = client.DownloadString(GlobalVars.UserAPI + nameorid + GlobalVars.Mode + 1);
                Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""level"":""(.+?)"".+?""pp_raw"":""(.+?)"".+?""country"":""(.+?)""");
                User_id = Convert.ToInt32(match.Groups[1].Value);
                Username = match.Groups[2].Value;
                PpRank = Convert.ToInt32(match.Groups[3].Value);
                Level = Convert.ToDouble(match.Groups[4].Value, CultureInfo.InvariantCulture);
                PpRaw = Convert.ToDouble(match.Groups[5].Value, CultureInfo.InvariantCulture);
                Country = match.Groups[6].Value;
                json = client.DownloadString(GlobalVars.UserBestAPI + User_id + GlobalVars.Mode + 1);
                BestScores = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            }
        }

        public void GetInfo(string json, bool isjson = true)
        {
            using (var client = new CustomWebClient())
            {
                Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""level"":""(.+?)"".+?""pp_raw"":""(.+?)"".+?""country"":""(.+?)""");
                User_id = Convert.ToInt32(match.Groups[1].Value);
                Username = match.Groups[2].Value;
                PpRank = Convert.ToInt32(match.Groups[3].Value);
                Level = Convert.ToDouble(match.Groups[4].Value, CultureInfo.InvariantCulture);
                PpRaw = Convert.ToDouble(match.Groups[5].Value, CultureInfo.InvariantCulture);
                Country = match.Groups[6].Value;
                json = client.DownloadString(GlobalVars.UserBestAPI + User_id + GlobalVars.Mode + 1);
                BestScores = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            }
        }

        public static string UserString(string username)
        {
            return client.DownloadString(GlobalVars.UserAPI + username + GlobalVars.Mode + 1);
        }
    }
}
