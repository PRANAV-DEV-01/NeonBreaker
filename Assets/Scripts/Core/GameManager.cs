using System.Collections;
using System.Collections.Generic;
using NeonBreaker.AudioVisual;
using NeonBreaker.Gameplay;
using NeonBreaker.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeonBreaker.Core
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Rules")]
        [SerializeField] private int maximumLevel = 10;
        [SerializeField] private int levelClearBonusPerLevel = 150;
        [SerializeField] private float turnEndDelay = 0.35f;
        [SerializeField] private float levelCompleteAutoDelay = 3.2f;

        public int Score { get; private set; }
        public int Level { get; private set; } = 1;
        public int Combo { get; private set; }
        public int LastBonus { get; private set; }
        public int MaximumLevel => maximumLevel;
        public bool IsEndless { get; private set; }

        public BrickManager BrickManager { get; private set; }
        public BallManager BallManager { get; private set; }
        public PowerUpManager PowerUpManager { get; private set; }
        public Launcher Launcher { get; private set; }
        public UIManager UIManager { get; private set; }
        public AudioManager AudioManager { get; private set; }
        public VFXManager VFXManager { get; private set; }
        public SaveManager SaveManager { get; private set; }

        public GameState CurrentState => stateMachine != null ? stateMachine.CurrentType : GameState.MainMenu;
        public bool CanLaunch => CurrentState == GameState.Playing
                                 && BallManager != null
                                 && BallManager.ActiveBallCount == 0
                                 && !BrickManager.IsBusy
                                 && !isTurnEnding
                                 && !levelClearing;

        private GameStateMachine stateMachine;
        private bool isTurnEnding;
        private bool levelClearing;
        private Coroutine endTurnRoutine;
        private Coroutine autoNextRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            GameEvents.ClearAllSubscriptions();

            BrickManager = FindAnyObjectByType<BrickManager>();
            BallManager = FindAnyObjectByType<BallManager>();
            PowerUpManager = FindAnyObjectByType<PowerUpManager>();
            Launcher = FindAnyObjectByType<Launcher>();
            UIManager = FindAnyObjectByType<UIManager>();
            AudioManager = FindAnyObjectByType<AudioManager>();
            VFXManager = FindAnyObjectByType<VFXManager>();
            SaveManager = FindAnyObjectByType<SaveManager>();

            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            GameEvents.BrickDestroyed += HandleBrickDestroyed;
            GameEvents.AllBallsLost += HandleAllBallsLost;
        }

        private void OnDisable()
        {
            GameEvents.BrickDestroyed -= HandleBrickDestroyed;
            GameEvents.AllBallsLost -= HandleAllBallsLost;
        }

        private void Start()
        {
            stateMachine = new GameStateMachine(new Dictionary<GameState, IGameState>
            {
                { GameState.MainMenu, new MainMenuState(this) },
                { GameState.Playing, new PlayingState(this) },
                { GameState.Paused, new PausedState(this) },
                { GameState.LevelComplete, new LevelCompleteState(this) },
                { GameState.GameOver, new GameOverState(this) },
                { GameState.Victory, new VictoryState(this) }
            });

            stateMachine.Initialize(GameState.MainMenu);
        }

        private void Update()
        {
            if (stateMachine == null)
            {
                return;
            }

            stateMachine.Tick();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (CurrentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }

            if (CurrentState != GameState.Playing)
            {
                return;
            }

            bool pointerOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!pointerOverUi && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                TryLaunch();
            }
        }

        public bool TryLaunch()
        {
            if (!CanLaunch || Launcher == null)
            {
                return false;
            }

            Combo = 0;
            GameEvents.RaiseComboChanged(Combo);
            BallManager.LaunchBall(Launcher.LaunchPoint, Launcher.AimDirection);
            return true;
        }

        public void StartGame()
        {
            Score = 0;
            Level = 1;
            Combo = 0;
            LastBonus = 0;
            IsEndless = false;
            levelClearing = false;
            isTurnEnding = false;

            GameEvents.RaiseScoreChanged(Score, 0);
            GameEvents.RaiseComboChanged(Combo);
            SetupLevel();
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void NextLevel()
        {
            if (CurrentState != GameState.LevelComplete)
            {
                return;
            }

            Level++;
            SetupLevel();
        }

        public void ContinueEndless()
        {
            if (CurrentState != GameState.Victory)
            {
                return;
            }

            IsEndless = true;
            Level++;
            SetupLevel();
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                stateMachine.ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                stateMachine.ChangeState(GameState.Playing);
            }
        }

        public void ReturnToMenu()
        {
            if (autoNextRoutine != null)
            {
                StopCoroutine(autoNextRoutine);
                autoNextRoutine = null;
            }

            BallManager.ResetBalls();
            BrickManager.ClearBricks();
            PowerUpManager.ClearAll();
            Score = 0;
            Combo = 0;
            Level = 1;
            LastBonus = 0;
            levelClearing = false;
            isTurnEnding = false;

            stateMachine.ChangeState(GameState.MainMenu);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void SetupLevel()
        {
            if (autoNextRoutine != null)
            {
                StopCoroutine(autoNextRoutine);
                autoNextRoutine = null;
            }

            BallManager.ResetBalls();
            PowerUpManager.ClearAll();
            BrickManager.GenerateLevel(Level);
            BallManager.SetLevel(Level);
            Launcher.ResetForLevel();

            levelClearing = false;
            isTurnEnding = false;

            GameEvents.RaiseLevelChanged(Level);
            GameEvents.RaiseBricksChanged(BrickManager.BrickCount);
            stateMachine.ChangeState(GameState.Playing);
        }

        private void HandleBrickDestroyed(Vector2 position, Color color, int scoreValue)
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            Combo++;
            float comboMultiplier = Mathf.Min(1f + (Combo - 1) * 0.35f, 4f);
            int surgeMultiplier = PowerUpManager != null ? PowerUpManager.ScoreMultiplier : 1;
            int points = Mathf.RoundToInt(scoreValue * comboMultiplier * surgeMultiplier);

            Score += points;
            GameEvents.RaiseScoreChanged(Score, points);
            GameEvents.RaiseComboChanged(Combo);

            if (BrickManager.BrickCount == 0 && !levelClearing)
            {
                levelClearing = true;
                StartCoroutine(FinishLevelAfterDelay());
            }
        }

        private IEnumerator FinishLevelAfterDelay()
        {
            yield return new WaitForSeconds(0.55f);

            if (CurrentState == GameState.Playing)
            {
                TriggerLevelCleared();
            }
        }

        private void HandleAllBallsLost()
        {
            if (CurrentState != GameState.Playing || isTurnEnding || levelClearing || BrickManager.BrickCount == 0)
            {
                return;
            }

            isTurnEnding = true;
            endTurnRoutine = StartCoroutine(EndTurnRoutine());
        }

        private IEnumerator EndTurnRoutine()
        {
            yield return new WaitForSeconds(turnEndDelay);

            if (CurrentState != GameState.Playing || levelClearing)
            {
                isTurnEnding = false;
                yield break;
            }

            if (BrickManager.BrickCount == 0)
            {
                isTurnEnding = false;
                TriggerLevelCleared();
                yield break;
            }

            BrickManager.IsBusy = true;
            yield return BrickManager.DescendRoutine();
            BrickManager.IsBusy = false;

            if (CurrentState != GameState.Playing)
            {
                isTurnEnding = false;
                yield break;
            }

            if (BrickManager.HasBrickBelowDangerLine())
            {
                isTurnEnding = false;
                TriggerGameOver();
                yield break;
            }

            if (BrickManager.BrickCount == 0)
            {
                isTurnEnding = false;
                TriggerLevelCleared();
                yield break;
            }

            isTurnEnding = false;
            Launcher.ResetForNewTurn();
        }

        private void TriggerLevelCleared()
        {
            if (CurrentState != GameState.Playing)
            {
                return;
            }

            levelClearing = false;
            LastBonus = levelClearBonusPerLevel * Level;
            Score += LastBonus;
            GameEvents.RaiseScoreChanged(Score, LastBonus);
            SaveManager.SubmitScore(Score, Level);
            BallManager.ResetBalls();
            PowerUpManager.ClearAll();
            GameEvents.RaiseLevelCleared(Level);

            if (Level >= maximumLevel && !IsEndless)
            {
                TriggerVictory();
                return;
            }

            stateMachine.ChangeState(GameState.LevelComplete);
            autoNextRoutine = StartCoroutine(AutoNextLevel());
        }

        private IEnumerator AutoNextLevel()
        {
            yield return new WaitForSecondsRealtime(levelCompleteAutoDelay);

            if (CurrentState == GameState.LevelComplete)
            {
                NextLevel();
            }
        }

        private void TriggerGameOver()
        {
            if (CurrentState == GameState.GameOver)
            {
                return;
            }

            if (endTurnRoutine != null)
            {
                StopCoroutine(endTurnRoutine);
            }

            SaveManager.SubmitScore(Score, Level);
            BallManager.ResetBalls();
            PowerUpManager.ClearAll();
            GameEvents.RaiseGameOver(Score);
            stateMachine.ChangeState(GameState.GameOver);
        }

        private void TriggerVictory()
        {
            SaveManager.SubmitScore(Score, Level);
            BallManager.ResetBalls();
            PowerUpManager.ClearAll();
            GameEvents.RaiseVictory(Score);
            stateMachine.ChangeState(GameState.Victory);
        }
    }
}
