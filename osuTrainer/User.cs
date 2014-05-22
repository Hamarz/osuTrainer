using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using ServiceStack.Text;

namespace osuTrainer
{
    public class User
    {
        private static CustomWebClient client = new CustomWebClient();

        public int User_id { get; set; }

        public string Username { get; set; }

        public int Pp_rank { get; set; }

        public double Level { get; set; }

        public double Pp_raw { get; set; }

        public string Country { get; set; }

        public List<UserBest> BestScores { get; set; }

        /// <summary>
        /// username can be either username or user id
        /// </summary>
        public User(string username)
        {
            GetUser(username);
        }

        public User(string json, bool isjson = true)
        {
            using (var client = new CustomWebClient())
            {
                Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""level"":""(.+?)"".+?""pp_raw"":""(.+?)"".+?""country"":""(.+?)""");
                User_id = Convert.ToInt32(match.Groups[1].Value);
                Username = match.Groups[2].Value;
                Pp_rank = Convert.ToInt32(match.Groups[3].Value);
                Level = Convert.ToDouble(match.Groups[4].Value, CultureInfo.InvariantCulture);
                Pp_raw = Convert.ToDouble(match.Groups[5].Value, CultureInfo.InvariantCulture);
                Country = match.Groups[6].Value;
                json = client.DownloadString(GlobalVars.UserBestAPI + User_id);
                BestScores = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            }
        }

        private void GetUser(string username)
        {
            using (var client = new CustomWebClient())
            {
                string json = client.DownloadString(GlobalVars.UserAPI + username);
                Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""level"":""(.+?)"".+?""pp_raw"":""(.+?)"".+?""country"":""(.+?)""");
                User_id = Convert.ToInt32(match.Groups[1].Value);
                Username = match.Groups[2].Value;
                Pp_rank = Convert.ToInt32(match.Groups[3].Value);
                Level = Convert.ToDouble(match.Groups[4].Value, CultureInfo.InvariantCulture);
                Pp_raw = Convert.ToDouble(match.Groups[5].Value, CultureInfo.InvariantCulture);
                Country = match.Groups[6].Value;
                json = client.DownloadString(GlobalVars.UserBestAPI + User_id);
                BestScores = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            }
        }

        public static string UserString(string username)
        {
            return client.DownloadString("https://osu.ppy.sh/api/get_user?k=" + Properties.Settings.Default.APIKey + "&u=" + username);
        }
    }
}