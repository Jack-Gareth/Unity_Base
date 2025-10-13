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
        // Retry input connection if it failed in OnEnable
        if (InputManager.Instance != null)
        {
            Debug.Log("PlayerController Start: InputManager available, ensuring callbacks are connected");
            // Remove first to avoid double registration
            InputManager.Instance.OnMoveInput -= OnMoveInput;
            InputManager.Instance.OnJumpInput -= OnJumpInput;
            // Then add
            InputManager.Instance.OnMoveInput += OnMoveInput;
            InputManager.Instance.OnJumpInput += OnJumpInput;
        }
        else
        {
            Debug.LogError("PlayerController Start: InputManager.Instance is still null!");
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("PlayerController OnEnable called");
        if (InputManager.Instance != null)
        {
            Debug.Log("InputManager found, setting up callbacks");
            InputManager.Instance.OnMoveInput += OnMoveInput;
            InputManager.Instance.OnJumpInput += OnJumpInput;
        }
        else
        {
            Debug.LogWarning("InputManager.Instance is null in PlayerController.OnEnable");
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
        // Simple test to see if Update is working
        if (Time.frameCount == 100)
        {
            Debug.Log("PlayerController Update is working!");
        }
        
        CheckGroundStatus();
        CheckWallStatus();
        HandleWallClimbing();
        
        // Debug current state every 30 frames
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"Player State - CanWallClimb: {CanWallClimb()}, TouchingWallLeft: {isTouchingWallLeft}, TouchingWallRight: {isTouchingWallRight}, IsWallClimbing: {isWallClimbing}, MoveInput: {moveInput}");
        }
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
        
        if (wasGrounded != isGrounded)
        {
            Debug.Log($"Ground status changed: {isGrounded}");
        }
    }
    
    /// <summary>
    /// Checks if the player is touching walls (only matters when level is blue)
    /// </summary>
    private void CheckWallStatus()
    {
        if (!CanWallClimb()) 
        {
            // Reset wall status if not blue
            isTouchingWallLeft = false;
            isTouchingWallRight = false;
            return;
        }
        
        bool wasTouchingLeft = isTouchingWallLeft;
        bool wasTouchingRight = isTouchingWallRight;
        
        // Use player position if wall check transforms are not set up
        Vector3 leftCheckPos = wallCheckLeft != null ? wallCheckLeft.position : transform.position;
        Vector3 rightCheckPos = wallCheckRight != null ? wallCheckRight.position : transform.position;
        
        // Perform raycasts with debug info
        RaycastHit2D leftHit = Physics2D.Raycast(leftCheckPos, Vector2.left, wallCheckDistance, wallLayerMask);
        RaycastHit2D rightHit = Physics2D.Raycast(rightCheckPos, Vector2.right, wallCheckDistance, wallLayerMask);
        
        isTouchingWallLeft = leftHit.collider != null;
        isTouchingWallRight = rightHit.collider != null;
        
        // Debug wall detection
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            Debug.Log($"Wall Detection - Left: {(leftHit.collider != null ? leftHit.collider.name : "None")}, Right: {(rightHit.collider != null ? rightHit.collider.name : "None")}");
            Debug.Log($"Wall LayerMask: {wallLayerMask}, Distance: {wallCheckDistance}");
        }
        
        if (wasTouchingLeft != isTouchingWallLeft || wasTouchingRight != isTouchingWallRight)
        {
            Debug.Log($"Wall status changed - Left: {isTouchingWallLeft}, Right: {isTouchingWallRight}, CanWallClimb: {CanWallClimb()}");
        }
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
                Debug.Log("Stopping wall climbing - level not blue");
                isWallClimbing = false;
            }
            return;
        }
        
        bool isTouchingAnyWall = isTouchingWallLeft || isTouchingWallRight;
        bool pressingUp = moveInput.y > 0;
        
        Debug.Log($"Wall Climbing Check - TouchingWall: {isTouchingAnyWall}, PressingUp: {pressingUp}, IsWallClimbing: {isWallClimbing}, Grounded: {isGrounded}");
        
        // Start or continue wall climbing if touching wall and pressing up
        if (isTouchingAnyWall && pressingUp)
        {
            if (!isWallClimbing)
            {
                Debug.Log("Started wall climbing");
                isWallClimbing = true;
            }
            
            // Apply upward climbing velocity
            float climbVelocity = moveInput.y * wallClimbSpeed;
            rb.linearVelocity = new Vector2(0f, climbVelocity);
            Debug.Log($"Climbing up wall with velocity: {climbVelocity}");
        }
        else
        {
            // Stop wall climbing if not touching wall or not pressing up
            if (isWallClimbing)
            {
                Debug.Log("Stopped wall climbing");
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
            // Normal ground jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            Debug.Log("Ground jump performed");
        }
        else if (isWallClimbing && CanWallClimb())
        {
            // Wall jump - push away from wall
            Vector2 jumpDirection = wallJumpDirection.normalized;
            
            if (isTouchingWallLeft)
            {
                // Jump right when on left wall
                rb.linearVelocity = new Vector2(jumpDirection.x * wallJumpForce, jumpDirection.y * wallJumpForce);
            }
            else if (isTouchingWallRight)
            {
                // Jump left when on right wall
                rb.linearVelocity = new Vector2(-jumpDirection.x * wallJumpForce, jumpDirection.y * wallJumpForce);
            }
            
            isWallClimbing = false;
            Debug.Log("Wall jump performed");
        }
    }
    
    private void OnMoveInput(Vector2 input)
    {
        moveInput = input;
        Debug.Log($"Movement input received: {input}");
    }
    
    private void OnJumpInput()
    {
        Debug.Log($"Jump input detected! IsGrounded: {isGrounded}, IsWallClimbing: {isWallClimbing}, CanWallClimb: {CanWallClimb()}");
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