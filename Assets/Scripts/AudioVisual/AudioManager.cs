using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.AudioVisual
{
    [DefaultExecutionOrder(-70)]
    public class AudioManager : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.28f;

        [Range(0f, 1f)]
        [SerializeField] private float effectsVolume = 0.7f;

        private AudioSource musicSource;
        private AudioSource effectsSource;

        private AudioClip musicClip;
        private AudioClip hitClip;
        private AudioClip breakClip;
        private AudioClip launchClip;
        private AudioClip lostClip;
        private AudioClip powerUpClip;
        private AudioClip gameOverClip;
        private AudioClip victoryClip;
        private AudioClip levelClearClip;
        private AudioClip clickClip;

        private void Awake()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;

            effectsSource = gameObject.AddComponent<AudioSource>();
            effectsSource.playOnAwake = false;
            effectsSource.volume = effectsVolume;

            musicClip = Resources.Load<AudioClip>("Audio/bgm");
            hitClip = Resources.Load<AudioClip>("Audio/hit");
            breakClip = Resources.Load<AudioClip>("Audio/break");
            launchClip = Resources.Load<AudioClip>("Audio/launch");
            lostClip = Resources.Load<AudioClip>("Audio/lost");
            powerUpClip = Resources.Load<AudioClip>("Audio/powerup");
            gameOverClip = Resources.Load<AudioClip>("Audio/gameover");
            victoryClip = Resources.Load<AudioClip>("Audio/victory");
            levelClearClip = Resources.Load<AudioClip>("Audio/levelclear");
            clickClip = Resources.Load<AudioClip>("Audio/click");
        }

        private void OnEnable()
        {
            GameEvents.BrickHit += HandleBrickHit;
            GameEvents.BrickDestroyed += HandleBrickDestroyed;
            GameEvents.BallLaunched += HandleBallLaunched;
            GameEvents.BallLost += HandleBallLost;
            GameEvents.PowerUpCollected += HandlePowerUpCollected;
            GameEvents.GameOverTriggered += HandleGameOver;
            GameEvents.VictoryTriggered += HandleVictory;
            GameEvents.LevelCleared += HandleLevelCleared;
        }

        private void OnDisable()
        {
            GameEvents.BrickHit -= HandleBrickHit;
            GameEvents.BrickDestroyed -= HandleBrickDestroyed;
            GameEvents.BallLaunched -= HandleBallLaunched;
            GameEvents.BallLost -= HandleBallLost;
            GameEvents.PowerUpCollected -= HandlePowerUpCollected;
            GameEvents.GameOverTriggered -= HandleGameOver;
            GameEvents.VictoryTriggered -= HandleVictory;
            GameEvents.LevelCleared -= HandleLevelCleared;
        }

        private void Start()
        {
            PlayMusic();
        }

        private void PlayMusic()
        {
            if (musicClip != null && !musicSource.isPlaying)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }

        private void HandleBrickHit(Vector2 position, Color color)
        {
            PlayEffect(hitClip, 0.45f, 0.08f);
        }

        private void HandleBrickDestroyed(Vector2 position, Color color, int points)
        {
            PlayEffect(breakClip, 0.6f, 0.1f);
        }

        private void HandleBallLaunched(Vector2 position)
        {
            PlayEffect(launchClip, 0.7f, 0f);
        }

        private void HandleBallLost(Vector2 position)
        {
            PlayEffect(lostClip, 0.45f, 0f);
        }

        private void HandlePowerUpCollected(PowerUpType type)
        {
            PlayEffect(powerUpClip, 0.8f, 0f);
        }

        private void HandleGameOver(int score)
        {
            PlayEffect(gameOverClip, 0.9f, 0f);
        }

        private void HandleVictory(int score)
        {
            PlayEffect(victoryClip, 0.9f, 0f);
        }

        private void HandleLevelCleared(int level)
        {
            PlayEffect(levelClearClip, 0.8f, 0f);
        }

        public void PlayUiClick()
        {
            PlayEffect(clickClip, 0.55f, 0.04f);
        }

        private void PlayEffect(AudioClip clip, float volume, float randomPitch)
        {
            if (clip == null || effectsSource == null)
            {
                return;
            }

            effectsSource.pitch = 1f + Random.Range(-randomPitch, randomPitch);
            effectsSource.PlayOneShot(clip, volume);
        }
    }
}
