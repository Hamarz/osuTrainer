using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace osuTrainerOS
{
    public class Beatmap
    {
        public int Beatmap_id { get; set; }

        public int BeatmapSet_id { get; set; }

        public int Total_length { get; set; }

        public int Hit_length { get; set; }

        public string Version { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Creator { get; set; }

        public double Bpm { get; set; }

        public double Difficultyrating { get; set; }

        public GlobalVars.GameMode Mode { get; set; }

        public string Url { get; set; }

        public string DownloadUrl { get; set; }

        public string BloodcatUrl { get; set; }

        public string NoVideoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public Beatmap()
        {
        }

        public Beatmap(int beatmapid)
        {
            using (var client = new CustomWebClient())
            {
                string json = client.DownloadString(GlobalVars.BeatmapAPI + beatmapid);
                Match match = Regex.Match(json, @"""beatmapset_id"":""(.+?)"".+?""total_length"":""(.+?)"".+?""hit_length"":""(.+?)"".+?""version"":""(.+?)"".+?""artist"":""(.+?)"".+?""title"":""(.+?)"".+?""creator"":""(.+?)"".+?""bpm"":""(.+?)"".+?""difficultyrating"":""(.+?)"".+?""mode"":""(.+?)""");
                BeatmapSet_id = Convert.ToInt32(match.Groups[1].Value);
                Beatmap_id = beatmapid;
                Total_length = Convert.ToInt32(match.Groups[2].Value);
                Hit_length = Convert.ToInt32(match.Groups[3].Value);
                Version = match.Groups[4].Value;
                Artist = match.Groups[5].Value;
                Title = match.Groups[6].Value;
                Creator = match.Groups[7].Value;
                Bpm = Convert.ToDouble(match.Groups[8].Value, CultureInfo.InvariantCulture);
                Difficultyrating = Convert.ToDouble(match.Groups[9].Value, CultureInfo.InvariantCulture);
                Mode = (GlobalVars.GameMode)Enum.Parse(typeof(GlobalVars.GameMode), match.Groups[10].Value);
                Url = GlobalVars.Beatmap + beatmapid;
                ThumbnailUrl = @"http://b.ppy.sh/thumb/" + BeatmapSet_id + "l.jpg";
                BloodcatUrl = GlobalVars.Bloodcat + BeatmapSet_id;
            }
        }
    }
}