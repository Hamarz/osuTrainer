namespace osuTrainer.ViewModels
{
    public class ScoreInfo
    {
        public string RankImage { get; set; }

        public string Version { get; set; }

        public GlobalVars.Mods Mods { get; set; }
        public double Bpm { get; set; }

        public double Pp { get; set; }

        public int BeatmapId { get; set; }
        public int BeatmapSetId { get; set; }
        public string BeatmapName { get; set; }
        public string BeatmapArtist { get; set; }
        public string BeatmapCreator { get; set; }
        public string BeatmapUrl { get; set; }
        public string BloodcatDlUrl { get; set; }
        public string DlUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string OsuDirectCmd { get; set; }
        public string TotalTime { get; set; }
        public string DrainingTime { get; set; }
    }
}