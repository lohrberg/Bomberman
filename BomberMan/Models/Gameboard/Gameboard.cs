using BomberMan.Models;
using BomberMan.Models.BoostersBoard;
using BomberMan.Models.Items.Bombs;
using BomberMan.Models.Items.Booster;
using BomberMan.Models.Tiles;
using BomberMan.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static BomberMan.Enums.Enums;


namespace BomberMan.Models.Gameboard
{
    [AddINotifyPropertyChangedInterface]
    public class GameBoard
    {
        // Skapar en samling tiles, svårighetsgrad, och ett antal tiles som ska användas. 
        public ObservableCollection<Tile> TilesCollection { get; set; } = new ObservableCollection<Tile>();
        public int Difficulty { get; set; } = 1;
        public int TilesAmount { get; set; } = 80;

      

        // Skapar gameboard beroende på level och fyller den med tiles. 
        public GameBoard(int level)
        {
            Difficulty = level;
            FillGameboard();
        }

        
        public void FillGameboard()
        {
            switch (Difficulty)                  // Svårighetsgraden avgör hur många tiles som sätts ut
            { 
                case 1: TilesAmount = 80;
                break;
            case 2: TilesAmount = 120;
                break;
            case 3: TilesAmount = 150;
                break;
            case 4:
                  TilesAmount = 180;
                break;
            }

            // Först skapas de tiles som alltid ska finnas. 15x15 grasstiles med Stonetiles runt om. 
            int[] evenNumbers = { 2, 4, 6, 8, 10, 12 };
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Tile TempTile = new GrassTile();
                    if (i == 0 || i == 15)
                    {
                        TempTile = new StoneTile();
                    }
                    else if (j == 0 || j == 15)
                    {
                        TempTile = new StoneTile();
                    }
                    TempTile.TileX = i * 50;
                    TempTile.TileY = j * 50;

                    if (TempTile.TileX == 0)
                    {
                        TempTile = new StoneTile();
                        TempTile.TileX = i * 50;
                        TempTile.TileY = j * 50;
                    }
                    else if (TempTile.TileX / 50 == 14)
                    {
                        TempTile = new StoneTile();
                        TempTile.TileX = i * 50;
                        TempTile.TileY = j * 50;
                    }
                    else if (TempTile.TileY == 0)
                    {
                        TempTile = new StoneTile();
                        TempTile.TileX = i * 50;
                        TempTile.TileY = j * 50;
                    }
                    else if (TempTile.TileY / 50 == 14)
                    {
                        TempTile = new StoneTile();
                        TempTile.TileX = i * 50;
                        TempTile.TileY = j * 50;
                    }
                    // StoneTiles läggs även för att skapa mönstret. 
                    else if (evenNumbers.Contains(i) && evenNumbers.Contains(j))
                    {
                        TempTile = new StoneTile();
                        TempTile.TileX = i * 50;
                        TempTile.TileY = j * 50;
                    }

                    AddOrReplaceTile(TempTile);
                }
            }
            //Sedan skapas DirtTiles beroende på hur svårighetsgraden. 
            int[] possibleValues = { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650 };
            Random random = new Random();
            for (int i = 0; i < TilesAmount; i++)
            {
                int x = possibleValues[random.Next(possibleValues.Length)];
                int y = possibleValues[random.Next(possibleValues.Length)];
                if (x > 100 || y > 100)
                {
                    AddOrReplaceTile(new DirtTile() { TileX = x, TileY = y });
                }
            }
        }

        // Metoden för att lägga till Tiles kollar om det redan finns en befintlig Tile med samma kordinater. Den byts då ut. 
        public void AddOrReplaceTile(Tile newTile)
        {
            // Hitta den befintliga tile med samma X och Y, om den finns
            Tile existingTile = GetTileAtPosition(newTile.TileX, newTile.TileY);

            if (existingTile != null)
            {
                // om den valda Tilen inte är en StoneTile Ersätts den med en dirtTile
                if (existingTile.Value != 2)
                {
                    // Ersätt den befintliga tile med den nya
                    int index = TilesCollection.IndexOf(existingTile);
                    TilesCollection[index] = newTile;  // Uppdatera tile i samma position
                }

            }
            else
            {
                // Om ingen befintlig tile hittades, lägg till den nya tile
                TilesCollection.Add(newTile);
            }
        }



        public Tile GetTileAtPosition(int x, int y)
        {
            // Använder LINQ för att hitta en tile med rätt position (TileX och TileY)
            return TilesCollection.FirstOrDefault(t => t.TileX == x && t.TileY == y);
        }

        public Tile GetRandomTile()                                         // sponsored by CHAT
        {
            if (TilesCollection.Count == 0)
            {
                return null; // Hantera om samlingen är tom
            }

            Random random = new Random();
            while (true)
            {
                int randomTileIndex = random.Next(TilesCollection.Count);
                Tile randomTile = TilesCollection[randomTileIndex];
                if (randomTile is GrassTile)
                {
                    return randomTile;
                }
            }
        }

        public bool CheckIfAnyDirtTilesLeft()                                   // Metoden kollar om det finns några DirtTiles kvar. Metoden kan i nuläget inte bara kolla efter DirtTiles, eftersom att det förekommer DirtTiles som agerar som gräs. Detta är tänkt att ändras senare. 
        {
            // Använd LINQ för att kontrollera om det finns några jordtiles (DirtTile) kvar i samlingen
            return TilesCollection.Any(tile => tile.TileMaterial is Enums.Enums.TileMaterial.Dirt);
        }

    }
}
  

