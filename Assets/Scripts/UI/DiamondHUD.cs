using UnityEngine;
using UnityEngine.UI;

public class DiamondHUD : MonoBehaviour
{
    [SerializeField] private Image[] diamondIcons;
    [SerializeField] private Color collectedColor = Color.yellow;
    [SerializeField] private Color uncollectedColor = Color.gray;

    private void OnEnable()
    {
        UIEvents.OnDiamondCountUpdated += UpdateDiamondDisplay;
    }

    private void OnDisable()
    {
        UIEvents.OnDiamondCountUpdated -= UpdateDiamondDisplay;
    }

    private void Start()
    {
            UpdateDiamondDisplay(
                ItemCollectionManager.Instance.ItemsCollected, 
                ItemCollectionManager.Instance.TotalItems);
        
    }

    private void UpdateDiamondDisplay(int collected, int total)
    {
        if (diamondIcons == null || diamondIcons.Length == 0) return;

        for (int i = 0; i < diamondIcons.Length; i++)
        {
            if (diamondIcons[i] != null)
            {
                diamondIcons[i].color = (i < collected) ? collectedColor : uncollectedColor;
            }
        }
    }
}
