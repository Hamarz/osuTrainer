using System.Collections.Generic;

namespace osuTrainer
{
    public class User
    {
        public int User_id { get; set; }

        public string Username { get; set; }

        public int Pp_rank { get; set; }

        public double Level { get; set; }

        public double Pp_raw { get; set; }

        public string Country { get; set; }

        public List<ScoreInfo> TopScores { get; set; }

        public int CountryRank { get; set; }

        public User()
        {
            TopScores = new List<ScoreInfo>();
        }
    }
}