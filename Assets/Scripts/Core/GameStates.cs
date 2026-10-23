using UnityEngine;

namespace NeonBreaker.Core
{
    public class MainMenuState : IGameState
    {
        private readonly GameManager game;

        public MainMenuState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 1f;
            game.UIManager.ShowMainMenu(game.SaveManager.HighScore, game.SaveManager.BestLevel);
        }

        public void Exit()
        {
            game.UIManager.HideAllMenus();
        }

        public void Tick()
        {
        }
    }

    public class PlayingState : IGameState
    {
        private readonly GameManager game;

        public PlayingState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 1f;
            game.UIManager.HidePause();
            game.UIManager.HideLevelComplete();
            game.UIManager.HideGameOver();
            game.UIManager.HideVictory();
            game.UIManager.ShowHud(game.Level, game.Score, game.BrickManager.BrickCount, game.SaveManager.HighScore);
        }

        public void Exit()
        {
            game.UIManager.HideHud();
        }

        public void Tick()
        {
        }
    }

    public class PausedState : IGameState
    {
        private readonly GameManager game;

        public PausedState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
            game.UIManager.ShowPause();
        }

        public void Exit()
        {
            game.UIManager.HidePause();
        }

        public void Tick()
        {
        }
    }

    public class LevelCompleteState : IGameState
    {
        private readonly GameManager game;

        public LevelCompleteState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
            game.UIManager.HideHud();
            game.UIManager.ShowLevelComplete(game.Level, game.LastBonus, game.MaximumLevel);
        }

        public void Exit()
        {
            game.UIManager.HideLevelComplete();
        }

        public void Tick()
        {
        }
    }

    public class GameOverState : IGameState
    {
        private readonly GameManager game;

        public GameOverState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
            game.UIManager.HideHud();
            game.UIManager.ShowGameOver(game.Score, game.SaveManager.HighScore);
        }

        public void Exit()
        {
            game.UIManager.HideGameOver();
        }

        public void Tick()
        {
        }
    }

    public class VictoryState : IGameState
    {
        private readonly GameManager game;

        public VictoryState(GameManager gameManager)
        {
            game = gameManager;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
            game.UIManager.HideHud();
            game.UIManager.ShowVictory(game.Score, game.SaveManager.HighScore);
        }

        public void Exit()
        {
            game.UIManager.HideVictory();
        }

        public void Tick()
        {
        }
    }
}
