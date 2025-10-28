using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelCompleteUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private Image[] diamondIcons;

    [Header("Diamond Colors")]
    [SerializeField] private Color collectedColor = Color.yellow;
    [SerializeField] private Color uncollectedColor = Color.gray;

    [Header("Settings")]
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
        Debug.Log("ShowLevelComplete called!");
        
        if (isLevelComplete) return;

        isLevelComplete = true;

        if (completePanel != null)
        {
            completePanel.SetActive(true);
        }

        UpdateDiamondDisplay();
        StartCoroutine(DisablePlayer());
    }

    private void UpdateDiamondDisplay()
    {
        if (ItemCollectionManager.Instance == null)
        {
            Debug.LogWarning("ItemCollectionManager.Instance is null!");
            return;
        }

        if (diamondIcons == null || diamondIcons.Length == 0)
        {
            Debug.LogWarning("Diamond icons array is null or empty!");
            return;
        }

        int collected = ItemCollectionManager.Instance.ItemsCollected;
        Debug.Log($"Updating diamonds: {collected} collected out of {ItemCollectionManager.Instance.TotalItems}");

        for (int i = 0; i < diamondIcons.Length; i++)
        {
            if (diamondIcons[i] != null)
            {
                diamondIcons[i].color = (i < collected) ? collectedColor : uncollectedColor;
                Debug.Log($"Diamond {i}: Color set to {((i < collected) ? "Collected" : "Uncollected")}");
            }
            else
            {
                Debug.LogWarning($"Diamond icon {i} is null!");
            }
        }
    }

    private IEnumerator DisablePlayer()
    {
        yield return new WaitForSeconds(disablePlayerDelay);

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }
    }

    public void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
