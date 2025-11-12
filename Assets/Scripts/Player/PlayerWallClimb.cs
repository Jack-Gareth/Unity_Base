using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerWallClimb : MonoBehaviour
{
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1f);

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isTouchingLeft, isTouchingRight, isClimbing;
    private bool justWallJumped;
    private float wallJumpTime;
    private const float WALL_JUMP_LOCK_TIME = 0.2f;

    public bool IsOnWall => (isTouchingLeft || isTouchingRight) && CanWallClimb();
    public bool JustWallJumped => justWallJumped && (Time.time - wallJumpTime < WALL_JUMP_LOCK_TIME);

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void SetMoveInput(Vector2 input) => moveInput = input;

    private void Update()
    {
        if (Time.time - wallJumpTime >= WALL_JUMP_LOCK_TIME)
        {
            justWallJumped = false;
        }
        
        if (!CanWallClimb()) return;

        CheckWalls();
        HandleClimb();
    }

    private void CheckWalls()
    {
        Vector3 leftPos = wallCheckLeft ? wallCheckLeft.position : transform.position;
        Vector3 rightPos = wallCheckRight ? wallCheckRight.position : transform.position;

        isTouchingLeft = Physics2D.Raycast(leftPos, Vector2.left, wallCheckDistance, wallMask);
        isTouchingRight = Physics2D.Raycast(rightPos, Vector2.right, wallCheckDistance, wallMask);
    }

    private void HandleClimb()
    {
        if (JustWallJumped)
            return;
            
        bool touching = isTouchingLeft || isTouchingRight;
        bool pressingUp = moveInput.y > 0;

        if (touching && pressingUp)
        {
            isClimbing = true;
            rb.linearVelocity = new Vector2(0f, moveInput.y * climbSpeed);
        }
        else
        {
            isClimbing = false;
        }
    }

    public bool TryWallJump()
    {
        CheckWalls();
        
        if (!CanWallClimb()) return false;

        bool touching = isTouchingLeft || isTouchingRight;
        if (!touching) return false;

        Vector2 dir = wallJumpDirection.normalized;
        Vector2 jumpVelocity = Vector2.zero;
        
        if (isTouchingLeft)
        {
            jumpVelocity = new Vector2(dir.x * wallJumpForce, dir.y * wallJumpForce);
            Debug.Log($"Wall Jump LEFT: velocity={jumpVelocity}");
        }
        else if (isTouchingRight)
        {
            jumpVelocity = new Vector2(-dir.x * wallJumpForce, dir.y * wallJumpForce);
            Debug.Log($"Wall Jump RIGHT: velocity={jumpVelocity}");
        }
        
        rb.linearVelocity = jumpVelocity;
        isClimbing = false;
        justWallJumped = true;
        wallJumpTime = Time.time;
        return true;
    }

    private bool CanWallClimb()
    {
        return LevelColorManager.Instance != null &&
               LevelColorManager.Instance.CurrentColor == LevelColor.Blue;
    }
}
