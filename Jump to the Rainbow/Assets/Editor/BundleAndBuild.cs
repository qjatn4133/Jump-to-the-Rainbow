using UnityEngine;
using UnityEditor;

public class BundleAndBuild
{
    [MenuItem("Jump to the Rainbow Debug/Build with Bundle/Build")]
    static void Build()
    {
        AssetBundles.BuildScript.BuildStandalonePlayer();
    }

    [MenuItem("Jump to the Rainbow Debug/Build with Bundle/Build and Run")]
    static void BuildAndRun()
    {
        AssetBundles.BuildScript.BuildStandalonePlayer(true);
    }
}
