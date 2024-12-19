using BomberMan.Models.Items.Bombs;
using BomberMan.Services;
using BomberMan.ViewModels;
using PropertyChanged;
using System.Collections.ObjectModel;
using BomberMan.Models.Gameboard;
using BomberMan.Models.BoostersBoard;
using static BomberMan.Enums.Enums;
using System.Windows.Media.Imaging;
using BomberMan.Models.Items.Booster;
using BomberMan.Models.Tiles;

namespace BomberMan.Models.BombBoard
{
    [AddINotifyPropertyChangedInterface] // Fody auto implementerar INotifyPropertyChanged

    public class BombBoard
    {
        private readonly MainViewModel _mainViewModel;
        public ObservableCollection<BombModel> ActiveBombList { get; set; } = new ObservableCollection<BombModel>();
        private readonly IImageLoaderService _imageLoaderService;
        private int _steps = 0; // Håller koll på vilken animationsbild som ska visas
        private int _slowDownFrameRate = 0; // Kontrollerar hastigheten för animationen
        private int _BombSteps = 0;
        public List<BitmapImage> BombMovements { get; set; } // Lista som innehåller Bombens animationer

        // Ljud som har att göra med bomberna. 
        CachedSound smallExplosion = new CachedSound("Assets/Audio/Effects/explosion_01.wav");
        CachedSound spawnBomb = new CachedSound("Assets/Audio/Effects/spawn_01.wav");

        public BombBoard()
        {
            ActiveBombList = new ObservableCollection<BombModel>();
        }

        //Konstruktor för xUnitTest
        public BombBoard(bool isTest)
        {
            smallExplosion = null;
            spawnBomb = null;
        }

        // Metod som lägger ut random bomber på spelbrädet. 
        public void SpawnBombRandomly(GameBoard gameboard)
        {
            // Plockar en Tile på från spelbrädet och skapar en ny bomb. 
            Tile randomTile = gameboard.GetRandomTile();
            BombModel randoBomb = new BombModel();
            randoBomb.BombX = randomTile.TileX;
            randoBomb.BombY = randomTile.TileY;
            AddBomb(randoBomb);
            AudioPlaybackEngine.Instance.PlaySound(spawnBomb);
        }

        // Uppdaterrar alla aktiva bombers timers för att de ska efplodera efter en satt tid.
        public void UpdateBombTimers(ObservableCollection<BombModel> ActiveBombList)
        {
            foreach (var Bomb in ActiveBombList)
            {
                Bomb.BombTimer++;
            }
        }

        // Aktiverar explodebomb när timern når sin gräns
        public void ExplodeBombsTimeLimit(GameBoard gameBoard, BoosterBoard boosterBoard)
        {
            foreach (var Bomb in ActiveBombList)
            {
                if (Bomb.BombTimer > Bomb.BombTimerLimit)
                {
                   ExplodeBomb(Bomb, gameBoard, boosterBoard);
                }
            }
        }

