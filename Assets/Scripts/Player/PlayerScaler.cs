using UnityEngine;

public class PlayerScaler : MonoBehaviour
{
    private const float SHRINK_SCALE_FACTOR = 0.5f;
    
    private Vector3 defaultScale;
    private Vector3 shrunkScale;
    private bool isShrunken = false;
    
    private void Awake()
    {
        defaultScale = transform.localScale;
        shrunkScale = defaultScale * SHRINK_SCALE_FACTOR;
    }
    
    private void OnEnable()
    {
        PlayerEvents.OnGravityFlip += HandleAbilityPressed;
        PlayerEvents.OnColorChanged += HandleColorChange;
    }
    
    private void OnDisable()
    {
        PlayerEvents.OnGravityFlip -= HandleAbilityPressed;
        PlayerEvents.OnColorChanged -= HandleColorChange;
    }
    
    private void HandleAbilityPressed()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Green)
            return;

        if (isShrunken)
        {
            ResizeToDefault();
        }
        else
        {
            ShrinkPlayer();
        }
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Green && isShrunken)
        {
            ResizeToDefault();
        }
    }
    
    private void ShrinkPlayer()
    {
        transform.localScale = shrunkScale;
        isShrunken = true;
    }
    
    private void ResizeToDefault()
    {
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
