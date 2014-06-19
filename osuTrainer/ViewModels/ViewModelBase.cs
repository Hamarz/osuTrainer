using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using osuTrainer.Annotations;

namespace osuTrainer.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected string GetRankImageUri(GlobalVars.Rank rank)
        {
            switch (rank)
            {
                case GlobalVars.Rank.S:
                    return @"/osuTrainer;component/Resources/S_small.png";

                case GlobalVars.Rank.A:
                    return @"/osuTrainer;component/Resources/A_small.png";

                case GlobalVars.Rank.X:
                    return @"/osuTrainer;component/Resources/X_small.png";

                case GlobalVars.Rank.SH:
                    return @"/osuTrainer;component/Resources/SH_small.png";

                case GlobalVars.Rank.XH:
                    return @"/osuTrainer;component/Resources/XH_small.png";

                case GlobalVars.Rank.B:
                    return @"/osuTrainer;component/Resources/B_small.png";

                case GlobalVars.Rank.C:
                    return @"/osuTrainer;component/Resources/C_small.png";

                case GlobalVars.Rank.D:
                    return @"/osuTrainer;component/Resources/D_small.png";
                default:
                    return null;
            }
        }
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
