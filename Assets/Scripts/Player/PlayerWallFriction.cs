using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerWallFriction : MonoBehaviour
{
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask surfacesMask;

    private Vector2 moveInput;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        CheckWalls();
    }

    private void CheckWalls()
    {
        Vector3 leftPos = wallCheckLeft ? wallCheckLeft.position : transform.position;
        Vector3 rightPos = wallCheckRight ? wallCheckRight.position : transform.position;

        isTouchingLeftWall = IsWallSurface(leftPos, Vector2.left);
        isTouchingRightWall = IsWallSurface(rightPos, Vector2.right);
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

    public bool IsTouchingWall()
    {
        return isTouchingLeftWall || isTouchingRightWall;
    }

    public bool IsTouchingLeftWall => isTouchingLeftWall;
    public bool IsTouchingRightWall => isTouchingRightWall;
}
