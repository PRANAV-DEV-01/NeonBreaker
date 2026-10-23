using System;
using UnityEngine;

namespace NeonBreaker.Core
{
    public static class GameEvents
    {
        public static event Action<int, int> ScoreChanged;
        public static event Action<int> HighScoreChanged;
        public static event Action<int> LevelChanged;
        public static event Action<int> BricksChanged;
        public static event Action<int> ComboChanged;
        public static event Action<Vector2, Color, int> BrickDestroyed;
        public static event Action<Vector2, Color> BrickHit;
        public static event Action<Vector2> BombExploded;
        public static event Action<Vector2> BallLaunched;
        public static event Action<Vector2> BallLost;
        public static event Action AllBallsLost;
        public static event Action<PowerUpType> PowerUpCollected;
        public static event Action<PowerUpType> PowerUpExpired;
        public static event Action<int> LevelCleared;
        public static event Action<int> GameOverTriggered;
        public static event Action<int> VictoryTriggered;

        public static void RaiseScoreChanged(int score, int delta) => ScoreChanged?.Invoke(score, delta);
        public static void RaiseHighScoreChanged(int highScore) => HighScoreChanged?.Invoke(highScore);
        public static void RaiseLevelChanged(int level) => LevelChanged?.Invoke(level);
        public static void RaiseBricksChanged(int remaining) => BricksChanged?.Invoke(remaining);
        public static void RaiseComboChanged(int combo) => ComboChanged?.Invoke(combo);
        public static void RaiseBrickDestroyed(Vector2 position, Color color, int points) => BrickDestroyed?.Invoke(position, color, points);
        public static void RaiseBrickHit(Vector2 position, Color color) => BrickHit?.Invoke(position, color);
        public static void RaiseBombExploded(Vector2 position) => BombExploded?.Invoke(position);
        public static void RaiseBallLaunched(Vector2 position) => BallLaunched?.Invoke(position);
        public static void RaiseBallLost(Vector2 position) => BallLost?.Invoke(position);
        public static void RaiseAllBallsLost() => AllBallsLost?.Invoke();
        public static void RaisePowerUpCollected(PowerUpType type) => PowerUpCollected?.Invoke(type);
        public static void RaisePowerUpExpired(PowerUpType type) => PowerUpExpired?.Invoke(type);
        public static void RaiseLevelCleared(int level) => LevelCleared?.Invoke(level);
        public static void RaiseGameOver(int score) => GameOverTriggered?.Invoke(score);
        public static void RaiseVictory(int score) => VictoryTriggered?.Invoke(score);

        public static void ClearAllSubscriptions()
        {
            ScoreChanged = null;
            HighScoreChanged = null;
            LevelChanged = null;
            BricksChanged = null;
            ComboChanged = null;
            BrickDestroyed = null;
            BrickHit = null;
            BombExploded = null;
            BallLaunched = null;
            BallLost = null;
            AllBallsLost = null;
            PowerUpCollected = null;
            PowerUpExpired = null;
            LevelCleared = null;
            GameOverTriggered = null;
            VictoryTriggered = null;
        }
    }
}
