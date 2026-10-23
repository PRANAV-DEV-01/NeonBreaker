using UnityEngine;

namespace NeonBreaker.Core
{
    [DefaultExecutionOrder(-90)]
    public class SaveManager : MonoBehaviour
    {
        private const string HighScoreKey = "NeonBreaker.HighScore";
        private const string BestLevelKey = "NeonBreaker.BestLevel";

        public int HighScore { get; private set; }
        public int BestLevel { get; private set; }

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            BestLevel = PlayerPrefs.GetInt(BestLevelKey, 1);
        }

        public void SubmitScore(int score, int level)
        {
            bool changed = false;

            if (score > HighScore)
            {
                HighScore = score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                GameEvents.RaiseHighScoreChanged(HighScore);
                changed = true;
            }

            if (level > BestLevel)
            {
                BestLevel = level;
                PlayerPrefs.SetInt(BestLevelKey, BestLevel);
                changed = true;
            }

            if (changed)
            {
                PlayerPrefs.Save();
            }
        }

        public void ResetHighScore()
        {
            HighScore = 0;
            BestLevel = 1;
            PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.DeleteKey(BestLevelKey);
            PlayerPrefs.Save();
            GameEvents.RaiseHighScoreChanged(HighScore);
        }
    }
}
