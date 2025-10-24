using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private const float JUMP_FORCE = 15f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float coyoteTime = 0.05f;

    [SerializeField] private float jumpBufferTime = 0.05f;

    [SerializeField] private float jumpCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float lastJumpTime;
    private bool canJump;
    private PlayerPinkBounce pinkBounce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pinkBounce = GetComponent<PlayerPinkBounce>();
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

        if (isGrounded && canJump && Time.time - lastJumpPressedTime <= jumpBufferTime)
        {
            PerformJump();
        }
    }

    public void TryJump()
    {

        lastJumpPressedTime = Time.time;

        if (isGrounded && canJump)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        float jumpDirection = rb.gravityScale < 0 ? -1f : 1f;
        float adjustedJumpForce = JUMP_FORCE * jumpDirection;

        Debug.Log($"Jump - GravityScale: {rb.gravityScale}, JumpDirection: {jumpDirection}, JumpForce: {JUMP_FORCE}, AdjustedJumpForce: {adjustedJumpForce}");

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, adjustedJumpForce);
        isGrounded = false;
        lastJumpPressedTime = -999f;
        canJump = false;
        lastJumpTime = Time.time;

        if (pinkBounce != null)
        {
            pinkBounce.NotifyJumpPerformed();
        }
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
