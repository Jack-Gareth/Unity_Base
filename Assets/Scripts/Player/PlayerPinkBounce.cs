using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPinkBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private LayerMask surfacesMask;
    [SerializeField] private LayerMask nonMechanicLayer;
    [SerializeField] private float minVerticalDotProduct = 0.7f;

    private Rigidbody2D rb;
    private bool hasBouncedThisCollision;
    private bool hasJumpedInPinkMode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorChanged += HandleColorChanged;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorChanged -= HandleColorChanged;
    }

    private void HandleColorChanged(LevelColor newColor)
    {
        if (newColor != LevelColor.Pink)
            hasJumpedInPinkMode = false;
    }

    public void NotifyJumpPerformed()
    {
        if (LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor == LevelColor.Pink)
            hasJumpedInPinkMode = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;

        // Ignore collisions with non-mechanic surfaces
        if (((1 << layer) & nonMechanicLayer) != 0)
        {
            hasJumpedInPinkMode = false;
            return;
        }

        // Only bounce if currently pink
        if (LevelColorManager.Instance == null || LevelColorManager.Instance.CurrentColor != LevelColor.Pink)
            return;

        // Require a jump before first bounce
        if (!hasJumpedInPinkMode)
            return;

        // Check if this collision is with valid surfaces
        if (((1 << layer) & surfacesMask) == 0)
            return;

        // Bounce only if collision is vertical (not wall)
        if (!IsVerticalCollision(collision))
            return;

        // Apply bounce once per collision
        if (!hasBouncedThisCollision)
        {
            ApplyBounce();
            hasBouncedThisCollision = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & surfacesMask) != 0)
            hasBouncedThisCollision = false;
    }

    private bool IsVerticalCollision(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            float dot = Mathf.Abs(Vector2.Dot(contact.normal, Vector2.up));
            if (dot >= minVerticalDotProduct)
                return true;
        }
        return false;
    }

    private void ApplyBounce()
    {
        float direction = rb.gravityScale < 0 ? -1f : 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce * direction);
    }
}