        // Metoden för bombent explosion, Letar fram vilken Tile bomben ligger på. Kollar sedan hur långt den ska spränga åt varje håll. 
        public void ExplodeBomb(BombModel Bomb, GameBoard GameBoard, BoosterBoard boosterBoard)
        {
            Tile changedTile = new Tile();
            Tile explodedTile = new Tile();
            // metoden kollar bombent x och y värde. Modulis används för att anrunda bombens position till närmsta tal delbart med 50. Detta stämmer då överens med kordinaterna för tiles i GameBoard. 
            int bombXCoordinate = (Bomb.BombX + 2) - ((Bomb.BombX + 2) % 50); 
            int bombYcoordinate = (Bomb.BombY + 2) - ((Bomb.BombY + 2) % 50);

            changedTile = GameBoard.GetTileAtPosition(bombXCoordinate, bombYcoordinate);
            explodedTile = GameBoard.GetTileAtPosition(bombXCoordinate, bombYcoordinate);
            
            // Den tile som bomben ligger på exploderar alltid
            ExplodeTile(explodedTile, GameBoard, boosterBoard);

            //Metodenm kollar hur många rutor uppåt som ska brinna. Detta läses ut från bombens BombExplosionDistance.
            for (int i = 1; i <= Bomb.BombExplosionDistance; i++)                     
            {
                //Hämtar näste tile uppåt. Kollar om den är förstörbar och om den går att spränga förbi. 
                changedTile = GameBoard.GetTileAtPosition(bombXCoordinate, bombYcoordinate - (i * 50));
                if (changedTile.IsDestroyable)
                {
                    //Anropar i så fall metoden för att explodera tile. 
                    ExplodeTile(changedTile, GameBoard,boosterBoard);
                   
                    if (!changedTile.BlowThroug)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            //Upprepas sen för alla riktningar. 
            for (int i = 1; i <= Bomb.BombExplosionDistance; i++)                     //Metodenm kollar hur många rutor nedåt som ska brinna
            {
                changedTile = GameBoard.GetTileAtPosition(bombXCoordinate, bombYcoordinate + (i * 50));
                if (changedTile.IsDestroyable)
                {
                    ExplodeTile(changedTile, GameBoard, boosterBoard);
               
                    if (!changedTile.BlowThroug)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= Bomb.BombExplosionDistance; i++)                     //Metodenm kollar hur många rutor åt höger som ska brinna 
            {
                changedTile = GameBoard.GetTileAtPosition(bombXCoordinate + (i * 50), bombYcoordinate);
                if (changedTile.IsDestroyable)
                {
                    ExplodeTile(changedTile, GameBoard, boosterBoard);


                    if (!changedTile.BlowThroug)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= Bomb.BombExplosionDistance; i++)                     //Metodenm kollar hur många rutor åt vänster som ska brinna
            {
                changedTile = GameBoard.GetTileAtPosition(bombXCoordinate - (i * 50), bombYcoordinate);
                if (changedTile.IsDestroyable)
                {
                    ExplodeTile(changedTile, GameBoard, boosterBoard);

                    if (!changedTile.BlowThroug)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            // spelar upp ljud för explosion
            AudioPlaybackEngine.Instance.PlaySound(smallExplosion);
        }

        //Ändrar den sprängda tilen, så den blir fire och sen övergår till grass
        public void ExplodeTile(Tile destroyedTile, GameBoard gameboard, BoosterBoard boosterBoard)
        {

            //Ifall det finns en booster på vald tile så sprängs den bort
            if (destroyedTile.Booster != Booster.None)
            {
                boosterBoard.RemoveBooster(destroyedTile);
                destroyedTile.Booster = Booster.None;
            }
            //Lägger till booster, antingen coin eller special
            BoosterModel newBooster = boosterBoard.DropBoosterForTile(destroyedTile);
         
            destroyedTile.TileMaterial = TileMaterial.Fire;
            destroyedTile.IsWalkable = true;

            // Delay som gör att tilen först brinner sen byter till grass
            Task.Delay(1000).ContinueWith(_ =>
            {
                // För att kunna lägga till boostern i delay. För att den ska synas efter att elden försvunnit.
               System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                 
                    destroyedTile.TileMaterial = TileMaterial.Grass;
                    destroyedTile.IsWalkable = true;
                    destroyedTile.BlowThroug = true;

                    //Lägger till en booster på tilen ifall den som brann tidigare var dirt
                    if (newBooster != null)
                    {
                        boosterBoard.BoosterCollection.Add(newBooster);
                        destroyedTile.Booster = newBooster.Booster;
                    }
                  
                });
            });

        }

        // Metoden går igenom observableCollection med bomber och kollar hur det ska förflytta sig. 
        public bool UpdateBombPositions(GameBoard gameboard, BoosterBoard boosterBoard, List<BitmapImage> BombImages)                        
        {
            // En kopia av observable collection skapas, den används ifall bomber sprängs. 
            var bombsCopy = ActiveBombList.ToList();                                
            bool resetLevel = false;
            foreach (BombModel BombModel in bombsCopy)
            {
                // variabler skapas för att placka fram den Tile bomben håller på att kliva på. 
                int newxtPixelStepY = 0;                                                
                int nextPixelStepX = 0;
                Tile checkedTile = new Tile();
                // variabler skapas för att hålla koll på hur bomben kommer att röra sig.
                int changeX = 0;
                int changeY = 0;

                //Beroende på Orientation sparad i BombModel. 
                switch (BombModel.BombOrientation)                                  
                {
                    // Om bomben inte ska röra på sig hämtas ändå en Tile, detta för att kolla om bomben stpr på något som brinner. 
                    case Enums.Enums.ObjectOrientation.None:
                        nextPixelStepX = (BombModel.BombX + 2) - ((BombModel.BombX + 2) % 50); 
                        newxtPixelStepY = (BombModel.BombY + 2) - ((BombModel.BombY + 2) % 50);
                        checkedTile = gameboard.GetTileAtPosition(nextPixelStepX, newxtPixelStepY);

                        // Om bomben står på en Tile som brinner sätts timer till 151, detta gör att bomben kommer sprängas nästa game tick. 
                        if (checkedTile.TileMaterial is Enums.Enums.TileMaterial.Fire)
                        {
                            BombModel.BombTimer = 151;
                        }
                        break;

                    // Utifrån bombens riktning letar metoden upp vilken Tile bomben kommer att kliva på.
                    case Enums.Enums.ObjectOrientation.Up:

                        // Modulus används för att få reda på vilken Tile Bomben befinner sig på.
                            nextPixelStepX = (BombModel.BombX + 2) - ((BombModel.BombX + 2) % 50);   
                            newxtPixelStepY = (BombModel.BombY - 2) - ((BombModel.BombY - 2) % 50);
                        //Beroende på Orientation ändras X och Y värdena. Användas för att plocka fram den tile bomben håller på att kliva på. 
                            changeX = 0; 
                            changeY = -(BombModel.BombSpeed + 1);
                            
                        break;
                    case Enums.Enums.ObjectOrientation.Down:
                        
                            nextPixelStepX = (BombModel.BombX + 2) - ((BombModel.BombX + 2) % 50); 
                            newxtPixelStepY = (BombModel.BombY + BombModel.BombHeight + 2) - ((BombModel.BombY + BombModel.BombHeight + 2) % 50);
                            changeX = 0;
                            changeY = (BombModel.BombSpeed + 1);
                        
                        break;
                    case Enums.Enums.ObjectOrientation.Left:

                        nextPixelStepX = (BombModel.BombX - 2) - ((BombModel.BombX - 2) % 50);
                        newxtPixelStepY = (BombModel.BombY + 2) - ((BombModel.BombY + 2) % 50);
                        changeX = -(BombModel.BombSpeed + 1);
                        changeY = 0;
                        
                        break;
                    case Enums.Enums.ObjectOrientation.Right:
                        
                            nextPixelStepX = (BombModel.BombX + BombModel.BombWidth + 2) - ((BombModel.BombX + BombModel.BombWidth + 2) % 50);  
                            newxtPixelStepY = (BombModel.BombY + 2) - ((BombModel.BombY + 2) % 50);
                        changeX = (BombModel.BombSpeed + 1);
                        changeY = 0;

                        break;
                    default:
                        break;    
                }
                // En tile hämtas från de uppdaterade värdena. Detta är alltsåp den tile bomben kommer att kliva på. 
                checkedTile = gameboard.GetTileAtPosition(nextPixelStepX, newxtPixelStepY);

                // Om Tile är wakable uppdateras bombens position och en kontroll görs om den har klivit på en tile som brinner. 
                if (checkedTile.IsWalkable)                                      
                {
                    BombModel.BombY += changeY;
                    BombModel.BombX += changeX;
                    if(checkedTile.TileMaterial is Enums.Enums.TileMaterial.Fire)
                    {
                        BombModel.BombTimer = 151; 
                    }
                }
                // Om Tile inte är wakable, stannar bomben.
                else
                {
                    BombModel.BombOrientation = Enums.Enums.ObjectOrientation.None;
                }
                BombModel.BombTimer++;                                                    
                
                AnimateBomb(BombModel, BombImages);

                // En kontroll görs för att se om bombens timer har gått ut, i så fall exploderar bomben och tas bort från listan. 
                if (BombModel.BombTimer > 150)
                {
                    ExplodeBomb(BombModel, gameboard, boosterBoard);
                    RemoveBomb(BombModel);

                    //När en bomb exploderar kontrolleras det om det finns några exploderbara tiles kvar. Om det inte gör det startar banan om. 
                    if (!gameboard.CheckIfAnyDirtTilesLeft())
                    {
                        resetLevel = true;
                    }
                }

            }
            return resetLevel; 
        }

        private void AnimateBomb(BombModel Bomb, List<BitmapImage> BombImages)
        {
            Bomb.Framerate += 1; // Öka för att sakta ner animationen
            if (Bomb.Framerate == 4) // Uppdatera animation var fjärde gång
            {
                Bomb.Steps++; // Gå till nästa steg i animationen
                Bomb.Framerate = 0; // Återställ räkningen
            }
            _steps++; // Gå till nästa steg i animationen
            if (Bomb.Steps > 3 || Bomb.Steps < 1) // Om vi gått utanför gränserna för animationen, återställ
            {
                Bomb.Steps = 1;
            }

            Bomb.BombImages = BombImages[Bomb.Steps];
        }

        public void AddBomb(BombModel newBomb)
        {
            ActiveBombList.Add(newBomb);
            return;
        }

        public void RemoveBomb(BombModel bombToRemove)
        {
            if (ActiveBombList.Contains(bombToRemove))
            {
                ActiveBombList.Remove(bombToRemove);
            }
        }   
    }
}