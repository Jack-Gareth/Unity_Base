using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager : Singleton<LevelProgressionManager>
{
    [SerializeField] private string[] levelNames = { "Level 1", "Level 2", "Level 3" };

    public void LoadNextLevel()
    {
        int currentIndex = GetCurrentLevelIndex();
        if (currentIndex < 0) return;

        int nextIndex = (currentIndex + 1) % levelNames.Length;
        LoadLevel(levelNames[nextIndex]);
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelNames.Length) return;
        LoadLevel(levelNames[index]);
    }

    public void LoadLevel(string levelName)
    {
        if (SwipeTransition.Instance != null)
            SwipeTransition.Instance.TransitionToScene(levelName);
        else
            SceneManager.LoadScene(levelName);
    }

    public int GetCurrentLevelIndex()
    {
        string currentName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == currentName)
                return i;
        }

        return -1;
    }

    public int GetTotalLevels() => levelNames.Length;
}
