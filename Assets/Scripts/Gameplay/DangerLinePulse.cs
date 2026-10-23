using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DangerLinePulse : MonoBehaviour
    {
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float minimumAlpha = 0.35f;
        [SerializeField] private float maximumAlpha = 0.8f;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            Color color = spriteRenderer.color;
            float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            color.a = Mathf.Lerp(minimumAlpha, maximumAlpha, wave);
            spriteRenderer.color = color;
        }
    }
}
