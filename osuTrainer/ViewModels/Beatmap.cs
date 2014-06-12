using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace osuTrainer.ViewModels
{
    public class Beatmap
    {
        public Beatmap()
        {
        }

        public Beatmap(int beatmapId, string apiKey)
        {
            using (var client = new CustomWebClient())
            {
                string json = client.DownloadString(GlobalVars.BeatmapApi + apiKey + "&b=" + beatmapId);

                Match match = Regex.Match(json,
                    @"""beatmapset_id"":""(.+?)"".+?""total_length"":""(.+?)"".+?""hit_length"":""(.+?)"".+?""version"":""(.+?)"".+?""artist"":""(.+?)"".+?""title"":""(.+?)"".+?""creator"":""(.+?)"".+?""bpm"":""(.+?)"".+?""difficultyrating"":""(.+?)"".+?""mode"":""(.+?)""");
                BeatmapSet_Id = Convert.ToInt32(match.Groups[1].Value);
                Beatmap_Id = beatmapId;
                Total_Length = Convert.ToInt32(match.Groups[2].Value);
                Hit_Length = Convert.ToInt32(match.Groups[3].Value);
                Version = match.Groups[4].Value;
                Artist = match.Groups[5].Value;
                Title = match.Groups[6].Value;
                Creator = match.Groups[7].Value;
                Bpm = Convert.ToDouble(match.Groups[8].Value, CultureInfo.InvariantCulture);
                Difficultyrating = Convert.ToDouble(match.Groups[9].Value, CultureInfo.InvariantCulture);
                Mode = (GlobalVars.GameMode) Enum.Parse(typeof (GlobalVars.GameMode), match.Groups[10].Value);
            }
        }

        public int BeatmapSet_Id { get; set; }
        public int Beatmap_Id { get; set; }

        public int Approved { get; set; }

        public DateTime Approved_Date { get; set; }

        public DateTime Last_Update { get; set; }

        public int Total_Length { get; set; }

        public int Hit_Length { get; set; }

        public string Version { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Creator { get; set; }

        public double Bpm { get; set; }

        public double Difficultyrating { get; set; }
        public int Diff_Size { get; set; }
        public int Diff_Overall { get; set; }
        public int Diff_Approach { get; set; }
        public int Diff_Drain { get; set; }

        public GlobalVars.GameMode Mode { get; set; }
    }
}