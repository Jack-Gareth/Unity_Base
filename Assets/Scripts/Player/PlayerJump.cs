using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float coyoteTime = 0.05f; // time after leaving ground when you can still jump

    [SerializeField] private float jumpBufferTime = 0.05f;

    [SerializeField] private float jumpCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float lastJumpTime;
    private bool canJump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastJumpPressedTime = -999f;
        lastGroundedTime = -999f;
        lastJumpTime = -999f;
        canJump = true;
    }


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
            isGrounded = Time.time - lastGroundedTime <= coyoteTime;
        }

        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            canJump = true;
        }

        if (LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor == LevelColor.Yellow)
        {
            return;
        }

        if (isGrounded && canJump && Time.time - lastJumpPressedTime <= jumpBufferTime)
        {
            PerformJump();
        }
    }

    public void TryJump()
    {
        if (LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor == LevelColor.Yellow)
        {
            return;
        }

        lastJumpPressedTime = Time.time;

        if (isGrounded && canJump)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        float jumpDirection = rb.gravityScale < 0 ? -1f : 1f;
        float adjustedJumpForce = jumpForce * jumpDirection;
        
        Debug.Log($"Jump - GravityScale: {rb.gravityScale}, JumpDirection: {jumpDirection}, JumpForce: {jumpForce}, AdjustedJumpForce: {adjustedJumpForce}");
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, adjustedJumpForce);
        isGrounded = false;
        lastJumpPressedTime = -999f;
        canJump = false;
        lastJumpTime = Time.time;
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
