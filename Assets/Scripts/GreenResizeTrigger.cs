using UnityEngine;

public class GreenResizeTrigger : MonoBehaviour
{
    [Header("Trigger Mode")]
    [SerializeField] private bool isExitTrigger = false;
    
    private bool hasBeenUsed = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerScaler scaler = other.GetComponent<PlayerScaler>();
        if (scaler == null)
            return;

        if (isExitTrigger)
        {
            HandleExitTrigger(scaler);
        }
        else
        {
            HandleNormalTrigger(scaler, true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isExitTrigger)
            return;

        PlayerScaler scaler = other.GetComponent<PlayerScaler>();
        if (scaler != null)
        {
            HandleNormalTrigger(scaler, false);
        }
    }
    
    private void HandleNormalTrigger(PlayerScaler scaler, bool entering)
    {
        scaler.SetInResizeTrigger(entering);
        Debug.Log($"GreenResizeTrigger: Player {(entering ? "entered" : "exited")} resize trigger zone");
    }
    
    private void HandleExitTrigger(PlayerScaler scaler)
    {
        if (hasBeenUsed)
        {
            Debug.Log("GreenResizeTrigger Exit: Already used, ignoring");
            return;
        }
        
        if (LevelColorManager.Instance == null || LevelColorManager.Instance.CurrentColor != LevelColor.Green)
        {
            Debug.Log("GreenResizeTrigger Exit: Not in green mode, ignoring");
            return;
        }
        
        if (!scaler.IsShrunken)
        {
            Debug.Log("GreenResizeTrigger Exit: Player is already normal size, ignoring");
            return;
        }
        
        scaler.ForceResizeToDefault();
        hasBeenUsed = true;
        Debug.Log("GreenResizeTrigger Exit: Auto-resized player to default size and disabled trigger");
    }
    
    private void OnEnable()
    {
        hasBeenUsed = false;
    }
}
