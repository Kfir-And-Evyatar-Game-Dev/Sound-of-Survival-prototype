using System;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class WarehouseWebBuild
{
    [MenuItem("Build/Build Warehouse Web")]
    public static void Build()
    {
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Warehouse.unity" },
            locationPathName = "Builds/Web",
            target = BuildTarget.WebGL
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException($"Web build ended with {report.summary.result}.");
    }
}
