using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class SceneMergeConflictResolver : EditorWindow
{
    private string sceneFilePath = "Assets/Scenes/Level 1.unity";
    private bool keepNewerVersion = true;

    [MenuItem("Tools/Fix Scene Merge Conflicts")]
    public static void ShowWindow()
    {
        GetWindow<SceneMergeConflictResolver>("Merge Conflict Resolver");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Merge Conflict Resolver", EditorStyles.boldLabel);
        GUILayout.Space(10);

        sceneFilePath = EditorGUILayout.TextField("Scene File Path:", sceneFilePath);
        
        GUILayout.Space(10);
        keepNewerVersion = EditorGUILayout.Toggle("Keep Newer Version (Incoming)", keepNewerVersion);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Fix Merge Conflicts", GUILayout.Height(40)))
        {
            FixMergeConflicts();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "This will automatically resolve merge conflicts in the scene file.\n\n" +
            "- Keep Newer Version (checked): Keeps changes after '=======' marker\n" +
            "- Keep Older Version (unchecked): Keeps changes before '=======' marker\n\n" +
            "A backup will be created automatically.",
            MessageType.Info
        );
    }

    private void FixMergeConflicts()
    {
        string fullPath = Path.Combine(Application.dataPath, "..", sceneFilePath);
        
        if (!File.Exists(fullPath))
        {
            EditorUtility.DisplayDialog("Error", $"Scene file not found at: {fullPath}", "OK");
            return;
        }

        string backupPath = fullPath + ".backup";
        File.Copy(fullPath, backupPath, true);
        Debug.Log($"Backup created at: {backupPath}");

        string content = File.ReadAllText(fullPath);
        
        int conflictCount = Regex.Matches(content, @"<<<<<<<").Count;
        
        if (conflictCount == 0)
        {
            EditorUtility.DisplayDialog("No Conflicts", "No merge conflicts found in the scene file.", "OK");
            return;
        }

        string pattern = @"<<<<<<< \.merge_file_\w+\r?\n(.*?)\r?\n=======\r?\n(.*?)\r?\n>>>>>>> \.merge_file_\w+\r?\n";
        
        if (keepNewerVersion)
        {
            content = Regex.Replace(content, pattern, "$2\n", RegexOptions.Singleline);
        }
        else
        {
            content = Regex.Replace(content, pattern, "$1\n", RegexOptions.Singleline);
        }

        File.WriteAllText(fullPath, content);
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "Success", 
            $"Fixed {conflictCount} merge conflict(s)!\n\n" +
            $"Backup saved to: {backupPath}\n\n" +
            "Please reload the scene.", 
            "OK"
        );
        
        Debug.Log($"Merge conflicts resolved. {conflictCount} conflict(s) fixed.");
    }
}
