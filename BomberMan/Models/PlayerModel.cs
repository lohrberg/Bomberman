using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PropertyChanged;
using BomberMan.Enums;

namespace BomberMan.Models
{
    [AddINotifyPropertyChangedInterface] // Fody hanterar INotifyPropertyChanged automatiskt
    public class PlayerModel
    {
        // Egenskaper för spelarens X- och Y-koordinater
        public int PlayerX { get; set; } = 50;  // Fody hanterar PropertyChanged
        public int PlayerY { get; set; } = 50; 
        public Enum PlayerOrientation { get; set; } = Enums.Enums.ObjectOrientation.Down;

        // Read-only egenskaper för spelarens bredd och höjd (konstant värde)
        public int PlayerWidth { get; } = 50; // Konstant, ingen notifiering behövs
        public int PlayerHeight { get; } = 50;

        // Egenskap för spelarens bild
        public BitmapImage ?PlayerImage { get; set; } // Fody hanterar PropertyChanged
    }
}
