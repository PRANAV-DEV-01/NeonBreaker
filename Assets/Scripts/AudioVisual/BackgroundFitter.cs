using UnityEngine;

namespace NeonBreaker.AudioVisual
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundFitter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera == null || spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return;
            }

            float height = mainCamera.orthographicSize * 2f;
            float width = height * mainCamera.aspect;
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

            if (spriteSize.x <= 0f || spriteSize.y <= 0f)
            {
                return;
            }

            float scale = Mathf.Max(width / spriteSize.x, height / spriteSize.y);
            transform.localScale = Vector3.one * scale;
            transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 10f);
        }
    }
}
