using BomberMan.Models;
using System.Windows.Input;
using System.Windows.Media.Imaging;  // Behövs för BitmapImage
using PropertyChanged;
using System.Windows.Threading;
using BomberMan.Services;
using BomberMan.Models.Gameboard;
using BomberMan.Commands;
using BomberMan.Models.Items.Bombs;
using System.Collections.ObjectModel;
using static BomberMan.Enums.Enums;
using BomberMan.Models.BombBoard;
using System.Text.Json;
using System.IO;
using BomberMan.Models.BoostersBoard;
using System.Windows.Controls;


namespace BomberMan.ViewModels
{
    [AddINotifyPropertyChangedInterface] // Fody auto implementerar INotifyPropertyChanged

    public class MainViewModel
    {

        public int CountdownValue { get; set; }
        public bool IsCountdownVisible { get; set; }
        public DispatcherTimer _gameTimer; //Timer för att uppdatera spelet
        public PlayerModel Player { get; set; } = new PlayerModel(); // Spelarmodell
        public BombModel Bomb { get; set; } // BombModell
        public BombBoard ActiveBombs { get; set; } = new BombBoard();
        public List<BitmapImage> PlayerMovements { get; set; } // Lista som innehåller spelarens animationer
        public List<BitmapImage> BombImages { get; set; } // Lista som innehåller bombens animationer
        public GameBoard GameBoard { get; set; } = new GameBoard(1); // Skapar gameboard som håller koll på spelplanen. 
        public BoosterBoard BoosterBoard { get; set; } = new BoosterBoard();
        //Håller bilder att visa
        public ObservableCollection<BitmapImage> ScoreImages { get; set; }

        public bool VisibilitycontrollGameOverView { get; set; }
        public bool VisibilitycontrollGameOverLabel { get; set; } = false;
        public bool VisibilitycontrollRestartGame { get; set; } = false;

        public bool GoLeft { get; set; } // Gå åt vänster
        public bool GoRight { get; set; } // Gå åt höger
        public bool GoUp { get; set; } // Gå uppåt
        public bool GoDown { get; set; } // Gå nedåt
        public bool SpaceKey { get; set; } // Lägg eller putta bomb
        public bool ReadyForNextAction { get; set; } = true;  // Håller koll på om spelaren har gjort klart sin aktivitet och är redo för nästa. 
        public int PlayerSpeed { get; set; } = 1; // Standard hastighet för spelaren
        public int AnimateTimer { get; set; } = 0; // gör så att animationen bara spelar efter visst många flyttade pixlar
        public int Score { get; set; } = 0;
        public int BombSpawnTimer { get; set; } = 0;
        public int BombSpawnerInterval { get; set; } = 500;
        public int BombSpawnerCounter { get; set; } = 0;
        public bool resetLevel { get; set; } = false;
        public int GameLevel { get; set; } = 1;
        public Tile DeathTile { get; set; } = new Tile();
        private int _steps = 0; // Håller koll på vilken animationsbild som ska visas
        private int _slowDownFrameRate = 0; // Kontrollerar hastigheten för animationen
        string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        string fullPath;
        public string PlayerNameFromTextBox { get; set; }

        // Ljudfiler som kommer användas under rundan.
        CachedSound smallExplosion = new CachedSound("Assets/Audio/Effects/explosion_01.wav");
        CachedSound point = new CachedSound("Assets/Audio/Effects/point_01.wav");
        CachedSound spawnBomb = new CachedSound("Assets/Audio/Effects/spawn_01.wav");
        CachedSound kick = new CachedSound("Assets/Audio/Effects/spawn_02.wav");
        CachedSound newLevel = new CachedSound("Assets/Audio/Effects/level_01.wav");
        CachedSound booster = new CachedSound("Assets/Audio/Effects/add.wav");
        CachedSound death = new CachedSound("Assets/Audio/Effects/fail_02.wav");
        CachedSound cheer = new CachedSound("Assets/Audio/Effects/clapping.wav");
        CachedSound beep1 = new CachedSound("Assets/Audio/Effects/start_01.wav");
        CachedSound beep2 = new CachedSound("Assets/Audio/Effects/start_02.wav");



