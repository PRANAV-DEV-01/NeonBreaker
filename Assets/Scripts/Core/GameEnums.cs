namespace NeonBreaker.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        LevelComplete,
        GameOver,
        Victory
    }

    public enum BrickType
    {
        Normal,
        Tough,
        Bomb,
        PowerUp
    }

    public enum PowerUpType
    {
        WideLauncher,
        SlowMotion,
        ScoreSurge
    }
}
