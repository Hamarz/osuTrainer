using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace osuTrainer
{
    public static class BeatmapParser
    {
        public static bool GetBeatmapFromApi(Beatmap beatmap, int beatmapid, int mode = 0)
        {
            using (var client = new WebClient())
            {
                string json = client.DownloadString(GlobalVars.BeatmapAPI + beatmapid);
                Match match = Regex.Match(json, @"""total_length"":""(.+?)"".+?""hit_length"":""(.+?)"".+?""version"":""(.+?)"".+?""artist"":""(.+?)"".+?""title"":""(.+?)"".+?""creator"":""(.+?)"".+?""bpm"":""(.+?)"".+?""difficultyrating"":""(.+?)"".+?""mode"":""(.+?)""");
                if (match.Groups.Count > 9)
                {
                    beatmap.Beatmap_id = beatmapid;
                    beatmap.Total_length = Convert.ToInt32(match.Groups[1].Value);
                    beatmap.Hit_length = Convert.ToInt32(match.Groups[2].Value);
                    beatmap.Version = match.Groups[3].Value;
                    beatmap.Artist = match.Groups[4].Value;
                    beatmap.Title = match.Groups[5].Value;
                    beatmap.Creator = match.Groups[6].Value;
                    beatmap.Bpm = Convert.ToDouble(match.Groups[7].Value, CultureInfo.InvariantCulture);
                    beatmap.Difficultyrating = Convert.ToDouble(match.Groups[8].Value, CultureInfo.InvariantCulture);
                    beatmap.Mode = (GlobalVars.GameMode)Enum.Parse(typeof(GlobalVars.GameMode), match.Groups[9].Value);
                    beatmap.Url = GlobalVars.Beatmap + beatmapid;
                    GetBeatmapUrls(beatmap);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void GetBeatmapUrls(Beatmap beatmap)
        {
            using (var client = new WebClient())
            {
                string html = client.DownloadString(beatmap.Url);
                Match match = Regex.Match(html, @"<img class='bmt' src=""(.+?)"">");
                beatmap.ThumbnailUrl = "http:" + match.Groups[1].Value;
                match = Regex.Match(html, @"href=""(.+?)"".+?novid");
                beatmap.NoVideoUrl = @"http://osu.ppy.sh/" + match.Groups[1].Value;
                match = Regex.Match(html, @"href=""(.+?)"".+?beatmap.p");
                beatmap.DownloadUrl = @"http://osu.ppy.sh/" + match.Groups[1].Value;
            }
        }
    }
}