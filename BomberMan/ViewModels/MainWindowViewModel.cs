using BomberMan.Commands;
using BomberMan.Services;
using BomberMan.Views;
using BomberMan.Views.MainMenu;
using PropertyChanged;
using System.Windows.Input;

namespace BomberMan.ViewModels
{

    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        public object CurrentView { get; set; }
        public MainViewModel MainViewModel { get; set; }
        public bool IsHelpOverlayVisible { get; set; }

        public ICommand ShowHelpOverlayCommand { get; }
        public ICommand CloseHelpOverlayCommand { get; }
        public ICommand NavigateToGameCommand { get; }
        public ICommand PlayBackgroundMusicCommand { get; }
        public ICommand PlaySoundEffectCommand { get; }
        public ICommand NavigateToGameOverCommand { get; set; }
        public ICommand Restart { get; set; }

        private readonly AudioService audioService;
        string SelectedMusic { get; set; } = "BgMusic"; 

        public MainWindowViewModel()
        {
            ShowHelpOverlayCommand = new RelayCommand(ShowHelpOverlay);
            CloseHelpOverlayCommand = new RelayCommand(CloseHelpOverlay);
            audioService = new AudioService();
            PlayBackgroundMusic();

            PlayBackgroundMusicCommand = new RelayCommand(PlayBackgroundMusic);

            CurrentView = new MainMenuView();
            MainViewModel = new MainViewModel(); // Instantiate MainViewModel here
            NavigateToGameCommand = new RelayCommand(ShowGame);

            NavigateToGameOverCommand = new RelayCommand(ShowGameOver);
            Restart = new RelayCommand(ShowRestartedGame);

            MainViewModel.NavigateToGameOverCommand = NavigateToGameOverCommand;
            MainViewModel.RestartGame = Restart;
        }

        private void InitializeGameCommands()
        {
            // Initiera kommandona på nytt varje gång spelet startas om
            NavigateToGameOverCommand = new RelayCommand(ShowGameOver);
            Restart = new RelayCommand(ShowRestartedGame);
        }

        //Visar spelvyn och sätter datakontexten till MainViewModel. 
        private void ShowGame()
        {
            StopPlayBackgroundMusic();
            SelectedMusic = "BgMusic_5";
            PlayBackgroundMusic();

            var gameView = new GameView
            {
                DataContext = MainViewModel 
            };

            CurrentView = gameView;
            MainViewModel.StartCountdown();
        }

        // Som ShowtGame fast nollställer allt. 
        private void ShowRestartedGame()
        {
            StopPlayBackgroundMusic();
            SelectedMusic = "BgMusic_5";
            PlayBackgroundMusic();

            // Skapa om GameViewModel
            MainViewModel = new MainViewModel();

            // Återställ kommandon så att spelet kan navigera korrekt
            InitializeGameCommands();

            // Tilldela kommandona till GameViewModel igen
            MainViewModel.NavigateToGameOverCommand = NavigateToGameOverCommand;
            MainViewModel.RestartGame = Restart;

            var gameView = new GameView
            {
                DataContext = MainViewModel // Använd den nya MainViewModel
            };

            // Byt till GameView och starta nedräkningen
            CurrentView = gameView;
            MainViewModel.StartCountdown();
        }

        // Visar GameOverView och sätter datakontext till MainViewModel
        private async void ShowGameOver()
        {
            StopPlayBackgroundMusic();
            SelectedMusic = "BgMusic_3";

            var gameOverView = new GameOverView
            {
                DataContext = MainViewModel // Använd befintlig MainViewModel
            };

            // Vänta lite innan Game Over visas, för en mer smidig övergång
            await Task.Delay(1000);
            PlayBackgroundMusic();
            // Byt till GameOverView
            CurrentView = gameOverView;
        }

        private void PlayBackgroundMusic()
        {
            audioService.PlayClip(SelectedMusic);
        }
        public void StopPlayBackgroundMusic()
        {
            audioService.StopClip();
        }

        private void ShowHelpOverlay()
        {
            IsHelpOverlayVisible = true;
        }

        private void CloseHelpOverlay()
        {
            IsHelpOverlayVisible = false;
        }
    }
}
