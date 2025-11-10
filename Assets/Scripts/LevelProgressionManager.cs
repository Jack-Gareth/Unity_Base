using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager : MonoBehaviour
{
    public static LevelProgressionManager Instance { get; private set; }

    [Header("Level Configuration")]
    [SerializeField] private string[] levelNames = { "Level 1", "Level 2" };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int currentIndex = GetLevelIndex(currentScene.name);

        if (currentIndex == -1)
        {
            Debug.LogWarning($"Current scene '{currentScene.name}' is not in the level list!");
            return;
        }

        int nextIndex = (currentIndex + 1) % levelNames.Length;
        string nextLevelName = levelNames[nextIndex];

        if (SwipeTransition.Instance != null)
        {
            SwipeTransition.Instance.TransitionToScene(nextLevelName);
        }
        else
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void RestartCurrentLevel()
    {
        if (SwipeTransition.Instance != null)
        {
            SwipeTransition.Instance.RestartCurrentScene();
        }
        else
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelNames.Length)
        {
            Debug.LogWarning($"Level index {index} is out of range!");
            return;
        }

        string levelName = levelNames[index];

        if (SwipeTransition.Instance != null)
        {
            SwipeTransition.Instance.TransitionToScene(levelName);
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }

    public void LoadLevel(string levelName)
    {
        if (SwipeTransition.Instance != null)
        {
            SwipeTransition.Instance.TransitionToScene(levelName);
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }

    private int GetLevelIndex(string sceneName)
    {
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetCurrentLevelIndex()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return GetLevelIndex(currentScene.name);
    }

    public int GetTotalLevels()
    {
        return levelNames.Length;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
