using System.Drawing;

namespace osuTrainer
{
    public class ScoreInfo
    {
        public Bitmap RankImage { get; set; }

        public string Artist { get; set; }

        [System.ComponentModel.DisplayName("Beatmap name")]
        public string BeatmapName { get; set; }

        public string Version { get; set; }

        public string Creator { get; set; }

        public GlobalVars.Mods Mods { get; set; }

        public int BPM { get; set; }

        [System.ComponentModel.DisplayName("PP")]
        public int ppRaw { get; set; }

        public int BeatmapId { get; set; }
    }
}