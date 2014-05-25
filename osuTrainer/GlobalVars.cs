using System;

namespace osuTrainer
{
    public static class GlobalVars
    {
        public static readonly string Beatmap = @"http://osu.ppy.sh/b/";
        public static string BeatmapAPI = @"http://osu.ppy.sh/api/get_beatmaps?k=" + Properties.Settings.Default.APIKey + "&b=";
        public static string UserAPI = @"http://osu.ppy.sh/api/get_user?k=" + Properties.Settings.Default.APIKey + "&u=";
        public static readonly string Bloodcat = @"http://bloodcat.com/osu/m/";
        public static string UserBestAPI = @"https://osu.ppy.sh/api/get_user_best?k=" + Properties.Settings.Default.APIKey + "&u=";
        public static readonly string ThumbUrl = @"https://b.ppy.sh/thumb/40924l.jpg";
        public static readonly string DownloadURL = @"http://osu.ppy.sh/d/";
        public static readonly string OsuDirectURL = @"osu://dl/";

        // 0 = osu! 1 = Taiko 2 = CtB 3 = osu!mania
        public static readonly string Mode = @"&m=";

        public enum RankImage
        {
            //Hidden SS
            XH_small,

            SH_small,

            //SS
            X_small,

            S_small,
            A_small,
            B_small,
            C_small,
            D_small
        }

        public enum Rank
        {
            XH,
            SH,
            X,
            S,
            A,
            B,
            C,
            D
        }

        public enum GameMode
        {
            osu = 0,
            Taiko = 1,
            CtB = 2,
            Mania = 3,
        }

        [Flags]
        public enum Mods
        {
            None = 0,
            NF = 1,
            EZ = 2,
            NV = 4,
            HD = 8,
            HR = 16,
            SD = 32,
            DT = 64,
            Relax = 128,
            HT = 256,
            NC = 512,
            FL = 1024,
            Autoplay = 2048,
            SpunOut = 4096,
            Relax2 = 8192,  // Autopilot?
            Perfect = 16384,
            Key4 = 32768,
            Key5 = 65536,
            Key6 = 131072,
            Key7 = 262144,
            Key8 = 524288,
            keyMod = Key4 | Key5 | Key6 | Key7 | Key8,
            FadeIn = 1048576,
            Random = 2097152,
            LastMod = 4194304,
            FreeModAllowed = NF | EZ | HD | HR | SD | FL | FadeIn | Relax | Relax2 | SpunOut | keyMod
        }
    }
}