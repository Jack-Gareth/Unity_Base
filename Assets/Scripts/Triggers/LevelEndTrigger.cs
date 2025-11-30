using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            triggered = true;
            LevelCompleteUI.Instance?.ShowLevelComplete();
        }
    }
}
