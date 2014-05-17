using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace osuTrainer
{
    internal static class UserParser
    {
        public static bool GetUser(string username, User user)
        {
            using (var client = new WebClient())
            {
                string json = client.DownloadString(GlobalVars.UserAPI + username);
                Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""level"":""(.+?)"".+?""pp_raw"":""(.+?)"".+?""country"":""(.+?)""");
                if (match.Groups.Count > 6)
                {
                    user.User_id = Convert.ToInt32(match.Groups[1].Value);
                    user.Username = match.Groups[2].Value;
                    user.Pp_rank = Convert.ToInt32(match.Groups[3].Value);
                    user.Level = Convert.ToDouble(match.Groups[4].Value);
                    user.Pp_raw = Convert.ToDouble(match.Groups[5].Value);
                    user.Country = match.Groups[6].Value;
                    user.CountryRank = GetUserCountryRank(user.User_id);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static int GetUserCountryRank(int userId, int mode = 0)
        {
            using (var client = new WebClient())
            {
                string html = client.DownloadString(GlobalVars.User + "u=" + userId + "&m=" + mode);
                Match match = Regex.Match(html, @"\n#(.+)");
                return int.Parse(match.Groups[1].Value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            }
        }
    }
}