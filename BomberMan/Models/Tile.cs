using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BomberMan.Enums;
using PropertyChanged;

namespace BomberMan.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Tile

    {
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int Height { get; } = 50;
        public int Width { get; } = 50;
        public int Timer { get; set; } = 0;
        public Enums.Enums.TileMaterial TileMaterial{ get; set; }
        public Enums.Enums.TileState TileState { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsDestroyable { get; set;}
        public int Value { get; set; }
        public bool BlowThroug {  get; set; }
        public int StateTimer { get; set;}
        public Enums.Enums.Booster Booster { get; set; } = Enums.Enums.Booster.None;

        
    }


}

