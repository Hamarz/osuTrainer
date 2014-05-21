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

        public GlobalVars.Mods Enabled_Mods { get; set; }

        [System.ComponentModel.DisplayName("Raw PP")]
        public int ppRaw { get; set; }

        public int BeatmapId { get; set; }
    }
}