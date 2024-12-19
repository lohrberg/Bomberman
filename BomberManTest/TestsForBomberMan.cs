using BomberMan.Models;
using BomberMan.Models.BombBoard;
using BomberMan.Models.Gameboard;
using BomberMan.Models.Items.Bombs;
using BomberMan.Models.Tiles;
using BomberMan.ViewModels;
using BomberMan.Views.Tiles;
using Moq;


namespace BomberManTest
{   //Viktigt test f�r att kunna utesluta tiles som orsak ifall spelaren inte kan r�ra sig fr�n startpositionen.
    public class TestsForBomberMan
    {
        [Theory]
        [InlineData(50, 50)]
        [InlineData(50, 100)]
        [InlineData(100, 50)]
        public void TestIfTileAtAndAroundPlayerStartingPositionIsWalkable(int x, int y)
        {
            GameBoard gameBoard = new GameBoard(1);


            // Act
            var result = gameBoard.GetTileAtPosition(x, y);
            

            // Assert
            Assert.True(result.IsWalkable); // Kontrollera att resultatet �r som f�rv�ntat


        }

        //Viktigt test f�r att kunna veta att vi iaf spawnar en spelare p� r�tt position �ven om ingen bild sysns.

        [Fact]
        public void CheckIfPlayerHasSpawnedAtCorrectPosition()
        {

            MainViewModel viewModel = new MainViewModel(true);

            var result = viewModel.Player;

            Assert.Equal(result.PlayerX, 50);
            Assert.Equal(result.PlayerY, 50);

        }
        //Kontrollerar s� att CurrentPlayerScore i ScoreModel korrekt tar emot och uppdaterar med v�rdet fr�n points via MainviewModel.
        [Theory]
        [InlineData(0, 500, 500)]
        [InlineData(500, 500, 1000)]
        [InlineData(9000, 1000, 10000)]
        public void TestToSeeIfScoreModelScoreGetsAndAddsValueFromCurrentPlayerScore(int currentPlayerScore, int points, int expectedResult) 
        {
            ScoreModel scoreModel = new ScoreModel();

            scoreModel.CurrentPlayerScore = currentPlayerScore;

            scoreModel.AddPoints(points);

            Assert.Equal(expectedResult, scoreModel.CurrentPlayerScore);
        
        }
        // Strul med bildladdande
        [Fact]
        public void TestIfBombIsAddedToActiveBombList() 
        {
            

            BombModel bomb = new BombModel(true);
            BombBoard bombBoard = new BombBoard(true);

            int expectedResult = 1;

            bombBoard.AddBomb(bomb);

            int result = bombBoard.ActiveBombList.Count();

            Assert.Equal(expectedResult, result);
        }

        //kontrollerar s� att gameboard laddar korrekt antal tiles p� samtliga levels.
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void TestIfGameBoardGetsFilledWithTheRightAmountOfTilesForEachLevel(int level)
        {
            GameBoard gameBoard = new GameBoard(level);


            // Act
            int result = gameBoard.TilesCollection.Count();
            int expectedResult = 225;


            // Assert
            Assert.Equal(result, expectedResult); // Kontrollera att resultatet �r som f�rv�ntat


        }

    }
}