        //public MainMenuViewModel mainMenuViewModel { get; set; }
        public ImageLoaderService _imageLoaderService { get; set; }
        //------------------Hanterar allt med score och hur det ska visas i UI------------------------->
        public string LabelHighScore { get; set; }
        public ScoreModel ScoreModel { get; set; }
        public HighScoreModel HighScoreModel { get; set; }
        public ObservableCollection<HighScoreModel> PlayerHighScores { get; set; } = new ObservableCollection<HighScoreModel>();

        public ICommand GetPlayerName { get; }
        public ICommand KeyDownCommand { get; } // Kommando för när en tangent trycks ned
        public ICommand KeyUpCommand { get; } // Kommando för när en tangent släpps
        public ICommand NavigateToGameOverCommand { get; set; }
        public ICommand RestartGame { get; set; }
        public ICommand RestartButtonCommand { get; set; }
        // Kommando för att uppdatera poäng
        public ICommand AddHighScoreCommand { get; }

        private void UpdateScoreImages(int points)
        {
            // Rensa gamla bilder
            ScoreImages.Clear();

            // Konvertera poängen till en sträng för att hantera varje siffra
            string scoreString = ScoreModel.AddPoints(points).ToString();

            // Gå igenom varje siffra i poängen
            foreach (char digit in scoreString)
            {
                // Skapa sökvägen till motsvarande bild för varje siffra
                string imagePath = $"/Assets/Images/Numbers/num_{digit}.png"; // T.ex. num_1.png, num_2.png

                BitmapImage numberImage = new BitmapImage();
                numberImage.BeginInit();
                numberImage.UriSource = new Uri(imagePath, UriKind.Relative);
                numberImage.EndInit();

                // Lägg till bilden till listan
                ScoreImages.Add(numberImage);
                
                //ScoreImages.Add(new BitmapImage(new Uri(imagePath, UriKind.Relative)));
            }
        }


