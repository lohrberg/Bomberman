using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberMan.Enums;

namespace BomberMan.Models.Tiles
{
    internal class GrassTile : Tile
    {
        public GrassTile()
        {
                IsWalkable = true;
                IsDestroyable = true;
                TileMaterial = Enums.Enums.TileMaterial.Grass;
                Value = 1;
                BlowThroug = true;

        }
    }
}
