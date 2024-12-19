using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BomberMan.Models.Items.Booster
{
    [AddINotifyPropertyChangedInterface]
    public class BoosterModel
    {
        public int BoosterX { get; set; }
        public int BoosterY { get; set; }

        public Enums.Enums.Booster Booster { get; set; } = Enums.Enums.Booster.None;
        public int BoosterTimer { get; set; } = 0;
        public int BoosterTimerLimit { get; set; } = 150;

        public BitmapImage? BoosterImage { get; set; }
    }
}
