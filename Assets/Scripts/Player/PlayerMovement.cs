using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask surfacesMask;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerWallClimb wallClimb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wallClimb = GetComponent<PlayerWallClimb>();
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void SetMoveInput(Vector2 input) => moveInput = input;

    private void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, surfacesMask);
        }
    }

    private void FixedUpdate()
    {
        if (wallClimb != null && wallClimb.JustWallJumped)
            return;

        bool hasHorizontalInput = Mathf.Abs(moveInput.x) > 0.01f;

        if (hasHorizontalInput || isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }
}
