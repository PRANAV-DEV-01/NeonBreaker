using System.Collections.Generic;
using System.Text;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [DefaultExecutionOrder(-42)]
    public class PowerUpManager : MonoBehaviour
    {
        [Header("Durations")]
        [SerializeField] private float wideDuration = 12f;
        [SerializeField] private float slowDuration = 8f;
        [SerializeField] private float surgeDuration = 10f;

        [Header("Drops")]
        [SerializeField] private float dropSpeed = 3.2f;

        private readonly List<PowerUp> activeDrops = new List<PowerUp>();
        private readonly Dictionary<PowerUpType, float> activeUntil = new Dictionary<PowerUpType, float>();
        private readonly Dictionary<PowerUpType, Sprite> icons = new Dictionary<PowerUpType, Sprite>();

        public Launcher Launcher { get; private set; }

        public float SlowMotionMultiplier => IsActive(PowerUpType.SlowMotion) ? 0.62f : 1f;
        public int ScoreMultiplier => IsActive(PowerUpType.ScoreSurge) ? 2 : 1;

        private void Awake()
        {
            icons[PowerUpType.WideLauncher] = Resources.Load<Sprite>("Art/powerup_wide");
            icons[PowerUpType.SlowMotion] = Resources.Load<Sprite>("Art/powerup_slow");
            icons[PowerUpType.ScoreSurge] = Resources.Load<Sprite>("Art/powerup_surge");
        }

        private void Start()
        {
            Launcher = FindAnyObjectByType<Launcher>();
        }

        private void Update()
        {
            if (activeUntil.Count == 0)
            {
                return;
            }

            List<PowerUpType> expired = null;
            foreach (KeyValuePair<PowerUpType, float> pair in activeUntil)
            {
                if (Time.time >= pair.Value)
                {
                    if (expired == null)
                    {
                        expired = new List<PowerUpType>();
                    }

                    expired.Add(pair.Key);
                }
            }

            if (expired == null)
            {
                return;
            }

            foreach (PowerUpType type in expired)
            {
                activeUntil.Remove(type);

                if (type == PowerUpType.WideLauncher && Launcher != null)
                {
                    Launcher.SetWide(false);
                }

                GameEvents.RaisePowerUpExpired(type);
            }
        }

        public void SpawnDrop(Vector2 position)
        {
            PowerUpType type = (PowerUpType)Random.Range(0, 3);
            GameObject dropObject = new GameObject("PowerUp_" + type);
            dropObject.transform.position = position;

            PowerUp drop = dropObject.AddComponent<PowerUp>();
            drop.Initialize(this, type, icons[type], dropSpeed);
            activeDrops.Add(drop);
        }

        public void Collect(PowerUp powerUp)
        {
            if (powerUp == null || !activeDrops.Contains(powerUp))
            {
                return;
            }

            activeDrops.Remove(powerUp);
            Activate(powerUp.Type);
            Destroy(powerUp.gameObject);
        }

        public void ForgetDrop(PowerUp powerUp)
        {
            activeDrops.Remove(powerUp);
        }

        private void Activate(PowerUpType type)
        {
            activeUntil[type] = Time.time + GetDuration(type);

            if (type == PowerUpType.WideLauncher && Launcher != null)
            {
                Launcher.SetWide(true);
            }

            GameEvents.RaisePowerUpCollected(type);
        }

        private float GetDuration(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.WideLauncher:
                    return wideDuration;
                case PowerUpType.SlowMotion:
                    return slowDuration;
                case PowerUpType.ScoreSurge:
                    return surgeDuration;
                default:
                    return 8f;
            }
        }

        public bool IsActive(PowerUpType type)
        {
            return activeUntil.TryGetValue(type, out float endTime) && Time.time < endTime;
        }

        public string GetActiveSummary()
        {
            if (activeUntil.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<PowerUpType, float> pair in activeUntil)
            {
                float remaining = Mathf.Max(0f, pair.Value - Time.time);

                if (builder.Length > 0)
                {
                    builder.Append("     ");
                }

                builder.Append(GetDisplayName(pair.Key));
                builder.Append(' ');
                builder.Append(Mathf.CeilToInt(remaining));
                builder.Append('s');
            }

            return builder.ToString();
        }

        private static string GetDisplayName(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.WideLauncher:
                    return "WIDE";
                case PowerUpType.SlowMotion:
                    return "SLOW";
                case PowerUpType.ScoreSurge:
                    return "2X SCORE";
                default:
                    return type.ToString();
            }
        }

        public void ClearAll()
        {
            if (Launcher != null)
            {
                Launcher.SetWide(false);
            }

            activeUntil.Clear();

            foreach (PowerUp drop in activeDrops)
            {
                if (drop != null)
                {
                    Destroy(drop.gameObject);
                }
            }

            activeDrops.Clear();
        }
    }
}
