using BomberMan.Models.Gameboard;
using BomberMan.Models.Items.Bombs;
using BomberMan.Models.Items.Booster;
using BomberMan.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static BomberMan.Enums.Enums;

namespace BomberMan.Models.BoostersBoard
{
    [AddINotifyPropertyChangedInterface] // Fody auto implementerar INotifyPropertyChanged

    public class BoosterBoard
    {
        private readonly MainViewModel _mainViewModel;
        public ObservableCollection<BoosterModel> BoosterCollection { get; set; } = new ObservableCollection<BoosterModel>();

        public BoosterBoard()
        {
            BoosterCollection = new ObservableCollection<BoosterModel>();

        }

        //Tar bort booster från listan med boosters
        public void RemoveBooster(Tile newTile)
        {
            BoosterModel booster = GetBoosterAtPosition(newTile.TileX, newTile.TileY);
            if (BoosterCollection.Contains(booster))
            {
                BoosterCollection.Remove(booster);
            }
        }

        //Random för att se om boostern som sprängs fram ska vara något annat än coin
        public bool ShouldDropSpecialBooster()
        {
            return new Random().Next(0, 4) == 0;
        }

        //Slumpar fram vilken typ av special boosters som sprängs fram, antingen Speed eller Diamond
        public Booster DropRandomBooster()
        {
            Random random = new Random();
            // Går igenom listan med Booster och ger en random inom det angivna urvalet
            int randomBooster = random.Next(1, 3);
            return (Booster)randomBooster;
        }

        //Kollar vilken typ av booster som finns på angiven position
        public BoosterModel GetBoosterAtPosition(int x, int y)
        {
            // Använd LINQ för att hitta en booster med rätt position (TileX och TileY)
            return BoosterCollection.FirstOrDefault(t => t.BoosterX == x && t.BoosterY == y);
        }

        //Lägger till en booster när tile sprängs bort
        public BoosterModel DropBoosterForTile(Tile destroyedTile)

        {
            BoosterModel newBooster = null;

            if (destroyedTile.TileMaterial == TileMaterial.Dirt) //Kan bara komma fram en booster ifall tilen är dirt
            {

                //Ifall den inte ska droppa en special booster så droppas ett mynt
                if (!ShouldDropSpecialBooster())
                {
                    newBooster = new BoosterModel

                    {
                        BoosterX = destroyedTile.TileX,
                        BoosterY = destroyedTile.TileY,
                        Booster = Booster.Coin,
                        BoosterImage = new BitmapImage(new Uri("pack://application:,,,/BomberMan;component/Assets/Images/Items/coin.png"))

                    };

                    destroyedTile.Booster = Booster.Coin;

                }
                else
                {
                    Booster randomBooster = DropRandomBooster();
                    newBooster = new BoosterModel
                    {
                        Booster = randomBooster,
                        BoosterX = destroyedTile.TileX,
                        BoosterY = destroyedTile.TileY,
                    };


                    switch (randomBooster)
                    {
                        case Booster.Speed:
                            newBooster.BoosterImage = new BitmapImage(new Uri("pack://application:,,,/BomberMan;component/Assets/Images/Items/boot.png"));
                            destroyedTile.Booster = Booster.Speed;
                            break;
                        case Booster.Diamond:
                            newBooster.BoosterImage = new BitmapImage(new Uri("pack://application:,,,/BomberMan;component/Assets/Images/Items/diamond.png"));
                            destroyedTile.Booster = Booster.Diamond;
                            break;
                    }
                }
            }
            return newBooster;
        }

        //Returnerar ett värde som ger effekt när spelaren plockar upp en booster
        public int PickUpBooster(Tile tile, int value)
        {

            BoosterModel existingBooster = GetBoosterAtPosition(tile.TileX, tile.TileY);

            if (existingBooster != null)
            {
                switch (existingBooster.Booster)
                {
                    case Booster.Coin:
                        value = 500;
                        break;
                    case Booster.Speed:
                        value+=1;
                        break;
                    case Booster.Diamond:
                        value=1000;
                        break;

                }
                tile.Booster = Booster.None;
                RemoveBooster(tile);
                return value;
            }
            else
            {
                return value;
            }

        }



    }
}
