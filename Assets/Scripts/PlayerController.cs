using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask = 1 << 5; // Ground layer
    
    [Header("Wall Climbing (Blue Color Only)")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask wallLayerMask = (1 << 5) | (1 << 6); // Ground and Wall layers
    [SerializeField] private float wallClimbSpeed = 3f;
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1f);
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallClimbing;
    private Vector2 moveInput;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= OnMoveInput;
            InputManager.Instance.OnJumpInput -= OnJumpInput;
            InputManager.Instance.OnMoveInput += OnMoveInput;
            InputManager.Instance.OnJumpInput += OnJumpInput;
        }
    }
    
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += OnMoveInput;
            InputManager.Instance.OnJumpInput += OnJumpInput;
        }
    }
    
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= OnMoveInput;
            InputManager.Instance.OnJumpInput -= OnJumpInput;
        }
    }
    
    private void Update()
    {
        CheckGroundStatus();
        CheckWallStatus();
        HandleWallClimbing();
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    /// <summary>
    /// Handles horizontal movement of the player
    /// </summary>
    private void HandleMovement()
    {
        // Normal horizontal movement (always allowed unless wall climbing)
        if (!isWallClimbing)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }
    
    /// <summary>
    /// Checks if the player is touching the ground
    /// </summary>
    private void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
    
    /// <summary>
    /// Checks if the player is touching walls (only matters when level is blue)
    /// </summary>
    private void CheckWallStatus()
    {
        if (!CanWallClimb()) 
        {
            isTouchingWallLeft = false;
            isTouchingWallRight = false;
            return;
        }
        
        Vector3 leftCheckPos = wallCheckLeft != null ? wallCheckLeft.position : transform.position;
        Vector3 rightCheckPos = wallCheckRight != null ? wallCheckRight.position : transform.position;
        
        RaycastHit2D leftHit = Physics2D.Raycast(leftCheckPos, Vector2.left, wallCheckDistance, wallLayerMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rightCheckPos, Vector2.right, wallCheckDistance, wallLayerMask);
        
        isTouchingWallLeft = leftHit.collider != null;
        isTouchingWallRight = rightHit.collider != null;
    }
    
    /// <summary>
    /// Handles wall climbing mechanics when level is blue
    /// </summary>
    private void HandleWallClimbing()
    {
        if (!CanWallClimb()) 
        {
            if (isWallClimbing)
            {
                isWallClimbing = false;
            }
            return;
        }
        
        bool isTouchingAnyWall = isTouchingWallLeft || isTouchingWallRight;
        bool pressingUp = moveInput.y > 0;
        
        if (isTouchingAnyWall && pressingUp)
        {
            if (!isWallClimbing)
            {
                isWallClimbing = true;
            }
            
            float climbVelocity = moveInput.y * wallClimbSpeed;
            rb.linearVelocity = new Vector2(0f, climbVelocity);
        }
        else
        {
            if (isWallClimbing)
            {
                isWallClimbing = false;
            }
        }
    }
    
    /// <summary>
    /// Checks if wall climbing is allowed based on current level color
    /// </summary>
    private bool CanWallClimb()
    {
        return LevelColorManager.Instance != null && LevelColorManager.Instance.CurrentColor == LevelColor.Blue;
    }
    
    /// <summary>
    /// Handles player jump when grounded or wall jumping when on walls (blue level only)
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (isWallClimbing && CanWallClimb())
        {
            Vector2 jumpDirection = wallJumpDirection.normalized;
            
            if (isTouchingWallLeft)
            {
                rb.linearVelocity = new Vector2(jumpDirection.x * wallJumpForce, jumpDirection.y * wallJumpForce);
            }
            else if (isTouchingWallRight)
            {
                rb.linearVelocity = new Vector2(-jumpDirection.x * wallJumpForce, jumpDirection.y * wallJumpForce);
            }
            
            isWallClimbing = false;
        }
    }
    
    private void OnMoveInput(Vector2 input)
    {
        moveInput = input;
    }
    
    private void OnJumpInput()
    {
        Jump();
    }
    
    private void OnDrawGizmosSelected()
    {
        // Ground check visualization
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Wall check visualization (only when level is blue or in editor)
        if (CanWallClimb() || !Application.isPlaying)
        {
            // Left wall check
            Vector3 leftCheckPos = wallCheckLeft != null ? wallCheckLeft.position : transform.position;
            Gizmos.color = isTouchingWallLeft ? Color.blue : Color.cyan;
            Gizmos.DrawRay(leftCheckPos, Vector2.left * wallCheckDistance);
            
            // Right wall check
            Vector3 rightCheckPos = wallCheckRight != null ? wallCheckRight.position : transform.position;
            Gizmos.color = isTouchingWallRight ? Color.blue : Color.cyan;
            Gizmos.DrawRay(rightCheckPos, Vector2.right * wallCheckDistance);
        }
    }
}