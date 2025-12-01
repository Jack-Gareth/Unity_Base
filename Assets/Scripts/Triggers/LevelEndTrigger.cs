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
            
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.bodyType = RigidbodyType2D.Static;
            }
            
            LevelCompleteUI.Instance?.ShowLevelComplete();
        }
    }
}
