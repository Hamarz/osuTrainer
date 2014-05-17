namespace osuTrainer
{
    public class Beatmap
    {
        public int Beatmap_id { get; set; }

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
    }
}