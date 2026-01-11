using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerWallClimb : MonoBehaviour
{
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask surfacesMask;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 8f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1f, 1f);

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isTouchingLeft, isTouchingRight, isClimbing;
    private bool justWallJumped;
    private float wallJumpTime;
    private const float WALL_JUMP_LOCK_TIME = 0.2f;
    private LevelMechanicsManager currentZone;

    public bool IsOnWall => (isTouchingLeft || isTouchingRight) && CanWallClimb();
    public bool JustWallJumped => justWallJumped && (Time.time - wallJumpTime < WALL_JUMP_LOCK_TIME);

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelMechanicsManager zone = other.GetComponent<LevelMechanicsManager>();
        if (zone != null)
        {
            currentZone = zone;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LevelMechanicsManager zone = other.GetComponent<LevelMechanicsManager>();
        if (zone != null && zone == currentZone)
        {
            currentZone = null;
        }
    }

    public void SetMoveInput(Vector2 input) => moveInput = input;

    private void Update()
    {
        if (Time.time - wallJumpTime >= WALL_JUMP_LOCK_TIME)
        {
            justWallJumped = false;
        }

        CheckWalls();
        
        if (CanWallClimb())
        {
            HandleClimb();
        }
        else
        {
            HandleWallSlide();
        }
    }

    private void CheckWalls()
    {
        Vector3 leftPos = wallCheckLeft ? wallCheckLeft.position : transform.position;
        Vector3 rightPos = wallCheckRight ? wallCheckRight.position : transform.position;

        isTouchingLeft = IsWallSurface(leftPos, Vector2.left);
        isTouchingRight = IsWallSurface(rightPos, Vector2.right);
    }

    private bool IsWallSurface(Vector3 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, surfacesMask);
        
        if (hit.collider != null)
        {
            if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y))
            {
                return true;
            }
        }
        
        return false;
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

    private void HandleWallSlide()
    {
        if (JustWallJumped)
            return;

        bool touching = isTouchingLeft || isTouchingRight;
        
        if (touching && rb.linearVelocity.y < 0 && CanWallClimb())
        {
            float gravityDirection = rb.gravityScale < 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed * gravityDirection);
        }
    }

    public bool TryWallJump()
    {
        CheckWalls();
        
        if (!CanWallJump())
        {
            return false;
        }

        bool touching = isTouchingLeft || isTouchingRight;
        if (!touching)
        {
            return false;
        }

        Vector2 dir = wallJumpDirection.normalized;
        Vector2 jumpVelocity = Vector2.zero;
        
        if (isTouchingLeft)
        {
            jumpVelocity = new Vector2(dir.x * wallJumpForce, dir.y * wallJumpForce);
        }
        else if (isTouchingRight)
        {
            jumpVelocity = new Vector2(-dir.x * wallJumpForce, dir.y * wallJumpForce);
        }
        
        rb.linearVelocity = jumpVelocity;
        isClimbing = false;
        justWallJumped = true;
        wallJumpTime = Time.time;
        return true;
    }

    private bool CanWallClimb()
    {
        if (currentZone != null)
        {
            return currentZone.IsBlueMechanicEnabled && currentZone.IsPlayerInZone;
        }
        
        return false;
    }

    private bool CanWallJump()
    {
        if (currentZone != null)
        {
            bool inZone = currentZone.IsPlayerInZone;
            bool heightRequirementMet = currentZone.CanWallJump;
            
            return currentZone.IsBlueMechanicEnabled && inZone && heightRequirementMet;
        }
        
        return false;
    }
}
