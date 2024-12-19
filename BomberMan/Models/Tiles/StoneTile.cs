using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Models.Tiles
{
    public class StoneTile : Tile
    {
        public StoneTile()
        {
            IsWalkable = false;
            IsDestroyable = false;
            TileMaterial = Enums.Enums.TileMaterial.Stone;
            Value = 2;
            BlowThroug = false;
        }
    }
}
