using System.Collections.Generic;
namespace osuTrainer
{
    public interface IUser
    {
        int User_id { get; set; }

        string Username { get; set; }

        int PpRank { get; set; }

        double Level { get; set; }

        double PpRaw { get; set; }

        string Country { get; set; }

        List<UserBest> BestScores { get; set; }
    }
}