using UnityEngine;
using System.Collections;

public class PlayerScaler : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] [Range(0.1f, 1f)] private float shrinkScaleFactor = 0.6f;
    [SerializeField] [Range(0.01f, 2f)] private float transitionDuration = 0.3f;
    
    private Vector3 defaultScale;
    private Vector3 shrunkScale;
    private bool isShrunken = false;
    private Coroutine resizeCoroutine;
    private bool isInResizeTrigger = false;
    
    private void Awake()
    {
        defaultScale = transform.localScale;
        shrunkScale = defaultScale * shrinkScaleFactor;
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
        if (LevelColorManager.Instance == null || LevelColorManager.Instance.CurrentColor != LevelColor.Green)
        {
            Debug.Log("PlayerScaler: Not in green mode, cannot resize");
            return;
        }

        if (!isInResizeTrigger)
        {
            Debug.Log("PlayerScaler: Not in a resize trigger zone, cannot resize");
            return;
        }

        ToggleSize();
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Green && isShrunken)
        {
            ResizeToDefault();
        }
    }
    
    public void SetInResizeTrigger(bool inTrigger)
    {
        isInResizeTrigger = inTrigger;
    }
    
    public bool IsShrunken
    {
        get { return isShrunken; }
    }
    
    public void ForceResizeToDefault()
    {
        if (isShrunken)
        {
            ResizeToDefault();
            Debug.Log($"PlayerScaler: Force resizing to DEFAULT (100%) at time {Time.time}");
        }
    }
    
    public void ToggleSize()
    {
        if (isShrunken)
        {
            ResizeToDefault();
            Debug.Log($"PlayerScaler: Resizing to DEFAULT (100%) at time {Time.time}");
        }
        else
        {
            ShrinkPlayer();
            Debug.Log($"PlayerScaler: Resizing to SMALL ({shrinkScaleFactor * 100}%) at time {Time.time}");
        }
    }
    
    private void ShrinkPlayer()
    {
        if (resizeCoroutine != null)
        {
            StopCoroutine(resizeCoroutine);
        }
        resizeCoroutine = StartCoroutine(ResizeCoroutine(shrunkScale));
        isShrunken = true;
    }
    
    private void ResizeToDefault()
    {
        if (resizeCoroutine != null)
        {
            StopCoroutine(resizeCoroutine);
        }
        resizeCoroutine = StartCoroutine(ResizeCoroutine(defaultScale));
        isShrunken = false;
    }
    
    private IEnumerator ResizeCoroutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
        resizeCoroutine = null;
    }

    public void ResetSize()
    {
        if (isShrunken)
        {
            ResizeToDefault();
        }
    }
}
