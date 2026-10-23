using System.Collections.Generic;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [DefaultExecutionOrder(-45)]
    public class BallManager : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float baseSpeed = 11.5f;
        [SerializeField] private float speedPerLevel = 0.35f;
        [SerializeField] private float maximumSpeed = 16f;

        [Header("Visuals")]
        [SerializeField] private float ballRadius = 0.16f;

        private readonly List<Ball> activeBalls = new List<Ball>();
        private PhysicsMaterial2D bounceMaterial;
        private Material trailMaterial;
        private Sprite ballSprite;
        private PowerUpManager powerUpManager;
        private int level = 1;

        public int ActiveBallCount => activeBalls.Count;
        public float CurrentSpeed
        {
            get
            {
                float levelSpeed = Mathf.Min(baseSpeed + (level - 1) * speedPerLevel, maximumSpeed);
                float slowMultiplier = powerUpManager != null ? powerUpManager.SlowMotionMultiplier : 1f;
                return levelSpeed * slowMultiplier;
            }
        }

        private void Awake()
        {
            ballSprite = Resources.Load<Sprite>("Art/ball");
            bounceMaterial = new PhysicsMaterial2D("BallBounce")
            {
                bounciness = 1f,
                friction = 0.02f
            };

            Shader spriteShader = Shader.Find("Sprites/Default");
            trailMaterial = new Material(spriteShader);
        }

        private void Start()
        {
            powerUpManager = FindAnyObjectByType<PowerUpManager>();
        }

        public void SetLevel(int newLevel)
        {
            level = Mathf.Max(1, newLevel);
        }

        public void LaunchBall(Vector2 position, Vector2 direction)
        {
            GameObject ballObject = new GameObject("Ball");
            ballObject.transform.position = position;

            SpriteRenderer spriteRenderer = ballObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = ballSprite;
            spriteRenderer.sortingOrder = 5;

            AddTrail(ballObject);

            Rigidbody2D body = ballObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.freezeRotation = true;

            CircleCollider2D circle = ballObject.AddComponent<CircleCollider2D>();
            circle.radius = ballRadius;
            circle.sharedMaterial = bounceMaterial;

            Ball ball = ballObject.AddComponent<Ball>();
            ball.Initialize(this, CurrentSpeed);
            ball.Launch(direction, CurrentSpeed);

            activeBalls.Add(ball);
            GameEvents.RaiseBallLaunched(position);
        }

        private void AddTrail(GameObject ballObject)
        {
            TrailRenderer trail = ballObject.AddComponent<TrailRenderer>();
            trail.time = 0.22f;
            trail.startWidth = 0.17f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.04f;
            trail.material = trailMaterial;
            trail.sortingOrder = 4;
            trail.startColor = new Color(0.4f, 1f, 1f, 0.85f);
            trail.endColor = new Color(0.4f, 1f, 1f, 0f);
        }

        public void RemoveBall(Ball ball)
        {
            if (ball == null || !activeBalls.Contains(ball))
            {
                return;
            }

            Vector2 lostPosition = ball.transform.position;
            activeBalls.Remove(ball);
            Destroy(ball.gameObject);

            GameEvents.RaiseBallLost(lostPosition);

            if (activeBalls.Count == 0)
            {
                GameEvents.RaiseAllBallsLost();
            }
        }

        public void ResetBalls()
        {
            foreach (Ball ball in activeBalls)
            {
                if (ball != null)
                {
                    Destroy(ball.gameObject);
                }
            }

            activeBalls.Clear();
        }
    }
}
