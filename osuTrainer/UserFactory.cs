using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace osuTrainer
{
    public static class UserFactory
    {
        public static IUser GetUser(int gameMode)
        {
            switch (gameMode)
            {
                case 0:
                    return new UserStandard();
                case 1:
                    return new UserTaiko();
                case 2:
                    return new UserCtb();
                case 3:
                    return new UserMania();
                default:
                    return null;
            }
        }
    }
}
