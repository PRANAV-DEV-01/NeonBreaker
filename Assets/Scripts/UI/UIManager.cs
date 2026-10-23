using System.Collections;
using NeonBreaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NeonBreaker.UI
{
    [DefaultExecutionOrder(-80)]
    public class UIManager : MonoBehaviour
    {
        private const float AutoContinueSeconds = 3.2f;

        private static readonly Color Cyan = new Color(0.28f, 0.94f, 1f);
        private static readonly Color Magenta = new Color(1f, 0.3f, 0.82f);
        private static readonly Color Gold = new Color(1f, 0.83f, 0.32f);
        private static readonly Color SoftWhite = new Color(0.85f, 0.92f, 1f);
        private static readonly Color Danger = new Color(1f, 0.32f, 0.38f);

        private Canvas canvas;
        private Sprite panelSprite;
        private Sprite buttonSprite;
        private Sprite menuBackground;
        private Sprite gameOverBackground;
        private Sprite victoryBackground;

        private GameObject mainMenuPanel;
        private GameObject hudPanel;
        private GameObject pausePanel;
        private GameObject levelCompletePanel;
        private GameObject gameOverPanel;
        private GameObject victoryPanel;
        private GameObject howToPlayPanel;

        private Text scoreText;
        private Text hudHighScoreText;
        private Text levelText;
        private Text bricksText;
        private Text powerUpText;
        private Text comboText;
        private Text menuHighScoreText;
        private Text menuBestLevelText;
        private Text gameOverScoreText;
        private Text gameOverHighScoreText;
        private Text victoryScoreText;
        private Text victoryHighScoreText;
        private Text levelCompleteTitleText;
        private Text levelCompleteBonusText;
        private Text levelCompleteCountdownText;
        private Image flashImage;

        private Coroutine comboRoutine;
        private Coroutine scorePunchRoutine;
        private Coroutine flashRoutine;
        private float levelCompleteStartTime;

        private void Awake()
        {
            LoadSprites();
            EnsureEventSystem();

            canvas = UiFactory.CreateCanvas("UI Canvas");
            canvas.transform.SetParent(transform, false);

            BuildMainMenu();
            BuildHud();
            BuildPause();
            BuildLevelComplete();
            BuildGameOver();
            BuildVictory();
            BuildHowToPlay();
            BuildFlash();

            HideAllMenus();
            HideHud();
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += HandleScoreChanged;
            GameEvents.HighScoreChanged += HandleHighScoreChanged;
            GameEvents.LevelChanged += HandleLevelChanged;
            GameEvents.BricksChanged += HandleBricksChanged;
            GameEvents.ComboChanged += HandleComboChanged;
            GameEvents.BombExploded += HandleBombExploded;
            GameEvents.LevelCleared += HandleLevelCleared;
            GameEvents.GameOverTriggered += HandleGameOverFlash;
            GameEvents.VictoryTriggered += HandleVictoryFlash;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= HandleScoreChanged;
            GameEvents.HighScoreChanged -= HandleHighScoreChanged;
            GameEvents.LevelChanged -= HandleLevelChanged;
            GameEvents.BricksChanged -= HandleBricksChanged;
            GameEvents.ComboChanged -= HandleComboChanged;
            GameEvents.BombExploded -= HandleBombExploded;
            GameEvents.LevelCleared -= HandleLevelCleared;
            GameEvents.GameOverTriggered -= HandleGameOverFlash;
            GameEvents.VictoryTriggered -= HandleVictoryFlash;
        }

        private void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null)
            {
                return;
            }

            if (hudPanel.activeSelf && powerUpText != null && game.PowerUpManager != null)
            {
                powerUpText.text = game.PowerUpManager.GetActiveSummary();
            }

            if (levelCompletePanel.activeSelf && levelCompleteCountdownText != null)
            {
                float remaining = Mathf.Max(0f, AutoContinueSeconds - (Time.unscaledTime - levelCompleteStartTime));
                levelCompleteCountdownText.text = "AUTO-CONTINUE IN " + Mathf.CeilToInt(remaining) + "s";
            }
        }

        private void LoadSprites()
        {
            panelSprite = Resources.Load<Sprite>("Art/ui_panel");
            buttonSprite = Resources.Load<Sprite>("Art/ui_button");
            menuBackground = Resources.Load<Sprite>("Art/bg_menu");
            gameOverBackground = Resources.Load<Sprite>("Art/bg_gameover");
            victoryBackground = Resources.Load<Sprite>("Art/bg_victory");
        }

        private void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }
        }

        private GameObject CreateScreenRoot(string name)
        {
            GameObject root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(canvas.transform, false);

            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return root;
        }

        private Text CreateAnchoredText(
            Transform parent,
            string name,
            string content,
            int fontSize,
            Color color,
            TextAnchor alignment,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size,
            float lineSpacing = 1f)
        {
            Text text = UiFactory.CreateText(parent, name, content, fontSize, color, alignment, size, Vector2.zero, true, lineSpacing);
            UiFactory.SetAnchored(text.rectTransform, anchor, pivot, position, size);
            return text;
        }

        private void BuildMainMenu()
        {
            mainMenuPanel = CreateScreenRoot("Main Menu");
            UiFactory.CreateFullScreenImage(mainMenuPanel.transform, "Menu Background", menuBackground, Color.white);

            CreateAnchoredText(mainMenuPanel.transform, "Title Top", "NEON", 190, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 540f), new Vector2(1000f, 240f));
            CreateAnchoredText(mainMenuPanel.transform, "Title Bottom", "BREAKER", 150, Magenta, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 370f), new Vector2(1000f, 210f));
            CreateAnchoredText(mainMenuPanel.transform, "Subtitle", "PROCEDURAL BRICK BREAKER", 34, new Color(0.75f, 0.86f, 1f, 0.9f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 240f), new Vector2(900f, 60f));

            menuHighScoreText = CreateAnchoredText(mainMenuPanel.transform, "High Score", "BEST  0", 44, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 140f), new Vector2(700f, 70f));
            menuBestLevelText = CreateAnchoredText(mainMenuPanel.transform, "Best Level", "CLEAR 10 LEVELS TO WIN", 28, new Color(0.7f, 0.8f, 0.95f, 0.8f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(900f, 50f));

            AddButton(mainMenuPanel.transform, "Play", "PLAY", 56, new Vector2(0f, -90f), new Vector2(540f, 132f), HandlePlayClicked);
            AddButton(mainMenuPanel.transform, "How To Play", "HOW TO PLAY", 38, new Vector2(0f, -262f), new Vector2(520f, 112f), HandleHowToPlayClicked);
            AddButton(mainMenuPanel.transform, "Quit", "QUIT", 34, new Vector2(0f, -412f), new Vector2(420f, 100f), HandleQuitClicked);

            CreateAnchoredText(mainMenuPanel.transform, "Footer", "A / D  MOVE          MOUSE  AIM          CLICK / SPACE  LAUNCH", 26, new Color(1f, 1f, 1f, 0.55f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 170f), new Vector2(980f, 50f));
            CreateAnchoredText(mainMenuPanel.transform, "Footer Note", "CLEAR 10 LEVELS  â€¢  ENDLESS MODE UNLOCKS", 26, new Color(1f, 0.55f, 0.9f, 0.75f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 230f), new Vector2(900f, 50f));
        }

        private void BuildHud()
        {
            hudPanel = CreateScreenRoot("HUD");

            scoreText = CreateAnchoredText(hudPanel.transform, "Score", "SCORE  0", 44, SoftWhite, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -36f), new Vector2(560f, 80f));
            hudHighScoreText = CreateAnchoredText(hudPanel.transform, "High Score", "BEST  0", 34, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -36f), new Vector2(500f, 70f));
            levelText = CreateAnchoredText(hudPanel.transform, "Level", "LEVEL 1", 34, Cyan, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -122f), new Vector2(420f, 60f));
            bricksText = CreateAnchoredText(hudPanel.transform, "Bricks", "BRICKS  0", 28, new Color(0.8f, 0.9f, 1f, 0.8f), TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -176f), new Vector2(420f, 60f));
            powerUpText = CreateAnchoredText(hudPanel.transform, "Power Ups", "", 30, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -112f), new Vector2(920f, 60f));

            comboText = CreateAnchoredText(hudPanel.transform, "Combo", "", 82, Magenta, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 260f), new Vector2(800f, 120f));

            Button pauseButton = UiFactory.CreateButton(hudPanel.transform, "Pause", "II", 32, Vector2.zero, new Vector2(104f, 84f), buttonSprite, HandlePauseClicked);
            UiFactory.SetAnchored(pauseButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -36f), new Vector2(104f, 84f));

            CreateAnchoredText(hudPanel.transform, "Controls", "A / D  MOVE      MOUSE  AIM      CLICK / SPACE  LAUNCH", 24, new Color(1f, 1f, 1f, 0.45f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 50f), new Vector2(980f, 50f));
        }

        private void BuildPause()
        {
            pausePanel = CreateScreenRoot("Pause Menu");
            UiFactory.CreateFullScreenImage(pausePanel.transform, "Dim", null, new Color(0.01f, 0.02f, 0.05f, 0.88f), true);
            UiFactory.CreatePanel(pausePanel.transform, "Panel", new Vector2(820f, 880f), panelSprite, Color.white);

            CreateAnchoredText(pausePanel.transform, "Title", "PAUSED", 96, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 300f), new Vector2(760f, 120f));
            AddButton(pausePanel.transform, "Continue", "CONTINUE", 44, new Vector2(0f, 70f), new Vector2(540f, 122f), HandleResumeClicked);
            AddButton(pausePanel.transform, "Restart", "RESTART", 40, new Vector2(0f, -80f), new Vector2(540f, 112f), HandleRestartClicked);
            AddButton(pausePanel.transform, "Back To Menu", "BACK TO MENU", 38, new Vector2(0f, -220f), new Vector2(540f, 112f), HandleMainMenuClicked);

            CreateAnchoredText(pausePanel.transform, "Hint", "ESC TO CONTINUE", 26, new Color(1f, 1f, 1f, 0.55f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -360f), new Vector2(500f, 50f));
        }

        private void BuildLevelComplete()
        {
            levelCompletePanel = CreateScreenRoot("Level Complete");
            UiFactory.CreateFullScreenImage(levelCompletePanel.transform, "Dim", null, new Color(0.01f, 0.02f, 0.05f, 0.86f), true);
            UiFactory.CreatePanel(levelCompletePanel.transform, "Panel", new Vector2(830f, 660f), panelSprite, Color.white);

            levelCompleteTitleText = CreateAnchoredText(levelCompletePanel.transform, "Title", "LEVEL 1 CLEARED", 62, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 200f), new Vector2(780f, 100f));
            levelCompleteBonusText = CreateAnchoredText(levelCompletePanel.transform, "Bonus", "+150 BONUS", 46, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(700f, 70f));
            levelCompleteCountdownText = CreateAnchoredText(levelCompletePanel.transform, "Countdown", "AUTO-CONTINUE IN 3s", 26, new Color(1f, 1f, 1f, 0.6f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(600f, 50f));

            AddButton(levelCompletePanel.transform, "Next Level", "NEXT LEVEL", 42, new Vector2(0f, -150f), new Vector2(540f, 120f), HandleNextLevelClicked);
        }

        private void BuildGameOver()
        {
            gameOverPanel = CreateScreenRoot("Game Over");
            UiFactory.CreateFullScreenImage(gameOverPanel.transform, "Background", gameOverBackground, Color.white);
            UiFactory.CreateFullScreenImage(gameOverPanel.transform, "Dim", null, new Color(0.03f, 0f, 0.02f, 0.42f), true);
            UiFactory.CreatePanel(gameOverPanel.transform, "Panel", new Vector2(920f, 1000f), panelSprite, Color.white);

            CreateAnchoredText(gameOverPanel.transform, "Title", "GAME OVER", 140, Danger, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 310f), new Vector2(900f, 190f));
            gameOverScoreText = CreateAnchoredText(gameOverPanel.transform, "Score", "SCORE  0", 50, SoftWhite, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(800f, 80f));
            gameOverHighScoreText = CreateAnchoredText(gameOverPanel.transform, "High Score", "BEST  0", 36, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 45f), new Vector2(800f, 60f));

            AddButton(gameOverPanel.transform, "Retry", "RETRY", 44, new Vector2(0f, -140f), new Vector2(540f, 120f), HandleRetryClicked);
            AddButton(gameOverPanel.transform, "Main Menu", "MAIN MENU", 38, new Vector2(0f, -295f), new Vector2(540f, 110f), HandleMainMenuClicked);
        }

        private void BuildVictory()
        {
            victoryPanel = CreateScreenRoot("Victory");
            UiFactory.CreateFullScreenImage(victoryPanel.transform, "Background", victoryBackground, Color.white);
            UiFactory.CreateFullScreenImage(victoryPanel.transform, "Dim", null, new Color(0f, 0.03f, 0.02f, 0.3f), true);
            UiFactory.CreatePanel(victoryPanel.transform, "Panel", new Vector2(940f, 1020f), panelSprite, Color.white);

            CreateAnchoredText(victoryPanel.transform, "Title", "VICTORY!", 142, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 320f), new Vector2(920f, 200f));
            victoryScoreText = CreateAnchoredText(victoryPanel.transform, "Score", "SCORE  0", 50, SoftWhite, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(800f, 80f));
            victoryHighScoreText = CreateAnchoredText(victoryPanel.transform, "High Score", "BEST  0", 36, Gold, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 55f), new Vector2(800f, 60f));

            AddButton(victoryPanel.transform, "Endless", "ENDLESS MODE", 40, new Vector2(0f, -140f), new Vector2(560f, 120f), HandleEndlessClicked);
            AddButton(victoryPanel.transform, "Main Menu", "MAIN MENU", 38, new Vector2(0f, -295f), new Vector2(540f, 110f), HandleMainMenuClicked);

            CreateAnchoredText(victoryPanel.transform, "Note", "KEEP YOUR SCORE AND PLAY FOREVER", 26, new Color(1f, 1f, 1f, 0.6f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -420f), new Vector2(820f, 50f));
        }

        private void BuildHowToPlay()
        {
            howToPlayPanel = CreateScreenRoot("How To Play");
            UiFactory.CreateFullScreenImage(howToPlayPanel.transform, "Dim", null, new Color(0.01f, 0.02f, 0.05f, 0.93f), true);
            UiFactory.CreatePanel(howToPlayPanel.transform, "Panel", new Vector2(940f, 1360f), panelSprite, Color.white);

            CreateAnchoredText(howToPlayPanel.transform, "Title", "HOW TO PLAY", 72, Cyan, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 540f), new Vector2(880f, 100f));

            string instructions =
                "<color=#FF4DD2>DESTROY EVERY BRICK</color>\n" +
                "Bricks descend after every shot.\n" +
                "Do not let them reach the red line.\n\n" +
                "<color=#54F0FF>A / D  OR  ARROWS</color>\n" +
                "Move the launcher\n\n" +
                "<color=#54F0FF>MOUSE</color>\n" +
                "Aim the shot\n\n" +
                "<color=#54F0FF>CLICK  OR  SPACE</color>\n" +
                "Launch the ball\n\n" +
                "<color=#FFD24F>POWER-UPS</color>\n" +
                "Catch falling capsules with the launcher.\n\n" +
                "WIDE         Wider launcher (12s)\n" +
                "SLOW         Slower ball (8s)\n" +
                "2X SCORE     Double points (10s)\n\n" +
                "Special bricks: <color=#FF8C5A>BOMBS</color> explode,\n" +
                "<color=#FF4DD2>PINK CAPSULE BRICKS</color> drop power-ups.\n\n" +
                "Clear 10 levels to reach Victory.\n" +
                "Then keep going in Endless Mode.";

            CreateAnchoredText(howToPlayPanel.transform, "Instructions", instructions, 33, SoftWhite, TextAnchor.UpperLeft, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(820f, 1020f), 1.22f);

            AddButton(howToPlayPanel.transform, "Close", "CLOSE", 38, new Vector2(0f, -540f), new Vector2(400f, 100f), HandleCloseHowToPlayClicked);
        }

        private void BuildFlash()
        {
            flashImage = UiFactory.CreateFullScreenImage(canvas.transform, "Flash", null, new Color(1f, 1f, 1f, 0f), false);
            flashImage.transform.SetAsLastSibling();
        }

        private void AddButton(Transform parent, string name, string label, int fontSize, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            UiFactory.CreateButton(parent, name, label, fontSize, position, size, buttonSprite, () =>
            {
                PlayClick();
                action();
            });
        }

        private void PlayClick()
        {
            if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
            {
                GameManager.Instance.AudioManager.PlayUiClick();
            }
        }

        public void ShowMainMenu(int highScore, int bestLevel)
        {
            HideAllMenus();
            mainMenuPanel.SetActive(true);
            menuHighScoreText.text = "BEST  " + highScore.ToString("N0");
            menuBestLevelText.text = bestLevel > 1 ? "HIGHEST LEVEL  " + bestLevel : "CLEAR 10 LEVELS TO WIN";
        }

        public void ShowHud(int level, int score, int bricks, int highScore)
        {
            hudPanel.SetActive(true);
            levelText.text = "LEVEL " + level;
            scoreText.text = "SCORE  " + score.ToString("N0");
            bricksText.text = "BRICKS  " + bricks;
            hudHighScoreText.text = "BEST  " + highScore.ToString("N0");
        }

        public void HideHud()
        {
            hudPanel.SetActive(false);
            comboText.text = string.Empty;

            if (comboRoutine != null)
            {
                StopCoroutine(comboRoutine);
                comboRoutine = null;
            }
        }

        public void ShowPause()
        {
            pausePanel.SetActive(true);
        }

        public void HidePause()
        {
            pausePanel.SetActive(false);
        }

        public void ShowLevelComplete(int level, int bonus, int maximumLevel)
        {
            levelCompletePanel.SetActive(true);
            levelCompleteTitleText.text = "LEVEL " + level + " CLEARED";
            levelCompleteBonusText.text = "+" + bonus.ToString("N0") + " BONUS";
            levelCompleteStartTime = Time.unscaledTime;
            levelCompleteCountdownText.text = "AUTO-CONTINUE IN " + Mathf.CeilToInt(AutoContinueSeconds) + "s";
        }

        public void HideLevelComplete()
        {
            levelCompletePanel.SetActive(false);
        }

        public void ShowGameOver(int score, int highScore)
        {
            gameOverPanel.SetActive(true);
            gameOverScoreText.text = "SCORE  " + score.ToString("N0");
            gameOverHighScoreText.text = "BEST  " + highScore.ToString("N0");
        }

        public void HideGameOver()
        {
            gameOverPanel.SetActive(false);
        }

        public void ShowVictory(int score, int highScore)
        {
            victoryPanel.SetActive(true);
            victoryScoreText.text = "SCORE  " + score.ToString("N0");
            victoryHighScoreText.text = "BEST  " + highScore.ToString("N0");
        }

        public void HideVictory()
        {
            victoryPanel.SetActive(false);
        }

        public void HideAllMenus()
        {
            mainMenuPanel.SetActive(false);
            pausePanel.SetActive(false);
            levelCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            victoryPanel.SetActive(false);
            howToPlayPanel.SetActive(false);
        }

        private void HandleScoreChanged(int score, int delta)
        {
            scoreText.text = "SCORE  " + score.ToString("N0");

            if (delta > 0 && hudPanel.activeSelf)
            {
                if (scorePunchRoutine != null)
                {
                    StopCoroutine(scorePunchRoutine);
                }

                scorePunchRoutine = StartCoroutine(PunchScore());
            }
        }

        private IEnumerator PunchScore()
        {
            const float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float scale = progress < 0.4f ? Mathf.Lerp(1f, 1.16f, progress / 0.4f) : Mathf.Lerp(1.16f, 1f, (progress - 0.4f) / 0.6f);
                scoreText.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            scoreText.transform.localScale = Vector3.one;
        }

        private void HandleHighScoreChanged(int highScore)
        {
            hudHighScoreText.text = "BEST  " + highScore.ToString("N0");
            menuHighScoreText.text = "BEST  " + highScore.ToString("N0");
        }

        private void HandleLevelChanged(int level)
        {
            levelText.text = "LEVEL " + level;
        }

        private void HandleBricksChanged(int remaining)
        {
            bricksText.text = "BRICKS  " + remaining;
        }

        private void HandleComboChanged(int combo)
        {
            if (combo < 2)
            {
                comboText.text = string.Empty;

                if (comboRoutine != null)
                {
                    StopCoroutine(comboRoutine);
                    comboRoutine = null;
                }

                return;
            }

            if (comboRoutine != null)
            {
                StopCoroutine(comboRoutine);
            }

            comboRoutine = StartCoroutine(ComboRoutine(combo));
        }

        private IEnumerator ComboRoutine(int combo)
        {
            comboText.text = combo >= 8 ? "UNSTOPPABLE  x" + combo : "COMBO  x" + combo;

            Color color = combo >= 8 ? Gold : Magenta;
            color.a = 1f;
            comboText.color = color;

            const float duration = 0.65f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                comboText.transform.localScale = Vector3.one * Mathf.Lerp(1.3f, 1f, progress);
                color.a = 1f - progress;
                comboText.color = color;
                yield return null;
            }

            comboText.text = string.Empty;
            comboText.transform.localScale = Vector3.one;
        }

        private void HandleBombExploded(Vector2 position)
        {
            StartFlash(new Color(1f, 0.55f, 0.15f), 0.12f, 0.2f);
        }

        private void HandleLevelCleared(int level)
        {
            StartFlash(Cyan, 0.12f, 0.3f);
        }

        private void HandleGameOverFlash(int score)
        {
            StartFlash(Danger, 0.4f, 0.55f);
        }

        private void HandleVictoryFlash(int score)
        {
            StartFlash(Gold, 0.28f, 0.6f);
        }

        private void StartFlash(Color color, float intensity, float duration)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine(color, intensity, duration));
        }

        private IEnumerator FlashRoutine(Color color, float intensity, float duration)
        {
            float elapsed = 0f;
            Color flashColor = color;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                flashColor.a = Mathf.Lerp(intensity, 0f, elapsed / duration);
                flashImage.color = flashColor;
                yield return null;
            }

            flashColor.a = 0f;
            flashImage.color = flashColor;
        }

        private void HandlePlayClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        private void HandleHowToPlayClicked()
        {
            howToPlayPanel.SetActive(true);
        }

        private void HandleCloseHowToPlayClicked()
        {
            howToPlayPanel.SetActive(false);
        }

        private void HandleQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }

        private void HandlePauseClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }

        private void HandleResumeClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        private void HandleRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        private void HandleMainMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }

        private void HandleNextLevelClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NextLevel();
            }
        }

        private void HandleRetryClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        private void HandleEndlessClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ContinueEndless();
            }
        }
    }
}
