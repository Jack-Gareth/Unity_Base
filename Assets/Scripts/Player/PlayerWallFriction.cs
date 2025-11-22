using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerWallFriction : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D frictionMaterial;
    [SerializeField] private PhysicsMaterial2D noFrictionMaterial;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask surfacesMask;

    private Collider2D playerCollider;
    private Vector2 moveInput;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        
        if (frictionMaterial == null)
        {
            Debug.LogWarning("PlayerWallFriction: Friction Material not assigned!");
        }
        
        if (noFrictionMaterial == null)
        {
            noFrictionMaterial = new PhysicsMaterial2D("No Friction");
            noFrictionMaterial.friction = 0f;
            noFrictionMaterial.bounciness = 0f;
        }
        
        playerCollider.sharedMaterial = frictionMaterial;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void Update()
    {
        CheckWalls();
        UpdateFriction();
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

    private void UpdateFriction()
    {
        bool pressingIntoWall = false;

        if (isTouchingLeftWall && moveInput.x < -0.1f)
        {
            pressingIntoWall = true;
        }
        else if (isTouchingRightWall && moveInput.x > 0.1f)
        {
            pressingIntoWall = true;
        }

        if (pressingIntoWall)
        {
            playerCollider.sharedMaterial = noFrictionMaterial;
        }
        else
        {
            playerCollider.sharedMaterial = frictionMaterial;
        }
    }
}
