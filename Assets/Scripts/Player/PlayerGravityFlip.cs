using UnityEngine;

public class PlayerGravityFlip : MonoBehaviour
{
    private const float NORMAL_GRAVITY = 1f;
    
    [Header("Gravity Flip Settings")]
    [SerializeField] private float delayedGravityAction = 0.3f;
    [SerializeField] private float flippedGravitySpeed = 1f;
    
    private Rigidbody2D rb;
    private bool isGravityFlipped = false;
    private bool isTransitioning = false;
    private Coroutine flipCoroutine;
    private float expectedGravityScale;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = NORMAL_GRAVITY;
        expectedGravityScale = NORMAL_GRAVITY;
    }

    private void Update()
    {
        if (!isTransitioning && isGravityFlipped == false)
        {
            if (Mathf.Abs(rb.gravityScale - expectedGravityScale) > 0.01f)
            {
                Debug.LogWarning($"Gravity scale mismatch detected! Expected: {expectedGravityScale}, Actual: {rb.gravityScale}. Correcting...");
                rb.gravityScale = expectedGravityScale;
            }
        }
    }

    private void OnValidate()
    {
        if (rb != null && !isGravityFlipped && !isTransitioning)
        {
            rb.gravityScale = NORMAL_GRAVITY;
            expectedGravityScale = NORMAL_GRAVITY;
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("PlayerGravityFlip: OnEnable called. GameInputManager.Instance = " + (GameInputManager.Instance != null));
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnGravityFlipInput -= OnGravityFlipPressed;
            GameInputManager.Instance.OnGravityFlipInput += OnGravityFlipPressed;
            
            GameInputManager.Instance.OnRedColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnBlueColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnGreenColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnYellowColorInput -= OnYellowColorPressed;
            GameInputManager.Instance.OnPinkColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnBrownColorInput -= OnNonYellowColorPressed;
            
            GameInputManager.Instance.OnRedColorInput += OnNonYellowColorPressed;
            GameInputManager.Instance.OnBlueColorInput += OnNonYellowColorPressed;
            GameInputManager.Instance.OnGreenColorInput += OnNonYellowColorPressed;
            GameInputManager.Instance.OnYellowColorInput += OnYellowColorPressed;
            GameInputManager.Instance.OnPinkColorInput += OnNonYellowColorPressed;
            GameInputManager.Instance.OnBrownColorInput += OnNonYellowColorPressed;
            
            Debug.Log("PlayerGravityFlip: Successfully subscribed to OnGravityFlipInput");
        }
    }
    
    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnGravityFlipInput -= OnGravityFlipPressed;
            
            GameInputManager.Instance.OnRedColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnBlueColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnGreenColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnYellowColorInput -= OnYellowColorPressed;
            GameInputManager.Instance.OnPinkColorInput -= OnNonYellowColorPressed;
            GameInputManager.Instance.OnBrownColorInput -= OnNonYellowColorPressed;
        }
    }
    
    private void OnGravityFlipPressed()
    {
        Debug.Log("PlayerGravityFlip: OnGravityFlipPressed called. CurrentColor = " + LevelColorManager.Instance.CurrentColor);
        if (LevelColorManager.Instance.CurrentColor == LevelColor.Yellow)
        {
            FlipGravity();
        }
        else
        {
            Debug.Log("PlayerGravityFlip: Cannot flip gravity - current color is not Yellow");
        }
    }
    
    private void OnYellowColorPressed()
    {
        if (LevelColorManager.Instance.CurrentColor == LevelColor.Yellow && isGravityFlipped)
        {
            Debug.Log("PlayerGravityFlip: Toggling off Yellow while gravity flipped, resetting gravity");
            ResetGravity();
        }
    }
    
    private void OnNonYellowColorPressed()
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
        }

        isGravityFlipped = false;
        isTransitioning = false;
        rb.gravityScale = NORMAL_GRAVITY;
        expectedGravityScale = NORMAL_GRAVITY;
        
        Debug.Log("PlayerGravityFlip: Switching away from Yellow, resetting gravity to 1");
    }
    
    private void FlipGravity()
    {
        if (isTransitioning)
        {
            return;
        }
        
        isGravityFlipped = !isGravityFlipped;
        
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
        }
        
        if (isGravityFlipped)
        {
            Debug.Log("PlayerGravityFlip: Flipping gravity to inverted (falling upward)");
            float targetGravity = -flippedGravitySpeed;
            expectedGravityScale = targetGravity;
            flipCoroutine = StartCoroutine(SmoothGravityTransition(targetGravity));
        }
        else
        {
            Debug.Log("PlayerGravityFlip: Restoring normal gravity (falling downward)");
            float targetGravity = flippedGravitySpeed;
            expectedGravityScale = targetGravity;
            flipCoroutine = StartCoroutine(SmoothGravityTransition(targetGravity));
        }
    }
    
    private System.Collections.IEnumerator SmoothGravityTransition(float targetGravity)
    {
        isTransitioning = true;
        
        float startGravity = rb.gravityScale;
        float elapsed = 0f;
        
        while (elapsed < delayedGravityAction)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / delayedGravityAction;
            
            rb.gravityScale = Mathf.Lerp(startGravity, targetGravity, t);
            
            yield return null;
        }
        
        rb.gravityScale = targetGravity;
        
        isTransitioning = false;
    }
    
    public void ResetGravity()
    {
        if (isGravityFlipped)
        {
            if (flipCoroutine != null)
            {
                StopCoroutine(flipCoroutine);
            }
            
            isGravityFlipped = false;
            isTransitioning = false;
            rb.gravityScale = NORMAL_GRAVITY;
            expectedGravityScale = NORMAL_GRAVITY;
        }
    }
}
