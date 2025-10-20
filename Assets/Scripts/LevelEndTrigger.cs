using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelCompleteUI levelCompleteUI = FindObjectOfType<LevelCompleteUI>();
            
            if (levelCompleteUI != null)
            {
                levelCompleteUI.ShowLevelComplete();
            }
            else
            {
                Debug.LogWarning("LevelCompleteUI not found in the scene!");
            }
        }
    }
}
