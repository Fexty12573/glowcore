using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public static void BuildWebGL()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            Debug.LogError("BUILD SCENE: " + scene.path);
        }
        Build(new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "build/WebGL",
            target = BuildTarget.WebGL
        });
    }

    public static void BuildWindows()
    {
        Build(new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "build/StandaloneWindows64",
            target = BuildTarget.StandaloneWindows64
        });
    }

    public static void BuildLinux()
    {
        Build(new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "build/StandaloneLinux64",
            target = BuildTarget.StandaloneLinux64
        });
    }

    private static string[] GetScenes()
    {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }

    private static void Build(BuildPlayerOptions options)
    {
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
