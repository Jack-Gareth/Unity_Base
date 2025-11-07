using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class FixBrokenSceneReferences : EditorWindow
{
    [MenuItem("Tools/Fix Broken Scene References")]
    public static void FixReferences()
    {
        string scenePath = "Assets/Scenes/Level 1.unity";
        string fullPath = Path.Combine(Application.dataPath, "..", scenePath);
        
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Scene file not found at: {fullPath}");
            return;
        }

        string backupPath = fullPath + ".broken_backup";
        File.Copy(fullPath, backupPath, true);
        Debug.Log($"Backup created at: {backupPath}");

        string content = File.ReadAllText(fullPath);
        
        content = content.Replace("  - {fileID: 1660991431}\n", "");
        
        File.WriteAllText(fullPath, content);
        
        AssetDatabase.Refresh();
        
        Debug.Log("Fixed broken scene references! Please reload the scene.");
        EditorUtility.DisplayDialog("Success", "Fixed broken scene references!\n\nPlease reload Level 1 scene.", "OK");
    }
}
