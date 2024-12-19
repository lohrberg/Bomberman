using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Models.Tiles
{
    internal class DirtTile : Tile
    {
        public DirtTile()
        {
            IsWalkable = false;
            IsDestroyable = true;
            TileMaterial = Enums.Enums.TileMaterial.Dirt;
            Value = 3;
            BlowThroug = false; 
        }
    }
}
