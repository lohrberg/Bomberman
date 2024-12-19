using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BomberMan.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ScoreModel
    {
        // Egenskap för LabelText
        public string ScoreLabelText { get; set; }

        // Egenskap för Score
        public int CurrentPlayerScore { get; set; }


        // Konstruktor för att initiera LabelText och Score
        public ScoreModel()
        {
            ScoreLabelText = "YOUR SCORE";
            CurrentPlayerScore = 0;  // Initiera Score till 0
           
        }

        // Metod för att lägga till poäng och meddela förändring
        public int AddPoints(int points)
        {
            CurrentPlayerScore += points;  // Fody tar hand om att notifiera ändringen
            return CurrentPlayerScore;
        }
    }
}
