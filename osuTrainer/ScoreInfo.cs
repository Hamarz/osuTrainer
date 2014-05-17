using System;

namespace osuTrainer
{
    public class ScoreInfo
    {
        [System.ComponentModel.Browsable(false)]
        public bool IsFC { get; set; }

        [System.ComponentModel.DisplayName("Beatmap name")]
        public string BeatmapName { get; set; }

        [System.ComponentModel.Browsable(false)]
        public string BeatmapURL { get; set; }

        public int BeatmapId { get; set; }

        [System.ComponentModel.DisplayName("Raw PP")]
        public int ppRaw { get; set; }

        public int ppWeight { get; set; }

        [System.ComponentModel.Browsable(false)]
        public DateTime TimeAgo { get; set; }

        public GlobalVars.RankImage Rankname { get; set; }

        [System.ComponentModel.Browsable(false)]
        public GlobalVars.GameMode Mode { get; set; }
    }
}