using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NeonBreaker.EditorTools
{
    public static class BuildScript
    {
        [MenuItem("Neon Breaker/Build Windows Player")]
        public static void BuildWindows()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string buildFolder = Path.Combine(projectRoot, "Build");
            string sceneFile = Path.Combine(projectRoot, "Assets/Scenes/Main.unity");

            if (!File.Exists(sceneFile))
            {
                Debug.LogError("Main scene is missing. Run Neon Breaker > Setup Project first.");
                EditorApplication.Exit(1);
                return;
            }

            Directory.CreateDirectory(buildFolder);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = Path.Combine(buildFolder, "NeonBreaker.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.outputPath + " (" + summary.totalSize / 1048576 + " MB)");
            }
            else
            {
                Debug.LogError("Build failed: " + summary.result + " with " + summary.totalErrors + " errors.");
                EditorApplication.Exit(1);
            }
        }
    }
}
