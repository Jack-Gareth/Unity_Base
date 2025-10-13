using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float coyoteTime = 0.05f; // time after leaving ground when you can still jump

    [SerializeField] private float jumpBufferTime = 0.05f; // time before hitting ground when a jump input still counts

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        if (groundedNow)
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            // Consider within coyote time
            isGrounded = Time.time - lastGroundedTime <= coyoteTime;
        }

        if (isGrounded && Time.time - lastJumpPressedTime <= jumpBufferTime)
        {
            PerformJump();
        }
    }

    public void TryJump()
    {
        // Record the jump press time (buffer start)
        lastJumpPressedTime = Time.time;

        // If grounded or in coyote time, jump immediately
        if (isGrounded)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        lastJumpPressedTime = -999f; // reset buffer so it doesn’t trigger again
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
