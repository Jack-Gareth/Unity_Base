using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPinkBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private bool hasBouncedThisCollision = false;
    private bool hasJumpedInPinkMode = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnRedColorInput += OnColorChanged;
            GameInputManager.Instance.OnBlueColorInput += OnColorChanged;
            GameInputManager.Instance.OnGreenColorInput += OnColorChanged;
            GameInputManager.Instance.OnYellowColorInput += OnColorChanged;
            GameInputManager.Instance.OnPinkColorInput += OnColorChanged;
            GameInputManager.Instance.OnBrownColorInput += OnColorChanged;
        }
    }
    
    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnRedColorInput -= OnColorChanged;
            GameInputManager.Instance.OnBlueColorInput -= OnColorChanged;
            GameInputManager.Instance.OnGreenColorInput -= OnColorChanged;
            GameInputManager.Instance.OnYellowColorInput -= OnColorChanged;
            GameInputManager.Instance.OnPinkColorInput -= OnColorChanged;
            GameInputManager.Instance.OnBrownColorInput -= OnColorChanged;
        }
    }
    
    private void OnColorChanged()
    {
        if (LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor != LevelColor.Pink)
        {
            hasJumpedInPinkMode = false;
            Debug.Log("PlayerPinkBounce: Reset jump state (switched away from pink)");
        }
    }
    
    public void NotifyJumpPerformed()
    {
        if (LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor == LevelColor.Pink)
        {
            hasJumpedInPinkMode = true;
            Debug.Log("PlayerPinkBounce: First jump in pink mode performed, bouncing will now activate");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"PlayerPinkBounce: Collision detected with {collision.gameObject.name} on layer {collision.gameObject.layer}");
        
        if (LevelColorManager.Instance == null || LevelColorManager.Instance.CurrentColor != LevelColor.Pink)
        {
            Debug.Log("PlayerPinkBounce: Not in pink mode, skipping bounce");
            return;
        }
        
        if (!hasJumpedInPinkMode)
        {
            Debug.Log("PlayerPinkBounce: Haven't jumped yet in pink mode, skipping bounce");
            return;
        }
        
        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
        {
            Debug.Log($"PlayerPinkBounce: Object layer {collision.gameObject.layer} doesn't match ground layer mask {groundLayer.value}");
            return;
        }
        
        if (!hasBouncedThisCollision)
        {
            Debug.Log("PlayerPinkBounce: All conditions met, applying bounce!");
            ApplyBounce();
            hasBouncedThisCollision = true;
        }
        else
        {
            Debug.Log("PlayerPinkBounce: Already bounced this collision");
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            hasBouncedThisCollision = false;
        }
    }
    
    private void ApplyBounce()
    {
        float bounceDirection = rb.gravityScale < 0 ? -1f : 1f;
        float adjustedBounceForce = bounceForce * bounceDirection;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, adjustedBounceForce);
        
        Debug.Log($"Pink Bounce Applied! Force: {adjustedBounceForce}");
    }
}
