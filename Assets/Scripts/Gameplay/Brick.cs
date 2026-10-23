using System.Collections;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Brick : MonoBehaviour
    {
        public BrickType Type { get; private set; }
        public int ScoreValue { get; private set; }
        public Color BrickColor { get; private set; }
        public int HitPoints { get; private set; }
        public bool IsDestroyed { get; private set; }

        private BrickManager brickManager;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D brickCollider;
        private Color baseColor = Color.white;

        public void Initialize(BrickManager owner, BrickType type, int hitPoints, Sprite sprite, Color color, int scoreValue)
        {
            brickManager = owner;
            Type = type;
            HitPoints = hitPoints;
            ScoreValue = scoreValue;
            BrickColor = color;

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 2;

            brickCollider = GetComponent<BoxCollider2D>();
            brickCollider.size = new Vector2(1.02f, 0.5f);

            StartCoroutine(SpawnAnimation());
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsDestroyed)
            {
                return;
            }

            Ball ball = collision.collider.GetComponent<Ball>();
            if (ball == null)
            {
                ball = collision.collider.GetComponentInParent<Ball>();
            }

            if (ball != null && !ball.IsLost)
            {
                TakeHit(1);
            }
        }

        public void TakeHit(int damage)
        {
            if (IsDestroyed)
            {
                return;
            }

            HitPoints -= damage;

            if (HitPoints > 0)
            {
                GameEvents.RaiseBrickHit(transform.position, BrickColor);
                baseColor = new Color(0.74f, 0.8f, 0.92f);
                spriteRenderer.color = baseColor;
                StartCoroutine(HitFlash());
                return;
            }

            DestroyBrick();
        }

        private void DestroyBrick()
        {
            IsDestroyed = true;
            brickCollider.enabled = false;

            brickManager.NotifyBrickRemoved(this);
            GameEvents.RaiseBrickDestroyed(transform.position, BrickColor, ScoreValue);

            if (Type == BrickType.Bomb)
            {
                brickManager.ExplodeAt(transform.position, 1.5f);
            }
            else if (Type == BrickType.PowerUp)
            {
                brickManager.DropPowerUp(transform.position);
            }

            Destroy(gameObject);
        }

        private IEnumerator SpawnAnimation()
        {
            const float duration = 0.18f;
            float elapsed = 0f;
            transform.localScale = Vector3.zero;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, progress);
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        private IEnumerator HitFlash()
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.07f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = original;
            }
        }
    }
}
