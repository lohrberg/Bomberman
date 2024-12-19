using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Models
{

    [AddINotifyPropertyChangedInterface]

    public class HighScoreModel
    {
        public string PlayerName { get; set; }
   
        public int PlayerHighScore { get; set; }

    }
}
