using System.Collections;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.AudioVisual
{
    [DefaultExecutionOrder(-60)]
    public class VFXManager : MonoBehaviour
    {
        [SerializeField] private int maxParticles = 600;

        private Camera mainCamera;
        private Vector3 cameraOrigin;
        private ParticleSystem burstSystem;
        private Sprite ringSprite;
        private Coroutine shakeRoutine;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraOrigin = mainCamera.transform.position;
            }

            ringSprite = Resources.Load<Sprite>("Art/ring");
            CreateParticleSystem();
        }

        private void OnEnable()
        {
            GameEvents.BrickHit += HandleBrickHit;
            GameEvents.BrickDestroyed += HandleBrickDestroyed;
            GameEvents.BombExploded += HandleBombExploded;
            GameEvents.PowerUpCollected += HandlePowerUpCollected;
            GameEvents.LevelCleared += HandleLevelCleared;
            GameEvents.GameOverTriggered += HandleGameOver;
            GameEvents.VictoryTriggered += HandleVictory;
        }

        private void OnDisable()
        {
            GameEvents.BrickHit -= HandleBrickHit;
            GameEvents.BrickDestroyed -= HandleBrickDestroyed;
            GameEvents.BombExploded -= HandleBombExploded;
            GameEvents.PowerUpCollected -= HandlePowerUpCollected;
            GameEvents.LevelCleared -= HandleLevelCleared;
            GameEvents.GameOverTriggered -= HandleGameOver;
            GameEvents.VictoryTriggered -= HandleVictory;
        }

        private void CreateParticleSystem()
        {
            GameObject particleObject = new GameObject("BurstParticles");
            particleObject.transform.SetParent(transform);

            burstSystem = particleObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = burstSystem.main;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = 0.6f;
            main.startSpeed = 0f;
            main.startSize = 0.25f;
            main.startColor = Color.white;
            main.gravityModifier = 0.85f;
            main.maxParticles = maxParticles;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = burstSystem.emission;
            emission.enabled = false;

            ParticleSystem.ShapeModule shape = burstSystem.shape;
            shape.enabled = false;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = burstSystem.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = burstSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

            Sprite sprite = Resources.Load<Sprite>("Art/particle");
            ParticleSystemRenderer particleRenderer = particleObject.GetComponent<ParticleSystemRenderer>();
            particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            particleRenderer.sortingOrder = 10;

            Material material = new Material(Shader.Find("Sprites/Default"));
            if (sprite != null)
            {
                material.mainTexture = sprite.texture;
            }

            particleRenderer.material = material;
            burstSystem.Stop();
        }

        private void HandleBrickHit(Vector2 position, Color color)
        {
            Burst(position, color, 4, 2.4f, 0.18f);
        }

        private void HandleBrickDestroyed(Vector2 position, Color color, int points)
        {
            Burst(position, color, 18, 4.6f, 0.3f);
            Shake(0.045f, 0.12f);
        }

        private void HandleBombExploded(Vector2 position)
        {
            Burst(position, new Color(1f, 0.55f, 0.15f), 40, 7f, 0.5f);
            SpawnRing(position, new Color(1f, 0.55f, 0.15f), 3.4f);
            Shake(0.16f, 0.22f);
        }

        private void HandlePowerUpCollected(PowerUpType type)
        {
            Color color = new Color(0.5f, 1f, 1f);
            Burst(GetLauncherPosition(), color, 26, 4.5f, 0.35f);
            SpawnRing(GetLauncherPosition(), color, 2.2f);
            Shake(0.06f, 0.14f);
        }

        private void HandleLevelCleared(int level)
        {
            Vector3 center = new Vector3(0f, 2.5f, 0f);
            Color[] colors =
            {
                new Color(0.21f, 0.94f, 1f),
                new Color(1f, 0.24f, 0.8f),
                new Color(1f, 0.7f, 0.24f),
                new Color(0.42f, 1f, 0.56f)
            };

            for (int i = 0; i < colors.Length; i++)
            {
                Vector3 offset = new Vector3((i - 1.5f) * 2.4f, 0f, 0f);
                Burst(center + offset, colors[i], 34, 7.5f, 0.7f);
            }

            Shake(0.12f, 0.3f);
        }

        private void HandleGameOver(int score)
        {
            Burst(new Vector3(0f, -2f, 0f), new Color(1f, 0.3f, 0.35f), 60, 8f, 0.8f);
            Shake(0.3f, 0.45f);
        }

        private void HandleVictory(int score)
        {
            Burst(new Vector3(0f, 2f, 0f), new Color(1f, 0.85f, 0.3f), 70, 9f, 0.9f);
            SpawnRing(new Vector3(0f, 2f, 0f), new Color(1f, 0.85f, 0.3f), 4f);
            Shake(0.12f, 0.3f);
        }

        private Vector3 GetLauncherPosition()
        {
            GameManager game = GameManager.Instance;
            if (game != null && game.Launcher != null)
            {
                return game.Launcher.transform.position;
            }

            return new Vector3(0f, -7.3f, 0f);
        }

        public void Burst(Vector2 position, Color color, int count, float speed, float lifetime)
        {
            if (burstSystem == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float magnitude = Random.Range(speed * 0.35f, speed);

                ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams
                {
                    position = position,
                    velocity = new Vector3(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude, 0f),
                    startColor = color,
                    startSize = Random.Range(0.1f, 0.34f),
                    startLifetime = Random.Range(lifetime * 0.5f, lifetime * 1.4f)
                };

                burstSystem.Emit(emit, 1);
            }
        }

        public void SpawnRing(Vector2 position, Color color, float maxScale)
        {
            if (ringSprite == null)
            {
                return;
            }

            GameObject ringObject = new GameObject("Ring");
            ringObject.transform.position = position;

            SpriteRenderer renderer = ringObject.AddComponent<SpriteRenderer>();
            renderer.sprite = ringSprite;
            renderer.color = color;
            renderer.sortingOrder = 9;

            StartCoroutine(RingRoutine(ringObject, renderer, maxScale));
        }

        private IEnumerator RingRoutine(GameObject ringObject, SpriteRenderer renderer, float maxScale)
        {
            const float duration = 0.38f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float scale = Mathf.Lerp(0.4f, maxScale, progress);
                ringObject.transform.localScale = new Vector3(scale, scale, 1f);

                Color color = renderer.color;
                color.a = 1f - progress;
                renderer.color = color;
                yield return null;
            }

            Destroy(ringObject);
        }

        public void Shake(float strength, float duration)
        {
            if (mainCamera == null)
            {
                return;
            }

            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
            }

            shakeRoutine = StartCoroutine(ShakeRoutine(strength, duration));
        }

        private IEnumerator ShakeRoutine(float strength, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float fade = 1f - Mathf.Clamp01(elapsed / duration);
                Vector2 offset = Random.insideUnitCircle * strength * fade;
                mainCamera.transform.position = cameraOrigin + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }

            mainCamera.transform.position = cameraOrigin;
        }
    }
}
