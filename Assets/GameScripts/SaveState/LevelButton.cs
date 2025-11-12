using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private Button button;
    [SerializeField] private GameObject completedMarker;

    void Start()
    {
        bool isComplete = SaveManager.IsLevelComplete(levelName);

        if (completedMarker != null)
            completedMarker.SetActive(isComplete);

        button.onClick.AddListener(() => SceneManager.LoadScene(levelName));
    }
}
