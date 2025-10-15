using UnityEngine;
using TMPro;

public class DeathCounterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI deathCountText;
    
    [Header("Display Settings")]
    [SerializeField] private string displayFormat = "Deaths: {0}";
    
    private bool isSubscribed = false;
    
    private void Start()
    {
        UpdateDeathCountDisplay(0);
        TrySubscribe();
    }
    
    private void OnEnable()
    {
        TrySubscribe();
    }
    
    private void OnDisable()
    {
        TryUnsubscribe();
    }
    
    private void TrySubscribe()
    {
        if (!isSubscribed)
        {
            DeathCounter instance = FindFirstObjectByType<DeathCounter>();
            if (instance != null)
            {
                instance.OnDeathCountChanged += UpdateDeathCountDisplay;
                isSubscribed = true;
            }
        }
    }
    
    private void TryUnsubscribe()
    {
        if (isSubscribed)
        {
            DeathCounter instance = FindFirstObjectByType<DeathCounter>();
            if (instance != null)
            {
                instance.OnDeathCountChanged -= UpdateDeathCountDisplay;
            }
            isSubscribed = false;
        }
    }
    
    private void UpdateDeathCountDisplay(int deathCount)
    {
        if (deathCountText != null)
        {
            deathCountText.text = string.Format(displayFormat, deathCount);
        }
    }
}
