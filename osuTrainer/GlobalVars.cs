using System;

namespace osuTrainer
{
    public class GlobalVars
    {
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
            Relax2 = 8192, // Autopilot?
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

        public enum Rank
        {
            XH = 0,
            SH = 1,
            X = 2,
            S = 3,
            A = 4,
            B = 5,
            C = 6,
            D = 7
        }

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

        public static readonly string BeatmapUrl = @"http://osu.ppy.sh/b/";
        public static string BeatmapApi = @"http://osu.ppy.sh/api/get_beatmaps?k=";
        public static string UserApi = @"http://osu.ppy.sh/api/get_user?k=";
        public static readonly string BloodcatUrl = @"http://bloodcat.com/osu/m/";
        public static string UserBestApi = @"https://osu.ppy.sh/api/get_user_best?k=";
        public static readonly string ThumbUrl = @"https://b.ppy.sh/thumb/";
        public static readonly string DownloadURL = @"http://osu.ppy.sh/d/";
        public static readonly string OsuDirectURL = @"osu://dl/";
        // 0 = osu! 1 = Taiko 2 = CtB 3 = osu!mania
        public static readonly string Mode = @"&m=";
    }
}