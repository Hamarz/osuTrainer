using System;

namespace osuTrainer
{
    public class UserBest : IComparable<UserBest>
    {
        public int Beatmap_Id { get; set; }

        public int Score { get; set; }

        public int MaxCombo { get; set; }

        public int Count50 { get; set; }

        public int Count100 { get; set; }

        public int Count300 { get; set; }

        public int CountMiss { get; set; }

        public int CountKatu { get; set; }

        public int CountGeki { get; set; }

        public int Perfect { get; set; }

        public GlobalVars.Mods Enabled_Mods { get; set; }

        public int User_Id { get; set; }

        public DateTime Date { get; set; }

        public GlobalVars.Rank Rank { get; set; }

        public double PP { get; set; }

        public int CompareTo(UserBest other)
        {
            return Beatmap_Id.CompareTo(other.Beatmap_Id);
        }
    }
}