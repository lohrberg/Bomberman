using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Enums
{
    public class Enums
    {
        public enum ObjectOrientation       // Hej :) ! 
        {
            Up,
            Down,
            Left,
            Right, 
            None
        }

        public enum TileMaterial
        {
            Grass,
            Stone,
            Dirt,
            Fire
        }

        public enum TileState
        {
           Default, 
           Burning,
           Point
        }

        public enum Booster
        {
            None,
            Speed,
            Diamond,
            Coin
        }

     
    }
}
