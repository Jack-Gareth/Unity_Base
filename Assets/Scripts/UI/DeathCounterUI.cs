using UnityEngine;
using TMPro;

public class DeathCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text deathCounterText;

    private void OnEnable()
    {
        DeathCounter.OnDeathCountChanged += UpdateUI;
    }

    private void OnDisable()
    {
        DeathCounter.OnDeathCountChanged -= UpdateUI;
    }

    private void Start()
    {
        deathCounterText.text = "Deaths: " + DeathCounter.Instance.DeathCount;
    }

    private void UpdateUI(int newCount)
    {
        deathCounterText.text = "Deaths: " + newCount;
    }
}
