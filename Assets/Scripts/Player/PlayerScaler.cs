using UnityEngine;

public class PlayerScaler : MonoBehaviour
{
    private const float SHRINK_SCALE_FACTOR = 0.5f;
    
    private Vector3 defaultScale;
    private Vector3 shrunkScale;
    private bool isShrunken = false;
    private LevelColor lastColorWhenShrunken;
    
    private void Awake()
    {
        defaultScale = transform.localScale;
        shrunkScale = defaultScale * SHRINK_SCALE_FACTOR;
        Debug.Log($"PlayerScaler: Awake - Default scale: {defaultScale}, Shrunk scale: {shrunkScale}");
    }
    
    private void Start()
    {
        TrySubscribeToInputManager();
    }
    
    private void OnEnable()
    {
        TrySubscribeToInputManager();
    }
    
    private void TrySubscribeToInputManager()
    {
        Debug.Log("PlayerScaler: TrySubscribeToInputManager called. InputManager.Instance = " + (InputManager.Instance != null));
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            InputManager.Instance.OnRedColorInput -= OnOtherColorPressed;
            InputManager.Instance.OnBlueColorInput -= OnOtherColorPressed;
            
            InputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            InputManager.Instance.OnRedColorInput += OnOtherColorPressed;
            InputManager.Instance.OnBlueColorInput += OnOtherColorPressed;
            Debug.Log("PlayerScaler: Successfully subscribed to input events");
        }
        else
        {
            Debug.LogWarning("PlayerScaler: InputManager.Instance is null, cannot subscribe to events");
        }
    }
    
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            InputManager.Instance.OnRedColorInput -= OnOtherColorPressed;
            InputManager.Instance.OnBlueColorInput -= OnOtherColorPressed;
        }
    }
    
    private void OnGreenColorPressed()
    {
        Debug.Log("PlayerScaler: OnGreenColorPressed called. IsShrunken = " + isShrunken);
        if (!isShrunken)
        {
            ShrinkPlayer();
            lastColorWhenShrunken = LevelColor.Green;
        }
        else
        {
            ResizeToDefault();
        }
    }
    
    private void OnOtherColorPressed()
    {
        if (isShrunken)
        {
            ResizeToDefault();
        }
    }
    
    private void ShrinkPlayer()
    {
        Debug.Log($"PlayerScaler: Shrinking player from {transform.localScale} to {shrunkScale}");
        transform.localScale = shrunkScale;
        isShrunken = true;
    }
    
    private void ResizeToDefault()
    {
        Debug.Log($"PlayerScaler: Resizing player from {transform.localScale} to {defaultScale}");
        transform.localScale = defaultScale;
        isShrunken = false;
    }

    public void ResetSize()
    {
        if (isShrunken)
        {
            ResizeToDefault();
        }
    }
}
