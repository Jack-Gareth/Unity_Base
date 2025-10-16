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

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void SetMoveInput(Vector2 input) => moveInput = input;

    private void Update()
    {
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

    public void TryWallJump()
    {
        if (!isClimbing || !CanWallClimb()) return;

        Vector2 dir = wallJumpDirection.normalized;
        if (isTouchingLeft)
            rb.linearVelocity = new Vector2(dir.x * wallJumpForce, dir.y * wallJumpForce);
        else if (isTouchingRight)
            rb.linearVelocity = new Vector2(-dir.x * wallJumpForce, dir.y * wallJumpForce);

        isClimbing = false;
    }

    private bool CanWallClimb()
    {
        return LevelColorManager.Instance != null &&
               LevelColorManager.Instance.CurrentColor == LevelColor.Blue;
    }
}