        // Ladda highscores från JSON-fil
        public void LoadPlayerHighScoresFromJson()
        {
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(exePath).FullName).FullName).FullName).FullName;

            fullPath = Path.Combine(projectRootPath, "Json", "Highscore.json");

            if (File.Exists(fullPath))
            {
                // Läs filen och deserialisera till en lista av ScoreModel
                string json = File.ReadAllText(fullPath);
                List<HighScoreModel> highScoreFromJson = JsonSerializer.Deserialize<List<HighScoreModel>>(json);


                // Rensa den befintliga ObservableCollection
                PlayerHighScores.Clear();

                // Lägg till varje ScoreModel i ObservableCollection
                foreach (var highScore in highScoreFromJson)
                {
                    PlayerHighScores.Add(highScore);
                }
            }
            else
            {
                //om Json äe tom så skciar  den ut det här i listan så vet man iaf att binden fungerar
                HighScoreModel noValueLoaded = new HighScoreModel { PlayerName = "No data", PlayerHighScore = 0 };
                PlayerHighScores.Add(noValueLoaded);
            }
        }





        //Sätter vären på ett Highscore-objekt och lägger till det i highscorelistan samt tar bort det lägsta objektet i listan
        public void AddNewHighscore()
        {
            int currentPlayerScore = ScoreModel.CurrentPlayerScore;
            ObservableCollection<HighScoreModel> playerHighScores = PlayerHighScores;
            HighScoreModel newHighScore = new HighScoreModel();

            newHighScore.PlayerHighScore = currentPlayerScore;
            newHighScore.PlayerName = PlayerNameFromTextBox;
            PlayerHighScores.RemoveAt(4);
            PlayerHighScores.Add(newHighScore);
            SortHighScores();
            VisibilitycontrollGameOverView = false;
            VisibilitycontrollRestartGame = true;
            AudioPlaybackEngine.Instance.PlaySound(point);

            // Serialize the updated list to JSON
            string updatedJson = JsonSerializer.Serialize(PlayerHighScores, new JsonSerializerOptions { WriteIndented = true });

            // Write the updated JSON back to the file
            File.WriteAllText(fullPath, updatedJson);

        }


        public void SortHighScores()
        {
            // gör om skiten till en lista och sorterar den efter playerhighscore
            var sortedList = PlayerHighScores.OrderByDescending(x => x.PlayerHighScore).ToList();

            // Rensa den befintliga ObservableCollection
            PlayerHighScores.Clear();

            // Lägg till de sorterade objekten tillbaka till ObservableCollection
            foreach (var highScore in sortedList)
            {
                PlayerHighScores.Add(highScore);
            }

        }


        public MainViewModel()
        {
            // Laddar in bilderna som ska användas
            _imageLoaderService = new ImageLoaderService();

            // Initiera ScoreModel
            ScoreModel = new ScoreModel();

            ScoreImages = new ObservableCollection<BitmapImage>();
            UpdateScoreImages(0);  // Uppdatera bilderna för det initiala värdet

            // Initiera kommandot för att lägga till poäng
            AddHighScoreCommand = new RelayCommand(AddNewHighscore);

            LoadPlayerHighScoresFromJson();

            KeyDownCommand = new RelayCommandGeneric<Key>(OnKeyDown);
            KeyUpCommand = new RelayCommandGeneric<Key>(OnKeyUp);

            RestartButtonCommand = new RelayCommand(RestartGameFromButton);

            // Starta spelets timer
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(5); // Uppdateringsintervall för timer
            _gameTimer.Tick += OnGameTimerTick; // Vad som händer när timern tickar

            SetUp(); // Metod för att ladda in spelarens bilder för animation

        }

        //Konstruktor för xUnitTest
        public
            MainViewModel(bool istest)
        {
            // Initiera ScoreModel
            ScoreModel = new ScoreModel();

            // Initiera kommandot för att lägga till poäng
            AddHighScoreCommand = new RelayCommand(AddNewHighscore);

            LoadPlayerHighScoresFromJson();

            KeyDownCommand = new RelayCommandGeneric<Key>(OnKeyDown);
            KeyUpCommand = new RelayCommandGeneric<Key>(OnKeyUp);

            RestartButtonCommand = new RelayCommand(RestartGameFromButton);

            // Starta spelets timer
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromMilliseconds(5); // Uppdateringsintervall för timer
            _gameTimer.Tick += OnGameTimerTick; // Vad som händer när timern tickar

        }

        // visar en nedräkning i 3 sekunder när den körs
        public async void StartCountdown()
        {
            IsCountdownVisible = true; // Visa countdown Overlay 

            CountdownValue = 3;
            AudioPlaybackEngine.Instance.PlaySound(beep1);
            while (CountdownValue > 0)
            {
                _gameTimer.Stop();
                await Task.Delay(1000); // Vänta en sek
                CountdownValue--;
                AudioPlaybackEngine.Instance.PlaySound(beep1);
            }
            AudioPlaybackEngine.Instance.PlaySound(beep2);
            _gameTimer.Start();

            IsCountdownVisible = false; // Göm overlay
        }


        // Hanterar det som ska köras när spelet när spelrundan är slut
        private void GameOver()
        {
            // Stannar spelet, spelar upp ett ljud, sätter rundans poäng till slutlig poäng. 
            _gameTimer.Stop();
            AudioPlaybackEngine.Instance.PlaySound(death);
            int currentPlayerScore = ScoreModel.CurrentPlayerScore;
            ObservableCollection<HighScoreModel> playerHighScores = PlayerHighScores;

            // Kollar vilken setting på GAMEOVERVIEW som spelaren ska få se när den dör.
            foreach (var highScore in playerHighScores)
            {
                // Kollar om spelaren tar sig in i highscore eller inte. Navigerar till GameOverView och ändrar visibility på olika komponenter. 
                if (currentPlayerScore > highScore.PlayerHighScore && NavigateToGameOverCommand != null && NavigateToGameOverCommand.CanExecute(null))
                {
                    LabelHighScore = $"GRATZ, YOUR HIGHSCORE IS {currentPlayerScore}!";
                    AudioPlaybackEngine.Instance.PlaySound(cheer);
                    VisibilitycontrollGameOverLabel = true;
                    VisibilitycontrollGameOverView = true;
                    NavigateToGameOverCommand.Execute(null);
                }
                else if (NavigateToGameOverCommand != null && NavigateToGameOverCommand.CanExecute(null))
                {
                    LabelHighScore = $"YOUR SCORE IS {currentPlayerScore}!";
                    VisibilitycontrollRestartGame = true;
                    NavigateToGameOverCommand.Execute(null);
                    VisibilitycontrollGameOverView = false;
                }
            }
        }

        // Startar om speler, buden till restart knappen i gameOverView
        public void RestartGameFromButton()
        {
            RestartGame.Execute(null);
        }

        //Hanterar det som ska hända varje game tick. 
        private void OnGameTimerTick(object sender, EventArgs e)
        {
            // Kollar hur långt spelaren ska förflytta sig. Kopplat till splayer speed. 
            for (int i = 0; i < PlayerSpeed; i++)
            {
                UpdatePlayerPosition(GameBoard, BoosterBoard);
            }
            //Kollar om spelaren ska utföra något, typ lägga en bomb. 
            UpdatePlayerAction();
            //Sätter Bool restartLevel till utfallet av kontrollen i updaterBombPositions. 
            resetLevel = ActiveBombs.UpdateBombPositions(GameBoard, BoosterBoard, BombImages);
            //Lägger ut random bomber om timern tillåter 
            SpawnBombAtInterval();

            //Kollar om spelaren står på en farlig Tile, går i så fall till GameOver. 
            DeathTile = GameBoard.GetTileAtPosition((Player.PlayerX + 2) - ((Player.PlayerX + 2) % 50), (Player.PlayerY + 2) - (((Player.PlayerY + 2) % 50))); // detta ska brytas ut till en egen metod
            if (DeathTile.TileMaterial is TileMaterial.Fire)
            {
                Player.PlayerImage = _imageLoaderService.LoadImage("Assets/Images/player/dead2.png");
                GameOver();
            }
            if (resetLevel)
            {
                RestartLevel();
            }
        }

        // Startar om spelbrädet utan att starta om spelaren. Detta händer när spelaren tar sig till nästa nivå. 
        private void RestartLevel()
        {
            GameLevel++;
            Player.PlayerX = 50;
            Player.PlayerY = 50;
            GameBoard = new GameBoard(GameLevel);
            ActiveBombs = new BombBoard();
            BoosterBoard = new BoosterBoard();
            AudioPlaybackEngine.Instance.PlaySound(newLevel);

        }

        //Kollar om spelaren ska utföra något, just nu kan är det bara lägga eller sparka bomb som kan utföras. 
        private void UpdatePlayerAction()
        {
            if (SpaceKey)
            {
                if (!KickIfBombInTheWay())
                {
                    if (CheckIfCanSpawnBomb())
                    {
                        SpawnBomb(Player.PlayerOrientation, Player.PlayerX, Player.PlayerY);
                    }
                }
                SpaceKey = false;
            }
        }

        // Metoden kontrollerar om det ligger en bomb på en tile närliggande spelaren. 
        private bool KickIfBombInTheWay()
        {
            int tileCoordinateX = (Player.PlayerX + 2) - ((Player.PlayerX + 2) % 50);
            int tileCoordinateY = (Player.PlayerY + 2) - ((Player.PlayerY + 2) % 50);
            int bombCornerCoordinateX;
            int bombCornerCoordinateY;

            // Metoden kollar vilken Tile spelaren står på. Den väljer sedan en närliggande Tile beroende på hur spelaren är vänd
            switch (Player.PlayerOrientation)
            {
                case Enums.Enums.ObjectOrientation.Up:
                    tileCoordinateY -= 50;
                    break;
                case Enums.Enums.ObjectOrientation.Down:
                    tileCoordinateY += 50;
                    break;
                case Enums.Enums.ObjectOrientation.Left:
                    tileCoordinateX -= 50;
                    break;
                case Enums.Enums.ObjectOrientation.Right:
                    tileCoordinateX += 50;
                    break;
            }

            // När metoden har hämtat en Tile måste den kontrollera om det befinner sig någon bomb på den. Den gör detta genom att kolla om någon bomb har ett hörn som befinner sig inom aren av Tile. 
            foreach (BombModel Bomb in ActiveBombs.ActiveBombList)
            {
                bombCornerCoordinateX = Bomb.BombX;        // kollar bombens vänstra övre hörn. 
                bombCornerCoordinateY = Bomb.BombY;
                if ((tileCoordinateX == bombCornerCoordinateX - ((bombCornerCoordinateX) % 50)) && (tileCoordinateY == bombCornerCoordinateY - ((bombCornerCoordinateY) % 50)))
                {
                    Bomb.BombOrientation = Player.PlayerOrientation;
                    AudioPlaybackEngine.Instance.PlaySound(kick);
                    return true;
                }

                bombCornerCoordinateX = Bomb.BombX;        // kollar bombens vänstra nedre. 
                bombCornerCoordinateY = Bomb.BombY + 48;
                if ((tileCoordinateX == bombCornerCoordinateX - ((bombCornerCoordinateX) % 50)) && (tileCoordinateY == bombCornerCoordinateY - ((bombCornerCoordinateY) % 50)))
                {
                    Bomb.BombOrientation = Player.PlayerOrientation;
                    AudioPlaybackEngine.Instance.PlaySound(kick);
                    return true;
                }

                bombCornerCoordinateX = Bomb.BombX + 48;        // kollar bombens högra övre hörn. 
                bombCornerCoordinateY = Bomb.BombY;
                if ((tileCoordinateX == bombCornerCoordinateX - ((bombCornerCoordinateX) % 50)) && (tileCoordinateY == bombCornerCoordinateY - ((bombCornerCoordinateY) % 50)))
                {
                    Bomb.BombOrientation = Player.PlayerOrientation;
                    AudioPlaybackEngine.Instance.PlaySound(kick);
                    return true;
                }

                bombCornerCoordinateX = Bomb.BombX + 48;         // kollar bombens högra nedre hörn. 
                bombCornerCoordinateY = Bomb.BombY + 48;
                if ((tileCoordinateX == bombCornerCoordinateX - ((bombCornerCoordinateX) % 50)) && (tileCoordinateY == bombCornerCoordinateY - ((bombCornerCoordinateY) % 50)))
                {
                    Bomb.BombOrientation = Player.PlayerOrientation;
                    AudioPlaybackEngine.Instance.PlaySound(kick);
                    return true;
                }
            }

            return false;               // Om inget av hörnen på någon av bomberna befinner sig över Tile returneras false. 
        }

        // Metoden kontrollerar om spelaren kan lägga en bomb, dvs som den Tile bilben ska läggas på är wakable 
        private bool CheckIfCanSpawnBomb()
        {
            //Metoden kollar vilken Tile spelaren står på
            int tileCoordinateX = (Player.PlayerX + 2) - ((Player.PlayerX + 2) % 50);
            int tileCoordinateY = (Player.PlayerY + 2) - ((Player.PlayerY + 2) % 50);
            Tile tileToBeChecked = new Tile();

            // Beroende på spelarens riktning välja en närliggande tile
            switch (Player.PlayerOrientation)
            {
                case Enums.Enums.ObjectOrientation.Up:
                    tileCoordinateY -= 50;
                    tileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);
                    break;
                case Enums.Enums.ObjectOrientation.Down:
                    tileCoordinateY += 50;
                    tileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);
                    break;
                case Enums.Enums.ObjectOrientation.Left:
                    tileCoordinateX -= 50;
                    tileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);
                    break;
                case Enums.Enums.ObjectOrientation.Right:
                    tileCoordinateX += 50;
                    tileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);
                    break;
            }
            // Tile kontrolleras sedan oim den är wakable, dvs om man kan lägga en bomb på den
            if (tileToBeChecked.IsWalkable)
            {
                return true;
            }
            return false;
        }

        // Ladda spelarens bilder för animation
        private void SetUp()
        {
            // Ladda 16 bilder från angiven filväg
            PlayerMovements = Enumerable.Range(1, 16)
                .Select(i => _imageLoaderService.LoadImage($"Assets/Images/player/character_{i:D2}.png"))
                .ToList();
            Player.PlayerImage = PlayerMovements[0]; // Sätt första bilden som spelarens initialbild

            BombImages = Enumerable.Range(1, 4)
                .Select(i => _imageLoaderService.LoadImage($"Assets/Images/Bombs/RegularBomb/bomb2_{i:D2}.png"))
                .ToList();
        }

        //Lägger ut en bomb baserat på spelarens position och riktning.
        private void SpawnBomb(Enum orientation, int x, int y)
        {
            Bomb = new BombModel();
            switch (orientation)
            {
                case Enums.Enums.ObjectOrientation.Up:
                    Bomb.BombX = x;
                    Bomb.BombY = y - 50;
                    break;
                case Enums.Enums.ObjectOrientation.Down:
                    Bomb.BombX = x;
                    Bomb.BombY = y + 50;
                    break;
                case Enums.Enums.ObjectOrientation.Left:
                    Bomb.BombX = x - 50;
                    Bomb.BombY = y;
                    break;
                case Enums.Enums.ObjectOrientation.Right:
                    Bomb.BombX = x + 50;
                    Bomb.BombY = y;
                    break;
            }

            // Sätter första bilden som bombens initialbild
            Bomb.BombImages = BombImages[0];
            ActiveBombs.AddBomb(Bomb);
            AudioPlaybackEngine.Instance.PlaySound(spawnBomb);
            return;
        }

        // Styr hur ofta spelet lägger ut random bomber. Körs varje game tick
        private void SpawnBombAtInterval()
        {
            if (BombSpawnTimer < BombSpawnerInterval)
            {
                BombSpawnTimer++;
            }
            else
            {
                switch (BombSpawnerCounter)
                {
                    case 4:
                        BombSpawnerInterval = 300;
                        break;

                    case 10:
                        BombSpawnerInterval = 100;
                        break;

                    case 20:
                        BombSpawnerInterval = 50;
                        break;
                    case 40:
                        BombSpawnerInterval = 25;
                        break;
                    case 100:
                        BombSpawnerInterval = 15;
                        break;
                }
                BombSpawnerCounter++;
                BombSpawnTimer = 0;
                ActiveBombs.SpawnBombRandomly(GameBoard);
            }
        }

        // Hantera tangenttryckningar (keydown)
        public void OnKeyDown(Key key)
        {
            if (ReadyForNextAction)
            {
                switch (key)
                {
                    case Key.Left:
                        GoLeft = true; // Om vänsterpil trycks ned, sätt GoLeft till true
                        ReadyForNextAction = false;
                        break;
                    case Key.Right:
                        GoRight = true; // Om högerpil trycks ned, sätt GoRight till true
                        ReadyForNextAction = false;
                        break;
                    case Key.Up:
                        GoUp = true; // Om uppåtpil trycks ned, sätt GoUp till true
                        ReadyForNextAction = false;
                        break;
                    case Key.Down:
                        GoDown = true; // Om nedåtpil trycks ned, sätt GoDown till true
                        ReadyForNextAction = false;
                        break;
                    case Key.Space:
                        SpaceKey = true; // Om Space trycks ned, sätt SpaceKey till true
                        break;
                }
            }
        }

        // Hantera när en tangent släpps (keyup)
        public void OnKeyUp(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    //GoLeft = false; // När vänsterpil släpps, sätt GoLeft till false
                    break;
                case Key.Right:
                    //GoRight = false; // När högerpil släpps, sätt GoRight till false
                    break;
                case Key.Up:
                    //GoUp = false; // När uppåtpil släpps, sätt GoUp till false
                    break;
                case Key.Down:
                    //GoDown = false; // När nedåtpil släpps, sätt GoDown till false
                    break;
                case Key.Space:
                    SpaceKey = false; // När Space släpps, sätt SpaceKey till false
                    break;
            }
        }

        // Metoden uppdaterar spelaren position utifrån vilken tangent som hålls nere och volken Tile spelaren står på.
        public void UpdatePlayerPosition(GameBoard GameBoard, BoosterBoard boosterBoard)
        {
            int tileCoordinateX = 0;      // Koordinaterna som används för att ploca fram den Tile spelaren kommer att kliva på. 
            int tileCoordinateY = 0;
            if (GoLeft)                                     // Metoden kollar vilken knapp som är nertryckt. Detta värde ligger sparat i spelaren och kommer från MainView
            {
                if (Player.PlayerOrientation is Enums.Enums.ObjectOrientation.Left)                             // Metoden kontrollerar om spelaren är vänd åt det hållet den är på väg att gå åt. Annars kommer spelaren att vända sig. Dett agör att man kan klicka snabbt på knappen och vända sig utan att spelaren börjar gå. 
                {
                    tileCoordinateX = (Player.PlayerX - 2) - ((Player.PlayerX - 2) % 50);                       // Metoden kontrollerar vilken koordinat spelaren kommer att kliva på. 
                    tileCoordinateY = (Player.PlayerY + 2) - ((Player.PlayerY + 2) % 50);
                    Tile TileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);       // metoden kollar sedan vilken tile det kommer att vara. 

                    tileCoordinateX = (Player.PlayerX - 2) - ((Player.PlayerX - 2) % 50);                       // metoden gör sedan en likadan kontroll för spelarens nedersta punkt. Detta för att spelaren inte ska kunna gå vidare om huvudet eller fötterna tar i en Tile som inte går att gå på. 
                    tileCoordinateY = (Player.PlayerY + Player.PlayerHeight - 2) - ((Player.PlayerY + Player.PlayerHeight - 2) % 50);
                    Tile TileToBeChecked2 = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);
                    if (TileToBeChecked.IsWalkable && TileToBeChecked2.IsWalkable)                              // om båda punkter är godkända får spelaren gå fram två pixlar 
                    {
                        Player.PlayerX -= 2;
                        if (TileToBeChecked.Booster != Booster.None)
                        {
                            CheckIfStepOnBooster(TileToBeChecked, boosterBoard);
                      
                        }
                    }
                    else                                                                                        // Om spelaren går in i en vägg stannar den och inväntar nästa instruktion.
                    {
                        GoLeft = false;
                        ReadyForNextAction = true;
                    }
                    if (Player.PlayerX % 50 == 0)                                                               // om spelaren går fram till nästa hela Tile stannar den och inväntar nästa instruktion. 
                    {
                        ReadyForNextAction = true;
                        GoLeft = false;
                    }
                    AnimateTimer++;

                    if (AnimateTimer > 5)
                    {
                        AnimatePlayer(4, 7); // Kör animation för vänsterrörelse
                        AnimateTimer = 0;
                    }
                }
                else                                                                                            // om spelaren inte redan är vänd på det hållet som den klickade pilen vänds spelaren och inväntar nästa instruktion.
                {
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Left;
                    ReadyForNextAction = true;
                    GoLeft = false;
                    AnimatePlayer(4, 4);
                }
            }
            else if (GoRight)                                                                                   // Samma funktion finna för de höger, upp och ner. 
            {
                if (Player.PlayerOrientation is Enums.Enums.ObjectOrientation.Right)
                {

                    tileCoordinateX = (Player.PlayerX + Player.PlayerWidth + 2) - ((Player.PlayerX + Player.PlayerWidth + 2) % 50);
                    tileCoordinateY = (Player.PlayerY + 2) - ((Player.PlayerY + 2) % 50);
                    Tile TileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);

                    tileCoordinateX = (Player.PlayerX + Player.PlayerWidth + 2) - ((Player.PlayerX + Player.PlayerWidth + 2) % 50);
                    tileCoordinateY = (Player.PlayerY + Player.PlayerHeight - 2) - ((Player.PlayerY + Player.PlayerHeight - 2) % 50);
                    Tile TileToBeChecked2 = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);

                    if (TileToBeChecked.IsWalkable && TileToBeChecked2.IsWalkable)
                    {

                        Player.PlayerX += 2;
                        if (TileToBeChecked.Booster != Booster.None)
                        {
                            CheckIfStepOnBooster(TileToBeChecked, boosterBoard);
                        
                        }
                    }
                    else
                    {
                        GoRight = false;
                        ReadyForNextAction = true;
                    }

                    if (Player.PlayerX % 50 == 0)
                    {
                        ReadyForNextAction = true;
                        GoRight = false;
                    }
                    AnimateTimer++;

                    if (AnimateTimer > 5)
                    {
                        AnimatePlayer(8, 11);
                        AnimateTimer = 0;
                    }
                }
                else
                {
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Right;
                    ReadyForNextAction = true;
                    GoRight = false;
                    AnimatePlayer(8, 8);
                }
            }
            else if (GoUp) // Flytta uppåt om GoUp är true och spelaren inte är vid övre kanten
            {
                if (Player.PlayerOrientation is Enums.Enums.ObjectOrientation.Up)
                {
                    tileCoordinateX = (Player.PlayerX + 2) - ((Player.PlayerX + 2) % 50);
                    tileCoordinateY = (Player.PlayerY - 2) - ((Player.PlayerY - 2) % 50);
                    Tile TileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);

                    tileCoordinateX = (Player.PlayerX + Player.PlayerWidth - 2) - ((Player.PlayerX + Player.PlayerWidth - 2) % 50);
                    tileCoordinateY = (Player.PlayerY - 2) - ((Player.PlayerY - 2) % 50);
                    Tile TileToBeChecked2 = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);

                    if (TileToBeChecked.IsWalkable && TileToBeChecked2.IsWalkable)
                    {
                        Player.PlayerY -= 2;

                        if (TileToBeChecked.Booster != Booster.None)
                        {
                            CheckIfStepOnBooster(TileToBeChecked, boosterBoard);
                           
                        }
                    }
                    else
                    {
                        GoUp = false;
                        ReadyForNextAction = true;
                    }

                    if (Player.PlayerY % 50 == 0)
                    {
                        ReadyForNextAction = true;
                        GoUp = false;
                    }
                    AnimateTimer++;
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Up;
                    if (AnimateTimer > 5)
                    {
                        AnimatePlayer(12, 15);
                        AnimateTimer = 0;
                    }
                }
                else
                {
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Up;
                    ReadyForNextAction = true;
                    GoUp = false;
                    AnimatePlayer(12, 12);
                }
            }
            else if (GoDown)
            {
                if (Player.PlayerOrientation is Enums.Enums.ObjectOrientation.Down)
                {
                    tileCoordinateX = (Player.PlayerX + 2) - ((Player.PlayerX + 2) % 50);
                    tileCoordinateY = (Player.PlayerY + Player.PlayerHeight + 2) - ((Player.PlayerY + Player.PlayerHeight + 2) % 50);
                    Tile TileToBeChecked = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);


                    tileCoordinateX = (Player.PlayerX + Player.PlayerWidth - 2) - ((Player.PlayerX + Player.PlayerWidth - 2) % 50);
                    tileCoordinateY = (Player.PlayerY + Player.PlayerHeight + 2) - ((Player.PlayerY + Player.PlayerHeight + 2) % 50);
                    Tile TileToBeChecked2 = GameBoard.GetTileAtPosition(tileCoordinateX, tileCoordinateY);

                    if (TileToBeChecked.IsWalkable && TileToBeChecked2.IsWalkable)
                    {
                        Player.PlayerY += 2;

                        if (TileToBeChecked.Booster != Booster.None)
                        {
                            CheckIfStepOnBooster(TileToBeChecked, boosterBoard);
                    
                        }
                    }
                    else
                    {
                        GoDown = false;
                        ReadyForNextAction = true;
                    }
                    if (Player.PlayerY % 50 == 0)
                    {
                        ReadyForNextAction = true;
                        GoDown = false;
                    }
                    AnimateTimer++;
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Down;
                    if (AnimateTimer > 5)
                    {
                        AnimatePlayer(0, 3); // Kör animation för nedåtrörelse
                        AnimateTimer = 0;
                    }
                }
                else
                {
                    Player.PlayerOrientation = Enums.Enums.ObjectOrientation.Down;
                    ReadyForNextAction = true;
                    GoDown = false;
                    AnimatePlayer(0, 0);
                }
            }
        }


        // Sköter spelarens animation beroende på rörelseriktning
        private void AnimatePlayer(int start, int end)
        {
            _slowDownFrameRate += 1; // Öka för att sakta ner animationen
            if (_slowDownFrameRate == 4) // Uppdatera animation var fjärde gång
            {
                _steps++; // Gå till nästa steg i animationen
                _slowDownFrameRate = 0; // Återställ räkningen
            }
            _steps++; // Gå till nästa steg i animationen
            if (_steps > end || _steps < start) // Om vi gått utanför gränserna för animationen, återställ
            {
                _steps = start;
            }
            Player.PlayerImage = PlayerMovements[_steps]; // Sätt spelarens bild till rätt animationssteg
        }

        //Kontrollerar vad för typ av booster som spelaren kliver på och ger effekt utifrån det
        private void CheckIfStepOnBooster(Tile TileToBeChecked, BoosterBoard boosterBoard)
        {
            int value = 0;
            switch (TileToBeChecked.Booster)
            {
                case Booster.Coin:

                    value = boosterBoard.PickUpBooster(TileToBeChecked, value);
                    UpdateScoreImages(value);
                    AudioPlaybackEngine.Instance.PlaySound(point);
                    break;
                case Booster.Speed:
                    if (PlayerSpeed <= 5)
                    {
                        value = boosterBoard.PickUpBooster(TileToBeChecked, value);
                        PlayerSpeed += value;

                        DispatcherTimer timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(15);
                        timer.Tick += (sender, args) =>
                        {
                            PlayerSpeed -= 1; // Återställ hastigheten
                            timer.Stop(); // Stoppa timern efter att effekten är över
                        };
                        timer.Start();
                    }
                    value = boosterBoard.PickUpBooster(TileToBeChecked, value);
                    PlayerSpeed += value;
                    AudioPlaybackEngine.Instance.PlaySound(booster);
                    break;
                case Booster.Diamond:
                    value = boosterBoard.PickUpBooster(TileToBeChecked, value);
                    UpdateScoreImages(value);
                    AudioPlaybackEngine.Instance.PlaySound(point);
                    break;
            }
        }
        // Oj Erik! Läste du hela vägen ner hit? Det var imponerande! <3 
    }
}
