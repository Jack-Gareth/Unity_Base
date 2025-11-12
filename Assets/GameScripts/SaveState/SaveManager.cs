using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData CurrentSave { get; private set; } = new SaveData();

    public static void Save()
    {
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"[SaveManager] Saved to {savePath}");
    }

    public static void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CurrentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveManager] Save loaded successfully.");
        }
        else
        {
            CurrentSave = new SaveData();
            Debug.Log("[SaveManager] No save found, created new data.");
        }
    }

    public static void MarkLevelComplete(string levelName)
    {
        if (!CurrentSave.completedLevels.Contains(levelName))
        {
            CurrentSave.completedLevels.Add(levelName);
            Save();
        }
    }

    public static bool IsLevelComplete(string levelName)
    {
        return CurrentSave.completedLevels.Contains(levelName);
    }
}
