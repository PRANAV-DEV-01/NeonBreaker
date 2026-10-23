using System.Collections;
using System.Collections.Generic;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [DefaultExecutionOrder(-40)]
    public class Launcher : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float minimumX = -4.25f;
        [SerializeField] private float maximumX = 4.25f;
        [SerializeField] private float moveSpeed = 13f;

        [Header("Size")]
        [SerializeField] private float normalWidth = 2.4f;
        [SerializeField] private float wideWidth = 3.7f;
        [SerializeField] private float height = 0.36f;

        [Header("Aiming")]
        [SerializeField] private float minimumAngle = 24f;
        [SerializeField] private float maximumAngle = 156f;
        [SerializeField] private int aimDotCount = 16;

        private readonly List<SpriteRenderer> aimDots = new List<SpriteRenderer>();
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider;
        private Coroutine pulseRoutine;
        private float aimAngle = 90f;

        public float Width => boxCollider != null ? boxCollider.size.x : normalWidth;
        public Vector2 AimDirection => new Vector2(Mathf.Cos(aimAngle * Mathf.Deg2Rad), Mathf.Sin(aimAngle * Mathf.Deg2Rad));
        public Vector2 LaunchPoint => (Vector2)transform.position + AimDirection * 0.55f + Vector2.up * 0.25f;

        private void Awake()
        {
            transform.position = new Vector3(0f, -7.35f, 0f);

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Art/launcher");
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = new Vector2(normalWidth, height);
            spriteRenderer.sortingOrder = 3;

            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(normalWidth, height);

            BuildAimDots();
        }

        private void BuildAimDots()
        {
            GameObject container = new GameObject("AimDots");
            Sprite dotSprite = Resources.Load<Sprite>("Art/aim_dot");

            for (int i = 0; i < aimDotCount; i++)
            {
                GameObject dotObject = new GameObject("AimDot_" + i);
                dotObject.transform.SetParent(container.transform);

                SpriteRenderer dotRenderer = dotObject.AddComponent<SpriteRenderer>();
                dotRenderer.sprite = dotSprite;
                dotRenderer.sortingOrder = 2;
                dotRenderer.color = new Color(0.5f, 1f, 1f, 0.5f);
                aimDots.Add(dotRenderer);
            }
        }

        private void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null)
            {
                return;
            }

            bool isPlaying = game.CurrentState == GameState.Playing;

            if (isPlaying)
            {
                HandleMovement();
                UpdateAimFromPointer();
            }

            UpdateAimDots(isPlaying && game.CanLaunch);
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(horizontal) < 0.01f)
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    horizontal = -1f;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    horizontal = 1f;
                }
            }

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x + horizontal * moveSpeed * Time.deltaTime, minimumX, maximumX);
            transform.position = position;
        }

        private void UpdateAimFromPointer()
        {
            Vector3 pointer = Input.mousePosition;
            if (pointer.x < 0f || pointer.x > Screen.width || pointer.y < 0f || pointer.y > Screen.height)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(pointer);
            Vector2 direction = (Vector2)worldPoint - (Vector2)transform.position;

            if (direction.y <= 0.05f)
            {
                return;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            aimAngle = Mathf.Clamp(angle, minimumAngle, maximumAngle);
        }

        private void UpdateAimDots(bool visible)
        {
            for (int i = 0; i < aimDots.Count; i++)
            {
                SpriteRenderer dot = aimDots[i];
                dot.gameObject.SetActive(visible);

                if (!visible)
                {
                    continue;
                }

                float distance = 0.85f + i * 0.33f;
                dot.transform.position = (Vector2)transform.position + AimDirection * distance;

                float fade = 1f - (float)i / aimDots.Count;
                dot.color = new Color(0.5f, 1f, 1f, 0.12f + 0.55f * fade);
                dot.transform.localScale = Vector3.one * Mathf.Lerp(0.7f, 0.28f, (float)i / aimDots.Count);
            }
        }

        public void SetWide(bool wide)
        {
            if (spriteRenderer == null || boxCollider == null)
            {
                return;
            }

            float targetWidth = wide ? wideWidth : normalWidth;
            spriteRenderer.size = new Vector2(targetWidth, height);
            boxCollider.size = new Vector2(targetWidth, height);
        }

        public void Pulse()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
            }

            pulseRoutine = StartCoroutine(PulseRoutine());
        }

        private IEnumerator PulseRoutine()
        {
            Vector2 baseSize = spriteRenderer.size;
            spriteRenderer.size = baseSize + new Vector2(0.2f, 0.08f);
            yield return new WaitForSeconds(0.06f);
            spriteRenderer.size = baseSize;
        }

        public void ResetForLevel()
        {
            Vector3 position = transform.position;
            position.x = 0f;
            transform.position = position;
            aimAngle = 90f;
            SetWide(false);
            UpdateAimDots(false);
        }

        public void ResetForNewTurn()
        {
            UpdateAimDots(false);
        }
    }
}
