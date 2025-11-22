using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelManager", menuName = "Managers/LevelManager", order = 0)]
public class LevelManager : ScriptableObject
{
    public Action OnScreenShow;
    public Action OnScreenHide;
    public Action<Action> StartScreenHide;

    // Called when a level is completed
    public void MarkCurrentLevelComplete()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        SaveManager.MarkLevelComplete(currentLevel);
        Debug.Log($"[LevelManager] Marked {currentLevel} as complete.");
    }

    public void LoadSceneFade(int levelNum)
    {
        StartScreenHide?.Invoke(() => Load(levelNum));
    }

    public void LoadSceneInstant(int levelNum)
    {
        Load(levelNum);
    }

    private void Load(int num)
    {
        OnScreenHide?.Invoke();
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(num);
    }

    public void InvokeScreenVisible()
    {
        OnScreenShow?.Invoke();
    }
}
