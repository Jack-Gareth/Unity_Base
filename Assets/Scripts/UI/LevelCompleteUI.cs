using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : Singleton<LevelCompleteUI>
{
    [Header("UI References")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private UnityEngine.UI.Image[] diamondIcons;

    [Header("Diamond Colors")]
    [SerializeField] private Color collectedColor = Color.yellow;
    [SerializeField] private Color uncollectedColor = Color.gray;

    private bool isLevelComplete = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        TrySubscribeToInput();

        if (completePanel != null)
            completePanel.SetActive(false);

        if (completeText != null)
            completeText.text = "Finished";
    }

    private void OnDisable()
    {
        TryUnsubscribeFromInput();
    }

    private void TrySubscribeToInput()
    {
        if (GameInputManager.Instance != null)
            GameInputManager.Instance.OnConfirmInput += HandleConfirm;
    }

    private void TryUnsubscribeFromInput()
    {
        if (GameInputManager.Instance != null)
            GameInputManager.Instance.OnConfirmInput -= HandleConfirm;
    }

    public void ShowLevelComplete()
    {
        if (isLevelComplete)
            return;

        isLevelComplete = true;

        GameManager.Instance?.DisablePlayerControl();

        if (completePanel != null)
            completePanel.SetActive(true);

        UpdateDiamondDisplay();
    }

    private void HandleConfirm()
    {
        if (isLevelComplete)
            LoadNextLevel();
    }

    private void UpdateDiamondDisplay()
    {
        if (diamondIcons == null || diamondIcons.Length == 0)
            return;

        int collected = GameManager.Instance.ItemsCollected;

        for (int i = 0; i < diamondIcons.Length; i++)
            diamondIcons[i].color = (i < collected) ? collectedColor : uncollectedColor;
    }

    private void LoadNextLevel()
    {
        GameManager.Instance?.EnablePlayerControl();

        if (LevelProgressionManager.Instance != null)
        {
            LevelProgressionManager.Instance.LoadNextLevel();
            return;
        }

        // Fallback reload
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
