using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    public class PowerUp : MonoBehaviour
    {
        public PowerUpType Type { get; private set; }

        private PowerUpManager powerUpManager;
        private SpriteRenderer spriteRenderer;
        private float fallSpeed = 3.2f;
        private float spinSpeed;
        private Vector3 baseScale = Vector3.one;

        public void Initialize(PowerUpManager manager, PowerUpType type, Sprite icon, float speed)
        {
            powerUpManager = manager;
            Type = type;
            fallSpeed = speed;
            spinSpeed = Random.Range(-70f, 70f);

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = icon;
            spriteRenderer.sortingOrder = 4;
        }

        private void Update()
        {
            transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

            float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.08f;
            transform.localScale = baseScale * pulse;

            if (transform.position.y < -10f)
            {
                powerUpManager.ForgetDrop(this);
                Destroy(gameObject);
                return;
            }

            Launcher launcher = powerUpManager.Launcher;
            if (launcher == null)
            {
                return;
            }

            Vector2 delta = transform.position - launcher.transform.position;
            bool inX = Mathf.Abs(delta.x) <= launcher.Width * 0.5f + 0.25f;
            bool inY = Mathf.Abs(delta.y) <= 0.7f;

            if (inX && inY)
            {
                powerUpManager.Collect(this);
            }
        }
    }
}
