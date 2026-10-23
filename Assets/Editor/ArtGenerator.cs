using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NeonBreaker.EditorTools
{
    public static class ArtGenerator
    {
        private const string ArtFolder = "Assets/Resources/Art";

        private static readonly Color Cyan = new Color(0.21f, 0.94f, 1f);
        private static readonly Color Magenta = new Color(1f, 0.24f, 0.80f);
        private static readonly Color Orange = new Color(1f, 0.70f, 0.24f);
        private static readonly Color Green = new Color(0.42f, 1f, 0.56f);
        private static readonly Color Purple = new Color(0.65f, 0.48f, 1f);
        private static readonly Color Blue = new Color(0.30f, 0.61f, 1f);
        private static readonly Color Red = new Color(1f, 0.35f, 0.43f);
        private static readonly Color Gold = new Color(1f, 0.83f, 0.32f);

        private static readonly Dictionary<string, Vector4> SpriteBorders = new Dictionary<string, Vector4>
        {
            { "ui_panel", new Vector4(40f, 40f, 40f, 40f) },
            { "ui_button", new Vector4(30f, 30f, 30f, 30f) },
            { "launcher", new Vector4(30f, 0f, 30f, 0f) }
        };

        [MenuItem("Neon Breaker/Regenerate Art")]
        public static void RegenerateArt()
        {
            GenerateAll();
            AssetDatabase.Refresh();
            ConfigureImports();
            AssetDatabase.Refresh();
            Debug.Log("Neon Breaker art regenerated.");
        }

        public static void GenerateAll()
        {
            Directory.CreateDirectory(ArtFolder);

            GenerateBackgrounds();
            GenerateBricks();
            GenerateGameplaySprites();
            GenerateUiSprites();
            GeneratePowerUpIcons();
            GenerateDangerLine();

            AssetDatabase.Refresh();
        }

        private static void GenerateBackgrounds()
        {
            GenerateMenuBackground();
            GenerateGameBackground();
            GenerateGameOverBackground();
            GenerateVictoryBackground();
        }

        private static void GenerateMenuBackground()
        {
            Picture picture = new Picture(1080, 1920);
            picture.Gradient(new Color(0.015f, 0.012f, 0.05f), new Color(0.09f, 0.05f, 0.24f), new Color(0.02f, 0.015f, 0.06f));

            picture.Glow(new Vector2(540f, 1210f), 520f, new Color(Magenta.r, Magenta.g, Magenta.b, 0.4f), 2.2f);
            picture.Glow(new Vector2(540f, 1210f), 300f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.2f), 2.4f);
            picture.StrokeCircle(new Vector2(540f, 1210f), 260f, 3f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.35f));
            picture.StrokeCircle(new Vector2(540f, 1210f), 210f, 1.5f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.22f));
            picture.FillCircle(new Vector2(540f, 1210f), 160f, new Color(1f, 0.25f, 0.7f, 0.14f));

            picture.AddStars(190, 1337, 3.2f, new Color(0.8f, 0.9f, 1f, 0.55f));

            Vector2 vanishingPoint = new Vector2(540f, 980f);
            for (int i = 0; i < 15; i++)
            {
                float t = i / 14f;
                float y = 130f + Mathf.Pow(t, 2.1f) * 850f;
                picture.DrawLine(new Vector2(20f, y), new Vector2(1060f, y), new Color(Cyan.r, Cyan.g, Cyan.b, 0.06f + 0.03f * t), 1.5f);
            }

            for (int i = -7; i <= 7; i++)
            {
                Vector2 bottom = new Vector2(540f + i * 170f, -180f);
                picture.DrawLine(vanishingPoint, bottom, new Color(Magenta.r, Magenta.g, Magenta.b, 0.07f), 1.5f);
            }

            picture.Vignette(0.75f);
            picture.FillRect(0, 0, 1080, 120, new Color(0f, 0f, 0f, 0.35f));
            Save("bg_menu", picture);
        }

        private static void GenerateGameBackground()
        {
            Picture picture = new Picture(1080, 1920);
            picture.Gradient(new Color(0.01f, 0.015f, 0.045f), new Color(0.035f, 0.03f, 0.12f), new Color(0.015f, 0.02f, 0.06f));

            picture.Glow(new Vector2(540f, 1500f), 620f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.1f), 2.6f);
            picture.Glow(new Vector2(180f, 400f), 420f, new Color(Magenta.r, Magenta.g, Magenta.b, 0.08f), 2.8f);
            picture.Glow(new Vector2(900f, 900f), 380f, new Color(Purple.r, Purple.g, Purple.b, 0.07f), 2.8f);

            picture.AddStars(110, 4242, 2.4f, new Color(0.7f, 0.85f, 1f, 0.4f));

            for (int x = 40; x < 1080; x += 84)
            {
                picture.DrawLine(new Vector2(x, 40f), new Vector2(x, 1880f), new Color(Cyan.r, Cyan.g, Cyan.b, 0.025f), 1f);
            }

            for (int y = 60; y < 1920; y += 84)
            {
                picture.DrawLine(new Vector2(40f, y), new Vector2(1040f, y), new Color(Cyan.r, Cyan.g, Cyan.b, 0.025f), 1f);
            }

            picture.StrokeRoundedRect(new Vector2(540f, 960f), new Vector2(1040f, 1880f), 40f, 3f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.13f));
            picture.StrokeRoundedRect(new Vector2(540f, 960f), new Vector2(1020f, 1860f), 34f, 1f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.06f));

            picture.Vignette(0.62f);
            Save("bg_game", picture);
        }

        private static void GenerateGameOverBackground()
        {
            Picture picture = new Picture(1080, 1920);
            picture.Gradient(new Color(0.02f, 0.005f, 0.01f), new Color(0.18f, 0.02f, 0.06f), new Color(0.035f, 0.005f, 0.015f));

            picture.Glow(new Vector2(540f, 1200f), 620f, new Color(1f, 0.15f, 0.2f, 0.28f), 2.3f);
            picture.Glow(new Vector2(540f, 1200f), 260f, new Color(1f, 0.4f, 0.2f, 0.18f), 2.6f);

            System.Random random = new System.Random(991);
            for (int i = 0; i < 26; i++)
            {
                float angle = (float)random.NextDouble() * Mathf.PI * 2f;
                float length = 260f + (float)random.NextDouble() * 760f;
                Vector2 start = new Vector2(540f, 1150f);
                Vector2 end = start + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
                picture.DrawLine(start, end, new Color(1f, 0.2f, 0.25f, 0.1f), 2.5f);

                Vector2 mid = Vector2.Lerp(start, end, 0.55f);
                Vector2 branch = mid + new Vector2(Mathf.Cos(angle + 0.7f), Mathf.Sin(angle + 0.7f)) * 150f;
                picture.DrawLine(mid, branch, new Color(1f, 0.2f, 0.25f, 0.07f), 1.5f);
            }

            picture.AddStars(70, 5150, 2.6f, new Color(1f, 0.6f, 0.6f, 0.35f));
            picture.Vignette(0.8f);
            Save("bg_gameover", picture);
        }

        private static void GenerateVictoryBackground()
        {
            Picture picture = new Picture(1080, 1920);
            picture.Gradient(new Color(0.01f, 0.04f, 0.03f), new Color(0.03f, 0.16f, 0.13f), new Color(0.01f, 0.03f, 0.04f));

            Vector2 center = new Vector2(540f, 1350f);
            picture.Glow(center, 680f, new Color(Gold.r, Gold.g, Gold.b, 0.28f), 2.4f);
            picture.Glow(new Vector2(540f, 300f), 520f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.09f), 2.6f);

            for (int i = 0; i < 32; i++)
            {
                float angle = i * Mathf.PI * 2f / 32f;
                Vector2 end = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 1400f;
                Color rayColor = new Color(Gold.r, Gold.g, Gold.b, 0.035f + (i % 3) * 0.02f);
                picture.DrawLine(center, end, rayColor, 5f);
            }

            picture.StrokeCircle(center, 250f, 2f, new Color(Gold.r, Gold.g, Gold.b, 0.25f));
            picture.AddStars(150, 777, 3.0f, new Color(1f, 0.95f, 0.7f, 0.55f));
            picture.Vignette(0.7f);
            Save("bg_victory", picture);
        }

        private static void GenerateBricks()
        {
            Color[] palette = { Cyan, Magenta, Orange, Green, Purple, Blue, Red };
            string[] names = { "cyan", "magenta", "orange", "green", "purple", "blue", "red" };

            for (int i = 0; i < palette.Length; i++)
            {
                MakeNormalBrick("brick_" + names[i], palette[i]);
            }

            MakeToughBrick();
            MakeBombBrick();
            MakePowerBrick();
        }

        private static void MakeNormalBrick(string fileName, Color baseColor)
        {
            Picture picture = new Picture(112, 56);
            Vector2 center = new Vector2(56f, 28f);
            Color bright = Color.Lerp(baseColor, Color.white, 0.45f);
            Color dark = Color.Lerp(baseColor, Color.black, 0.55f);

            picture.Glow(center, 64f, new Color(baseColor.r, baseColor.g, baseColor.b, 0.28f), 2.4f);
            picture.FillRoundedRect(center, new Vector2(112f, 56f), 13f, bright);
            picture.FillRoundedRect(center, new Vector2(105f, 49f), 10f, Color.Lerp(baseColor, Color.black, 0.18f));
            picture.FillRoundedRect(new Vector2(56f, 38f), new Vector2(94f, 16f), 6f, new Color(1f, 1f, 1f, 0.2f));
            picture.FillRoundedRect(new Vector2(56f, 13f), new Vector2(94f, 10f), 5f, new Color(0f, 0f, 0f, 0.22f));
            picture.FillRoundedRect(new Vector2(56f, 28f), new Vector2(82f, 5f), 2.5f, new Color(1f, 1f, 1f, 0.1f));

            picture.FillCircle(new Vector2(15f, 43f), 2.2f, new Color(1f, 1f, 1f, 0.5f));
            picture.FillCircle(new Vector2(97f, 43f), 2.2f, new Color(1f, 1f, 1f, 0.5f));

            picture.FillRect(0, 0, 2, 56, new Color(bright.r, bright.g, bright.b, 0.6f));
            picture.FillRect(110, 0, 2, 56, new Color(dark.r, dark.g, dark.b, 0.5f));

            Save(fileName, picture);
        }

        private static void MakeToughBrick()
        {
            Picture picture = new Picture(112, 56);
            Vector2 center = new Vector2(56f, 28f);
            Color steel = new Color(0.55f, 0.63f, 0.76f);

            picture.Glow(center, 62f, new Color(0.55f, 0.7f, 0.9f, 0.25f), 2.4f);
            picture.FillRoundedRect(center, new Vector2(112f, 56f), 13f, new Color(0.8f, 0.86f, 0.95f));
            picture.FillRoundedRect(center, new Vector2(105f, 49f), 10f, new Color(0.26f, 0.31f, 0.4f));
            picture.FillRoundedRect(new Vector2(56f, 40f), new Vector2(94f, 12f), 5f, new Color(0.62f, 0.7f, 0.84f, 0.55f));
            picture.FillRoundedRect(new Vector2(56f, 14f), new Vector2(94f, 10f), 5f, new Color(0f, 0f, 0f, 0.3f));

            Vector2[] rivets =
            {
                new Vector2(19f, 20f),
                new Vector2(93f, 20f),
                new Vector2(19f, 37f),
                new Vector2(93f, 37f)
            };

            foreach (Vector2 rivet in rivets)
            {
                picture.FillCircle(rivet, 4f, new Color(0.12f, 0.16f, 0.24f));
                picture.FillCircle(rivet + new Vector2(-1f, 1f), 1.5f, new Color(0.9f, 0.95f, 1f, 0.7f));
            }

            picture.DrawLine(new Vector2(36f, 14f), new Vector2(50f, 30f), new Color(1f, 1f, 1f, 0.18f), 1.4f);
            picture.DrawLine(new Vector2(50f, 30f), new Vector2(46f, 42f), new Color(1f, 1f, 1f, 0.13f), 1.2f);
            picture.FillRect(0, 0, 2, 56, new Color(steel.r, steel.g, steel.b, 0.55f));
            Save("brick_tough", picture);
        }

        private static void MakeBombBrick()
        {
            Picture picture = new Picture(112, 56);
            Vector2 center = new Vector2(56f, 28f);
            Color baseColor = new Color(0.35f, 0.09f, 0.11f);

            picture.Glow(center, 64f, new Color(1f, 0.3f, 0.2f, 0.32f), 2.2f);
            picture.FillRoundedRect(center, new Vector2(112f, 56f), 13f, new Color(1f, 0.45f, 0.3f));
            picture.FillRoundedRect(center, new Vector2(105f, 49f), 10f, baseColor);
            picture.FillRoundedRect(new Vector2(56f, 38f), new Vector2(94f, 14f), 6f, new Color(0.55f, 0.15f, 0.15f, 0.6f));

            picture.FillCircle(new Vector2(53f, 26f), 15f, new Color(0.06f, 0.02f, 0.04f));
            picture.StrokeCircle(new Vector2(53f, 26f), 15f, 2f, new Color(1f, 0.5f, 0.35f, 0.9f));
            picture.FillCircle(new Vector2(47f, 32f), 4f, new Color(1f, 1f, 1f, 0.35f));

            picture.DrawLine(new Vector2(61f, 38f), new Vector2(69f, 48f), new Color(1f, 0.75f, 0.3f), 2.5f);
            picture.Glow(new Vector2(71f, 49f), 12f, new Color(1f, 0.7f, 0.2f, 0.9f), 1.6f);
            picture.FillCircle(new Vector2(71f, 49f), 2.5f, new Color(1f, 0.95f, 0.7f));

            Save("brick_bomb", picture);
        }

        private static void MakePowerBrick()
        {
            Picture picture = new Picture(112, 56);
            Vector2 center = new Vector2(56f, 28f);

            picture.Glow(center, 65f, new Color(Magenta.r, Magenta.g, Magenta.b, 0.4f), 2.2f);
            picture.FillRoundedRect(center, new Vector2(112f, 56f), 13f, new Color(1f, 0.6f, 0.92f));
            picture.FillRoundedRect(center, new Vector2(105f, 49f), 10f, new Color(0.3f, 0.04f, 0.24f));
            picture.FillRoundedRect(new Vector2(56f, 38f), new Vector2(94f, 14f), 6f, new Color(1f, 0.4f, 0.85f, 0.35f));

            picture.FillRoundedRect(new Vector2(56f, 26f), new Vector2(42f, 20f), 10f, new Color(1f, 1f, 1f, 0.95f));
            picture.FillRoundedRect(new Vector2(56f, 26f), new Vector2(34f, 12f), 6f, new Color(1f, 0.35f, 0.85f));
            picture.FillRect(50, 22, 2, 8, new Color(1f, 1f, 1f, 0.8f));
            picture.FillCircle(new Vector2(29f, 46f), 2f, new Color(1f, 1f, 1f, 0.8f));
            picture.FillCircle(new Vector2(87f, 15f), 1.6f, new Color(1f, 1f, 1f, 0.7f));

            Save("brick_power", picture);
        }

        private static void GenerateGameplaySprites()
        {
            MakeBall();
            MakeLauncher();
            MakeAimDot();
            MakeParticle();
            MakeRing();
        }

        private static void MakeBall()
        {
            Picture picture = new Picture(40, 40);
            Vector2 center = new Vector2(20f, 20f);

            picture.Glow(center, 20f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.9f), 1.8f);
            picture.FillCircle(center, 11f, Color.white);
            picture.StrokeCircle(center, 13f, 1.5f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.95f));
            picture.FillCircle(new Vector2(16f, 24f), 3f, new Color(1f, 1f, 1f, 0.95f));

            Save("ball", picture);
        }

        private static void MakeLauncher()
        {
            Picture picture = new Picture(256, 64);
            Vector2 center = new Vector2(128f, 32f);

            picture.Glow(center, 70f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.4f), 2.4f);
            picture.FillRoundedRect(center, new Vector2(256f, 64f), 26f, new Color(0.55f, 0.98f, 1f));
            picture.FillRoundedRect(center, new Vector2(248f, 56f), 22f, new Color(0.03f, 0.18f, 0.3f));
            picture.FillRoundedRect(new Vector2(128f, 44f), new Vector2(210f, 12f), 6f, new Color(0.45f, 0.95f, 1f, 0.45f));
            picture.FillRoundedRect(new Vector2(128f, 32f), new Vector2(196f, 8f), 4f, new Color(Magenta.r, Magenta.g, Magenta.b, 0.9f));
            picture.FillRoundedRect(new Vector2(128f, 18f), new Vector2(210f, 8f), 4f, new Color(0f, 0f, 0f, 0.28f));
            picture.FillCircle(new Vector2(30f, 32f), 12f, new Color(0.55f, 0.98f, 1f, 0.55f));
            picture.FillCircle(new Vector2(226f, 32f), 12f, new Color(0.55f, 0.98f, 1f, 0.55f));

            Save("launcher", picture);
        }

        private static void MakeAimDot()
        {
            Picture picture = new Picture(24, 24);
            Vector2 center = new Vector2(12f, 12f);

            picture.Glow(center, 11f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.8f), 1.9f);
            picture.FillCircle(center, 3.5f, Color.white);

            Save("aim_dot", picture);
        }

        private static void MakeParticle()
        {
            Picture picture = new Picture(32, 32);
            picture.Glow(new Vector2(16f, 16f), 16f, Color.white, 1.7f);
            picture.FillCircle(new Vector2(16f, 16f), 5f, Color.white);
            Save("particle", picture);
        }

        private static void MakeRing()
        {
            Picture picture = new Picture(128, 128);
            Vector2 center = new Vector2(64f, 64f);

            picture.Glow(center, 62f, new Color(1f, 1f, 1f, 0.25f), 2.4f);
            picture.StrokeCircle(center, 52f, 6f, new Color(1f, 1f, 1f, 0.95f));
            picture.StrokeCircle(center, 58f, 2f, new Color(1f, 1f, 1f, 0.35f));

            Save("ring", picture);
        }

        private static void GenerateUiSprites()
        {
            MakePanel();
            MakeButton();
        }

        private static void MakePanel()
        {
            Picture picture = new Picture(256, 256);
            Vector2 center = new Vector2(128f, 128f);

            picture.Glow(center, 128f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.08f), 2.4f);
            picture.FillRoundedRect(center, new Vector2(256f, 256f), 32f, new Color(0.02f, 0.06f, 0.14f, 0.97f));
            picture.StrokeRoundedRect(center, new Vector2(252f, 252f), 30f, 4f, new Color(0.35f, 0.95f, 1f, 0.65f));
            picture.StrokeRoundedRect(center, new Vector2(240f, 240f), 24f, 1.5f, new Color(1f, 1f, 1f, 0.1f));
            picture.FillRoundedRect(new Vector2(128f, 232f), new Vector2(200f, 10f), 5f, new Color(0.35f, 0.95f, 1f, 0.1f));

            Save("ui_panel", picture);
        }

        private static void MakeButton()
        {
            Picture picture = new Picture(300, 96);
            Vector2 center = new Vector2(150f, 48f);

            picture.Glow(center, 90f, new Color(Cyan.r, Cyan.g, Cyan.b, 0.25f), 2.4f);
            picture.FillRoundedRect(center, new Vector2(300f, 96f), 24f, new Color(0.4f, 0.95f, 1f, 0.9f));
            picture.FillRoundedRect(center, new Vector2(292f, 88f), 20f, new Color(0.03f, 0.13f, 0.25f));
            picture.FillRoundedRect(new Vector2(150f, 70f), new Vector2(240f, 14f), 6f, new Color(0.3f, 0.8f, 1f, 0.4f));
            picture.FillRoundedRect(new Vector2(150f, 24f), new Vector2(240f, 12f), 5f, new Color(0f, 0f, 0f, 0.35f));
            picture.FillRoundedRect(new Vector2(150f, 48f), new Vector2(240f, 2f), 1f, new Color(Magenta.r, Magenta.g, Magenta.b, 0.45f));

            Save("ui_button", picture);
        }

        private static void GeneratePowerUpIcons()
        {
            MakeWideIcon();
            MakeSlowIcon();
            MakeSurgeIcon();
        }

        private static void MakeWideIcon()
        {
            Picture picture = MakeIconBase(Cyan);
            Vector2 center = new Vector2(64f, 64f);
            Color symbol = Color.white;

            picture.FillRoundedRect(center, new Vector2(52f, 10f), 5f, symbol);
            picture.FillTriangle(new Vector2(20f, 64f), new Vector2(40f, 50f), new Vector2(40f, 78f), symbol);
            picture.FillTriangle(new Vector2(108f, 64f), new Vector2(88f, 50f), new Vector2(88f, 78f), symbol);

            Save("powerup_wide", picture);
        }

        private static void MakeSlowIcon()
        {
            Picture picture = MakeIconBase(new Color(0.55f, 0.75f, 1f));
            Vector2 center = new Vector2(64f, 64f);
            Color symbol = Color.white;

            picture.StrokeCircle(center, 28f, 4f, symbol);
            picture.DrawLine(center, new Vector2(64f, 82f), symbol, 4f);
            picture.DrawLine(center, new Vector2(80f, 64f), symbol, 4f);
            picture.FillCircle(center, 4f, symbol);

            Save("powerup_slow", picture);
        }

        private static void MakeSurgeIcon()
        {
            Picture picture = MakeIconBase(Gold);
            Vector2 center = new Vector2(64f, 64f);

            picture.FillPolygon(CreateStar(center, 30f, 13f, 5), Color.white);
            picture.FillPolygon(CreateStar(center, 16f, 7f, 5), new Color(1f, 0.85f, 0.4f));

            Save("powerup_surge", picture);
        }

        private static Picture MakeIconBase(Color accent)
        {
            Picture picture = new Picture(128, 128);
            Vector2 center = new Vector2(64f, 64f);

            picture.Glow(center, 60f, new Color(accent.r, accent.g, accent.b, 0.5f), 2.2f);
            picture.FillCircle(center, 46f, new Color(0.03f, 0.06f, 0.14f, 0.96f));
            picture.StrokeCircle(center, 46f, 3f, new Color(accent.r, accent.g, accent.b, 0.95f));
            picture.StrokeCircle(center, 40f, 1.5f, new Color(1f, 1f, 1f, 0.14f));

            return picture;
        }

        private static void GenerateDangerLine()
        {
            Picture picture = new Picture(256, 16);

            for (int x = 0; x < 256; x++)
            {
                float t = x / 255f;
                float alpha = Mathf.Pow(Mathf.Sin(t * Mathf.PI), 2f);
                picture.FillRect(x, 0, 1, 16, new Color(1f, 1f, 1f, alpha));
            }

            Save("danger_line", picture);
        }

        public static void ConfigureImports()
        {
            string[] files = Directory.GetFiles(ArtFolder, "*.png");

            foreach (string file in files)
            {
                string assetPath = file.Replace('\\', '/');

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(file);

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100f;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 2048;

                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteBorder = SpriteBorders.TryGetValue(fileName, out Vector4 border) ? border : Vector4.zero;
                importer.SetTextureSettings(settings);

                importer.SaveAndReimport();
            }
        }

        private static void Save(string fileName, Picture picture)
        {
            Texture2D texture = new Texture2D(picture.Width, picture.Height, TextureFormat.RGBA32, false);
            texture.SetPixels32(picture.Pixels);
            texture.Apply();

            string path = Path.Combine(ArtFolder, fileName + ".png");
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }

        private static Vector2[] CreateStar(Vector2 center, float outerRadius, float innerRadius, int points)
        {
            Vector2[] vertices = new Vector2[points * 2];

            for (int i = 0; i < points * 2; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                float angle = -Mathf.PI * 0.5f + i * Mathf.PI / points;
                vertices[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }

            return vertices;
        }

        private class Picture
        {
            public readonly int Width;
            public readonly int Height;
            public readonly Color32[] Pixels;

            public Picture(int width, int height)
            {
                Width = width;
                Height = height;
                Pixels = new Color32[width * height];
                Clear(new Color(0f, 0f, 0f, 0f));
            }

            public void Clear(Color color)
            {
                Color32 color32 = color;
                for (int i = 0; i < Pixels.Length; i++)
                {
                    Pixels[i] = color32;
                }
            }

            public void Gradient(Color bottom, Color middle, Color top)
            {
                for (int y = 0; y < Height; y++)
                {
                    float t = Height > 1 ? (float)y / (Height - 1) : 0f;
                    Color color = t < 0.55f
                        ? Color.Lerp(bottom, middle, t / 0.55f)
                        : Color.Lerp(middle, top, (t - 0.55f) / 0.45f);

                    for (int x = 0; x < Width; x++)
                    {
                        Pixels[y * Width + x] = color;
                    }
                }
            }

            public void Blend(int x, int y, Color color)
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height || color.a <= 0f)
                {
                    return;
                }

                int index = y * Width + x;
                Color destination = Pixels[index];
                float sourceAlpha = Mathf.Clamp01(color.a);
                float destinationAlpha = destination.a;
                float outAlpha = sourceAlpha + destinationAlpha * (1f - sourceAlpha);

                if (outAlpha <= 0.0001f)
                {
                    Pixels[index] = new Color32(0, 0, 0, 0);
                    return;
                }

                float red = (color.r * sourceAlpha + destination.r * destinationAlpha * (1f - sourceAlpha)) / outAlpha;
                float green = (color.g * sourceAlpha + destination.g * destinationAlpha * (1f - sourceAlpha)) / outAlpha;
                float blue = (color.b * sourceAlpha + destination.b * destinationAlpha * (1f - sourceAlpha)) / outAlpha;

                Pixels[index] = new Color32(
                    (byte)(Mathf.Clamp01(red) * 255f),
                    (byte)(Mathf.Clamp01(green) * 255f),
                    (byte)(Mathf.Clamp01(blue) * 255f),
                    (byte)(Mathf.Clamp01(outAlpha) * 255f));
            }

            public void FillRect(int x, int y, int width, int height, Color color)
            {
                int startX = Mathf.Max(0, x);
                int startY = Mathf.Max(0, y);
                int endX = Mathf.Min(Width, x + width);
                int endY = Mathf.Min(Height, y + height);

                for (int py = startY; py < endY; py++)
                {
                    for (int px = startX; px < endX; px++)
                    {
                        Blend(px, py, color);
                    }
                }
            }

            public void FillCircle(Vector2 center, float radius, Color color)
            {
                int minX = Mathf.FloorToInt(center.x - radius - 1f);
                int maxX = Mathf.CeilToInt(center.x + radius + 1f);
                int minY = Mathf.FloorToInt(center.y - radius - 1f);
                int maxY = Mathf.CeilToInt(center.y + radius + 1f);

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) - radius;
                        float coverage = Mathf.Clamp01(0.5f - distance);

                        if (coverage > 0f)
                        {
                            Blend(x, y, new Color(color.r, color.g, color.b, color.a * coverage));
                        }
                    }
                }
            }

            public void StrokeCircle(Vector2 center, float radius, float thickness, Color color)
            {
                int minX = Mathf.FloorToInt(center.x - radius - thickness - 1f);
                int maxX = Mathf.CeilToInt(center.x + radius + thickness + 1f);
                int minY = Mathf.FloorToInt(center.y - radius - thickness - 1f);
                int maxY = Mathf.CeilToInt(center.y + radius + thickness + 1f);

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float distance = Mathf.Abs(Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) - radius) - thickness * 0.5f;
                        float coverage = Mathf.Clamp01(0.5f - distance);

                        if (coverage > 0f)
                        {
                            Blend(x, y, new Color(color.r, color.g, color.b, color.a * coverage));
                        }
                    }
                }
            }

            public void FillRoundedRect(Vector2 center, Vector2 size, float radius, Color color)
            {
                int minX = Mathf.FloorToInt(center.x - size.x * 0.5f - 2f);
                int maxX = Mathf.CeilToInt(center.x + size.x * 0.5f + 2f);
                int minY = Mathf.FloorToInt(center.y - size.y * 0.5f - 2f);
                int maxY = Mathf.CeilToInt(center.y + size.y * 0.5f + 2f);

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float distance = RoundedRectDistance(x + 0.5f, y + 0.5f, center, size, radius);
                        float coverage = Mathf.Clamp01(0.5f - distance);

                        if (coverage > 0f)
                        {
                            Blend(x, y, new Color(color.r, color.g, color.b, color.a * coverage));
                        }
                    }
                }
            }

            public void StrokeRoundedRect(Vector2 center, Vector2 size, float radius, float thickness, Color color)
            {
                int minX = Mathf.FloorToInt(center.x - size.x * 0.5f - 2f);
                int maxX = Mathf.CeilToInt(center.x + size.x * 0.5f + 2f);
                int minY = Mathf.FloorToInt(center.y - size.y * 0.5f - 2f);
                int maxY = Mathf.CeilToInt(center.y + size.y * 0.5f + 2f);

                float innerRadius = Mathf.Max(0f, radius - thickness);
                Vector2 innerSize = new Vector2(Mathf.Max(0f, size.x - thickness * 2f), Mathf.Max(0f, size.y - thickness * 2f));

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float outerDistance = RoundedRectDistance(x + 0.5f, y + 0.5f, center, size, radius);
                        float innerDistance = RoundedRectDistance(x + 0.5f, y + 0.5f, center, innerSize, innerRadius);
                        float coverage = Mathf.Clamp01(0.5f - outerDistance) - Mathf.Clamp01(0.5f - innerDistance);

                        if (coverage > 0f)
                        {
                            Blend(x, y, new Color(color.r, color.g, color.b, color.a * coverage));
                        }
                    }
                }
            }

            private static float RoundedRectDistance(float px, float py, Vector2 center, Vector2 size, float radius)
            {
                Vector2 halfSize = size * 0.5f - Vector2.one * radius;
                Vector2 delta = new Vector2(Mathf.Abs(px - center.x), Mathf.Abs(py - center.y)) - halfSize;
                Vector2 outside = new Vector2(Mathf.Max(delta.x, 0f), Mathf.Max(delta.y, 0f));
                return outside.magnitude + Mathf.Min(Mathf.Max(delta.x, delta.y), 0f) - radius;
            }

            public void Glow(Vector2 center, float radius, Color color, float falloff)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
                int maxX = Mathf.Min(Width - 1, Mathf.CeilToInt(center.x + radius));
                int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
                int maxY = Mathf.Min(Height - 1, Mathf.CeilToInt(center.y + radius));

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;

                        if (distance >= 1f)
                        {
                            continue;
                        }

                        float alpha = Mathf.Pow(1f - distance, falloff) * color.a;
                        Blend(x, y, new Color(color.r, color.g, color.b, alpha));
                    }
                }
            }

            public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
            {
                float distance = Vector2.Distance(start, end);
                int steps = Mathf.Max(1, Mathf.CeilToInt(distance * 1.5f));

                for (int i = 0; i <= steps; i++)
                {
                    Vector2 point = Vector2.Lerp(start, end, (float)i / steps);
                    FillCircle(point, thickness * 0.5f, color);
                }
            }

            public void FillTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.x, Mathf.Min(b.x, c.x))));
                int maxX = Mathf.Min(Width - 1, Mathf.CeilToInt(Mathf.Max(a.x, Mathf.Max(b.x, c.x))));
                int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.y, Mathf.Min(b.y, c.y))));
                int maxY = Mathf.Min(Height - 1, Mathf.CeilToInt(Mathf.Max(a.y, Mathf.Max(b.y, c.y))));

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (PointInTriangle(new Vector2(x + 0.5f, y + 0.5f), a, b, c))
                        {
                            Blend(x, y, color);
                        }
                    }
                }
            }

            public void FillPolygon(Vector2[] vertices, Color color)
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minY = float.MaxValue;
                float maxY = float.MinValue;

                foreach (Vector2 vertex in vertices)
                {
                    minX = Mathf.Min(minX, vertex.x);
                    maxX = Mathf.Max(maxX, vertex.x);
                    minY = Mathf.Min(minY, vertex.y);
                    maxY = Mathf.Max(maxY, vertex.y);
                }

                int startX = Mathf.Max(0, Mathf.FloorToInt(minX));
                int endX = Mathf.Min(Width - 1, Mathf.CeilToInt(maxX));
                int startY = Mathf.Max(0, Mathf.FloorToInt(minY));
                int endY = Mathf.Min(Height - 1, Mathf.CeilToInt(maxY));

                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        if (PointInPolygon(new Vector2(x + 0.5f, y + 0.5f), vertices))
                        {
                            Blend(x, y, color);
                        }
                    }
                }
            }

            private static bool PointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
            {
                float denominator = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
                if (Mathf.Abs(denominator) < 0.00001f)
                {
                    return false;
                }

                float first = ((b.y - c.y) * (point.x - c.x) + (c.x - b.x) * (point.y - c.y)) / denominator;
                float second = ((c.y - a.y) * (point.x - c.x) + (a.x - c.x) * (point.y - c.y)) / denominator;
                float third = 1f - first - second;
                return first >= 0f && second >= 0f && third >= 0f;
            }

            private static bool PointInPolygon(Vector2 point, Vector2[] vertices)
            {
                bool inside = false;

                for (int i = 0, j = vertices.Length - 1; i < vertices.Length; j = i++)
                {
                    Vector2 current = vertices[i];
                    Vector2 previous = vertices[j];

                    bool intersects = current.y > point.y != previous.y > point.y
                                      && point.x < (previous.x - current.x) * (point.y - current.y) / (previous.y - current.y) + current.x;

                    if (intersects)
                    {
                        inside = !inside;
                    }
                }

                return inside;
            }

            public void AddStars(int count, int seed, float maximumSize, Color color)
            {
                System.Random random = new System.Random(seed);

                for (int i = 0; i < count; i++)
                {
                    float x = (float)random.NextDouble() * Width;
                    float y = (float)random.NextDouble() * Height;
                    float size = 0.8f + (float)random.NextDouble() * maximumSize;

                    Glow(new Vector2(x, y), size * 4.5f, new Color(color.r, color.g, color.b, color.a * 0.35f), 2.4f);
                    FillCircle(new Vector2(x, y), size, color);
                }
            }

            public void Vignette(float strength)
            {
                Vector2 center = new Vector2(Width * 0.5f, Height * 0.5f);
                float maximumDistance = center.magnitude;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), center) / maximumDistance;
                        float factor = 1f - Mathf.SmoothStep(0.35f, 1f, distance) * strength;
                        int index = y * Width + x;
                        Color32 pixel = Pixels[index];
                        pixel.r = (byte)(pixel.r * factor);
                        pixel.g = (byte)(pixel.g * factor);
                        pixel.b = (byte)(pixel.b * factor);
                        Pixels[index] = pixel;
                    }
                }
            }
        }
    }
}
