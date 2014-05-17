using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace osuTrainer
{
    public static class ScoreParser
    {
        public static string CombinePatterns(string[] patterns)
        {
            string result = "";
            for (int i = 0; i < patterns.Length; i++)
            {
                result += @".+?" + patterns[i];
            }
            return result;
        }

        public static string FormatScores(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty).Replace("%", "%\n");
        }

        public static void GetTopScores(User user, int mode = 0, bool fconly = true)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(GlobalVars.TopScores + "u=" + user.User_id + "&m=" + mode))
                {
                    htmlDocument.Load(stream);
                    HtmlNodeCollection divTags = htmlDocument.DocumentNode.SelectNodes(@"div[contains(@id,'performance-') and contains(@class,'prof-beatmap')]");
                    foreach (var node in divTags)
                    {
                        user.TopScores.Add(new ScoreInfo
                        {
                            Rankname = (GlobalVars.RankImage)Enum.Parse(typeof(GlobalVars.RankImage), Regex.Match(node.InnerHtml, @"s\/(.+?)\.png").Groups[1].Value),
                            BeatmapURL = @"http://osu.ppy.sh" + Regex.Match(node.InnerHtml, @"f=""(.+?)""").Groups[1].Value,
                            BeatmapName = Regex.Match(node.InnerHtml, @""">(.+?)<\/a").Groups[1].Value,
                            BeatmapId = Convert.ToInt32(Regex.Match(node.InnerHtml, @"(\d+)\?m").Groups[1].Value),
                            TimeAgo = DateTime.Parse(Regex.Match(node.InnerHtml, @"e='(.+?)'").Groups[1].Value),
                            ppRaw = Convert.ToInt32(Regex.Match(node.InnerHtml, @">(\d{1,3})p").Groups[1].Value),
                            ppWeight = Convert.ToInt32(Regex.Match(node.InnerHtml, @"(\d{1,3})%").Groups[1].Value)
                        });
                    }
                }
            }
        }

        public static Dictionary<int, ScoreInfo> GetScoreSuggestions(Dictionary<int, ScoreInfo> existingScores, User user, int minpp, int maxpp, int minSuggestions)
        {
            Dictionary<int, ScoreInfo> scoreSuggestions = new Dictionary<int, ScoreInfo>();
            int currentPage;
            string performanceRankingUrl;
            if (user.Pp_rank > 10000)
            {
                currentPage = (int)Math.Ceiling(user.CountryRank / 50.0);
                performanceRankingUrl = GlobalVars.CountryRanking + user.Country + "&page=";
            }
            else
            {
                currentPage = (int)Math.Ceiling(user.Pp_rank / 50.0);
                performanceRankingUrl = GlobalVars.AllRanking;
            }
            int userRank = 50;
            int userId;
            List<ScoreInfo> topScores;
            //Get User ID of last user on page then go up
            using (var client = new WebClient())
            {
                while (currentPage > 1)
                {
                    string html = client.DownloadString(performanceRankingUrl + currentPage);
                    while (userRank > 1)
                    {
                        MatchCollection matches = Regex.Matches(html, @"\/u\/(\d+?)""");
                        userId = Convert.ToInt32(matches[userRank - 1].Groups[1].Value);
                        topScores = GetTopScores(userId);
                        topScores.RemoveAll(o => existingScores.Keys.Contains(o.BeatmapId) || scoreSuggestions.Keys.Contains(o.BeatmapId) || o.ppRaw < minpp || o.ppRaw > maxpp);
                        foreach (ScoreInfo score in topScores)
                        {
                            scoreSuggestions.Add(score.BeatmapId, score);
                        }
                        userRank--;
                        if (scoreSuggestions.Count > minSuggestions)
                        {
                            return scoreSuggestions;
                        }
                    }
                    userRank = 50;
                    currentPage--;
                }
            }
            return scoreSuggestions;
        }

        public static List<ScoreInfo> GetTopScores(int userId, int mode = 0)
        {
            List<ScoreInfo> topScores = new List<ScoreInfo>();
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(GlobalVars.TopScores + "u=" + userId + "&m=" + mode))
                {
                    htmlDocument.Load(stream);
                    HtmlNodeCollection divTags = htmlDocument.DocumentNode.SelectNodes(@"div[contains(@id,'performance-') and contains(@class,'prof-beatmap')]");
                    foreach (var node in divTags)
                    {
                        topScores.Add(new ScoreInfo
                        {
                            Rankname = (GlobalVars.RankImage)Enum.Parse(typeof(GlobalVars.RankImage), Regex.Match(node.InnerHtml, @"s\/(.+?)\.png").Groups[1].Value),
                            BeatmapURL = @"http://osu.ppy.sh" + Regex.Match(node.InnerHtml, @"f=""(.+?)""").Groups[1].Value,
                            BeatmapName = Regex.Match(node.InnerHtml, @""">(.+?)<\/a").Groups[1].Value,
                            BeatmapId = Convert.ToInt32(Regex.Match(node.InnerHtml, @"(\d+)\?m").Groups[1].Value),
                            TimeAgo = DateTime.Parse(Regex.Match(node.InnerHtml, @"e='(.+?)'").Groups[1].Value),
                            ppRaw = Convert.ToInt32(Regex.Match(node.InnerHtml, @">(\d{1,3})p").Groups[1].Value),
                            ppWeight = Convert.ToInt32(Regex.Match(node.InnerHtml, @"(\d{1,3})%").Groups[1].Value)
                        });
                    }
                }
            }
            return topScores;
        }
    }
}