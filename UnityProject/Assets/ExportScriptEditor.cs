#if (UNITY_EDITOR)
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExportRootScript))]
public class ExportScriptEditor : Editor
{
    [CanBeNull] private ExportRootScript _baseScript;

    private void Awake()
    {
        _baseScript = (ExportRootScript) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Export asset bundle"))
            DoExport();
    }

    private void DoExport()
    {
        if (_baseScript == null)
            return;

        Debug.ClearDeveloperConsole();
        
        try
        {
            var savePath = EditorUtility.SaveFilePanel("Export UAE Bundle", null, "extras",
                "unitypackage");

            var selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (string.IsNullOrEmpty(savePath))
                return;

            if (File.Exists(savePath))
                File.Delete(savePath);

            var fileName = Path.GetFileName(savePath);
            var folderPath = Path.GetDirectoryName(savePath);

            // Convert export object root to prefab asset
            var prefabPath = "Assets/export.prefab";
            PrefabUtility.SaveAsPrefabAsset(_baseScript.gameObject, prefabPath);

            // Execute build
            BuildPipeline.BuildAssetBundles(folderPath,
                new[] {new AssetBundleBuild {assetBundleName = fileName, assetNames = new[] {prefabPath}}},
                BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
            EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);

            // Report result
            if (File.Exists(savePath))
                EditorUtility.DisplayDialog("Export", $"Bundle exported successfully:\r\n{savePath}", "OK");
            else
                EditorUtility.DisplayDialog("Export", $"Bundle export failed (file not created?)", "OK");

            // Cleanup
            AssetDatabase.DeleteAsset(prefabPath);
            AssetDatabase.Refresh();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            EditorUtility.DisplayDialog("Export", $"Bundle export failed!\r\n\r\n{ex}", "OK");
        }
    }
}
#endif