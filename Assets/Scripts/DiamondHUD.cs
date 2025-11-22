using UnityEngine;
using UnityEngine.UI;

public class DiamondHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] diamondIcons;

    [Header("Diamond Colors")]
    [SerializeField] private Color collectedColor = Color.yellow;
    [SerializeField] private Color uncollectedColor = Color.gray;

    private int lastCollectedCount = -1;

    private void Start()
    {
        UpdateDiamondDisplay();
    }

    private void Update()
    {
        if (ItemCollectionManager.Instance != null)
        {
            int currentCollected = ItemCollectionManager.Instance.ItemsCollected;
            
            if (currentCollected != lastCollectedCount)
            {
                lastCollectedCount = currentCollected;
                UpdateDiamondDisplay();
            }
        }
    }

    private void UpdateDiamondDisplay()
    {
        if (ItemCollectionManager.Instance == null || diamondIcons == null || diamondIcons.Length == 0)
        {
            return;
        }

        int collected = ItemCollectionManager.Instance.ItemsCollected;

        for (int i = 0; i < diamondIcons.Length; i++)
        {
            if (diamondIcons[i] != null)
            {
                diamondIcons[i].color = (i < collected) ? collectedColor : uncollectedColor;
            }
        }
    }
}
