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
        Debug.Log("PlayerScaler: TrySubscribeToInputManager called. InputManager.Instance = " + (GameInputManager.Instance != null));
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            GameInputManager.Instance.OnRedColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnBlueColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnYellowColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnPinkColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnBrownColorInput -= OnOtherColorPressed;
            
            GameInputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            GameInputManager.Instance.OnRedColorInput += OnOtherColorPressed;
            GameInputManager.Instance.OnBlueColorInput += OnOtherColorPressed;
            GameInputManager.Instance.OnYellowColorInput += OnOtherColorPressed;
            GameInputManager.Instance.OnPinkColorInput += OnOtherColorPressed;
            GameInputManager.Instance.OnBrownColorInput += OnOtherColorPressed;
            Debug.Log("PlayerScaler: Successfully subscribed to input events");
        }
        else
        {
            Debug.LogWarning("PlayerScaler: InputManager.Instance is null, cannot subscribe to events");
        }
    }
    
    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            GameInputManager.Instance.OnRedColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnBlueColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnYellowColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnPinkColorInput -= OnOtherColorPressed;
            GameInputManager.Instance.OnBrownColorInput -= OnOtherColorPressed;
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
