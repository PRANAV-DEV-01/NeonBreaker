using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Ball : MonoBehaviour
    {
        public bool IsLost { get; private set; }

        private BallManager ballManager;
        private Rigidbody2D body;
        private float speed;
        private float lifeTime;

        public void Initialize(BallManager manager, float startSpeed)
        {
            ballManager = manager;
            speed = startSpeed;
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 direction, float launchSpeed)
        {
            speed = launchSpeed;
            body.linearVelocity = direction.normalized * speed;
        }

        private void FixedUpdate()
        {
            if (IsLost || ballManager == null)
            {
                return;
            }

            lifeTime += Time.fixedDeltaTime;

            if (transform.position.y < -10.5f || lifeTime > 18f)
            {
                LoseBall();
                return;
            }

            Vector2 velocity = body.linearVelocity;
            if (velocity.sqrMagnitude < 0.01f)
            {
                return;
            }

            speed = ballManager.CurrentSpeed;
            velocity = velocity.normalized * speed;

            if (Mathf.Abs(velocity.y) < 1.2f)
            {
                velocity.y = (velocity.y >= 0f ? 1f : -1f) * 1.2f;
                velocity = velocity.normalized * speed;
            }

            body.linearVelocity = velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsLost)
            {
                return;
            }

            Launcher launcher = collision.collider.GetComponent<Launcher>();
            if (launcher != null)
            {
                BounceFromLauncher(launcher);
            }
        }

        private void BounceFromLauncher(Launcher launcher)
        {
            float halfWidth = Mathf.Max(0.1f, launcher.Width * 0.5f);
            float offset = Mathf.Clamp((transform.position.x - launcher.transform.position.x) / halfWidth, -1f, 1f);
            float angle = 90f + offset * 62f;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            body.position = new Vector2(body.position.x, launcher.transform.position.y + 0.45f);
            body.linearVelocity = direction * speed;
            launcher.Pulse();
        }

        private void LoseBall()
        {
            if (IsLost)
            {
                return;
            }

            IsLost = true;
            ballManager.RemoveBall(this);
        }
    }
}
