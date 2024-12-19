using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PropertyChanged;
using BomberMan.Enums;
using System.Windows.Automation;


namespace BomberMan.Models.Items.Bombs
{
    [AddINotifyPropertyChangedInterface] // Fody hanterar INotifyPropertyChanged automatiskt
    public class BombModel
    {
        // Egenskaper för spelarens X- och Y-koordinater
        public int BombX { get; set; }  // Fody hanterar PropertyChanged
        public int BombY { get; set; }

        // Egenskap för att se åt vilket hått bomben är på väg. 
        public Enum BombOrientation { get; set; } = Enums.Enums.ObjectOrientation.None;

        public int BombTimer { get; set; } = 0;
        public int BombTimerLimit { get; set; } = 150;
        public int BombSpeed { get; set; } = 1;
        public int BombExplosionDistance { get; set; } = 2;
        public BitmapImage? BombImages { get; set; } = null;
        // Read-only egenskaper för spelarens bredd och höjd (konstant värde)
        public int BombWidth { get; } = 50; // Konstant, ingen notifiering behövs
        public int BombHeight { get; } = 50;

        // bombens egenskaper för animation 
        public int Framerate { get; set; } = 0;
        public int Steps { get; set; } = 0;

        // Egenskap för Bombens bild
        
        //Konstruktor för xUnitTest
        public BombModel(bool test)
        {

            
            BombImages = null;
        }

        public BombModel()
        {

            BombImages = new BitmapImage(new Uri("pack://application:,,,/BomberMan;component/Assets/Images/Bombs/RegularBomb/bomb2_01.png")); // Fody hanterar PropertyChanged
        }


    }
   
}
