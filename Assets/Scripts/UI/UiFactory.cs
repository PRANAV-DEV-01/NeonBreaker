using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NeonBreaker.UI
{
    public static class UiFactory
    {
        private static Font cachedFont;

        public static Font Font
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return cachedFont;
            }
        }

        public static Canvas CreateCanvas(string name)
        {
            GameObject canvasObject = new GameObject(name);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static Image CreateFullScreenImage(Transform parent, string name, Sprite sprite, Color color, bool raycastTarget = false)
        {
            Image image = CreateImage(parent, name, sprite, color, raycastTarget);
            RectTransform rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return image;
        }

        public static Image CreateImage(Transform parent, string name, Sprite sprite, Color color, bool raycastTarget)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);

            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = raycastTarget;

            if (sprite != null && sprite.border != Vector4.zero)
            {
                image.type = Image.Type.Sliced;
            }

            return image;
        }

        public static Image CreatePanel(Transform parent, string name, Vector2 size, Sprite sprite, Color color)
        {
            Image image = CreateImage(parent, name, sprite, color, false);
            SetAnchored(image.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size);
            return image;
        }

        public static Text CreateText(
            Transform parent,
            string name,
            string content,
            int fontSize,
            Color color,
            TextAnchor alignment,
            Vector2 size,
            Vector2 position,
            bool shadow = true,
            float lineSpacing = 1f)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.AddComponent<Text>();
            text.font = Font;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.lineSpacing = lineSpacing;
            text.supportRichText = true;

            SetAnchored(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);

            if (shadow)
            {
                Shadow shadowEffect = textObject.AddComponent<Shadow>();
                shadowEffect.effectColor = new Color(0f, 0f, 0f, 0.75f);
                shadowEffect.effectDistance = new Vector2(3f, -4f);
            }

            return text;
        }

        public static Button CreateButton(
            Transform parent,
            string name,
            string label,
            int fontSize,
            Vector2 position,
            Vector2 size,
            Sprite sprite,
            UnityAction onClick)
        {
            Image image = CreateImage(parent, name, sprite, Color.white, true);
            SetAnchored(image.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);

            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.82f, 1f, 1f);
            colors.pressedColor = new Color(0.65f, 0.9f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.45f, 0.45f, 0.45f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            button.onClick.AddListener(onClick);

            Text text = CreateText(
                image.transform,
                "Label",
                label,
                fontSize,
                Color.white,
                TextAnchor.MiddleCenter,
                new Vector2(size.x - 24f, size.y),
                Vector2.zero,
                false);

            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.03f, 0.2f, 0.32f, 0.95f);
            outline.effectDistance = new Vector2(2f, -2f);

            return button;
        }

        public static void SetAnchored(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
