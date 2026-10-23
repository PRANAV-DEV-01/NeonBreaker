using System.IO;
using NeonBreaker.AudioVisual;
using NeonBreaker.Core;
using NeonBreaker.Gameplay;
using NeonBreaker.UI;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace NeonBreaker.EditorTools
{
    public static class ProjectSetup
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("Neon Breaker/Setup Project")]
        public static void SetupAll()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory("Assets/Resources/Art");
            Directory.CreateDirectory("Assets/Resources/Audio");

            ArtGenerator.GenerateAll();
            AudioGenerator.GenerateAll();

            AssetDatabase.Refresh();

            ArtGenerator.ConfigureImports();
            AudioGenerator.ConfigureImports();

            AssetDatabase.Refresh();

            CreateMainScene();
            ConfigurePlayerSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Neon Breaker project setup complete. Scene: " + ScenePath);
        }

        private static void CreateMainScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 9.6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.015f, 0.018f, 0.05f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            cameraObject.AddComponent<AudioListener>();

            GameObject gameRoot = new GameObject("Game");
            gameRoot.AddComponent<GameManager>();

            GameObject managersObject = new GameObject("Managers");
            managersObject.transform.SetParent(gameRoot.transform);
            AddManager<SaveManager>(managersObject.transform, "Save Manager");
            AddManager<BrickManager>(managersObject.transform, "Brick Manager");
            AddManager<BallManager>(managersObject.transform, "Ball Manager");
            AddManager<PowerUpManager>(managersObject.transform, "Power Up Manager");
            AddManager<UIManager>(managersObject.transform, "UI Manager");
            AddManager<AudioManager>(managersObject.transform, "Audio Manager");
            AddManager<VFXManager>(managersObject.transform, "VFX Manager");

            GameObject launcherObject = new GameObject("Launcher");
            launcherObject.transform.SetParent(gameRoot.transform);
            launcherObject.transform.position = new Vector3(0f, -7.35f, 0f);
            launcherObject.AddComponent<Launcher>();

            GameObject boundariesObject = new GameObject("Boundaries");
            boundariesObject.transform.SetParent(gameRoot.transform);
            boundariesObject.AddComponent<BoundaryBuilder>();

            CreateBackground();
            CreateDangerLine();

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();

            EditorSceneManager.SaveScene(scene, ScenePath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }

        private static void AddManager<T>(Transform parent, string objectName) where T : Component
        {
            GameObject managerObject = new GameObject(objectName);
            managerObject.transform.SetParent(parent);
            managerObject.AddComponent<T>();
        }

        private static void CreateBackground()
        {
            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.position = new Vector3(0f, 0f, 10f);

            SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Art/bg_game.png");
            renderer.sortingOrder = -100;

            backgroundObject.AddComponent<BackgroundFitter>();
        }

        private static void CreateDangerLine()
        {
            GameObject dangerObject = new GameObject("Danger Line");
            dangerObject.transform.position = new Vector3(0f, -5.15f, 0f);
            dangerObject.transform.localScale = new Vector3(4.2f, 1f, 1f);

            SpriteRenderer renderer = dangerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Art/danger_line.png");
            renderer.color = new Color(1f, 0.28f, 0.35f, 0.65f);
            renderer.sortingOrder = 1;

            dangerObject.AddComponent<DangerLinePulse>();
        }

        private static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "Neon Breaker";
            PlayerSettings.productName = "Neon Breaker";
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.usePlayerLog = true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.neonbreaker.game");
        }
    }
}
