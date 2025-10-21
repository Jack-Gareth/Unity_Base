using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TextMeshProUGUI completeText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float disablePlayerDelay = 0.5f;

    private bool isLevelComplete = false;

    private void Start()
    {
        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Complete Panel is not assigned in the inspector!");
        }

        if (completeText != null)
        {
            completeText.text = "Finished";
        }
    }

    public void ShowLevelComplete()
    {
        if (isLevelComplete) return;

        isLevelComplete = true;

        if (completePanel != null)
        {
            completePanel.SetActive(true);
        }

        StartCoroutine(DisablePlayerAndReset());
    }

    private IEnumerator DisablePlayerAndReset()
    {
        yield return new WaitForSeconds(disablePlayerDelay);

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }

        yield return new WaitForSeconds(displayDuration - disablePlayerDelay);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
