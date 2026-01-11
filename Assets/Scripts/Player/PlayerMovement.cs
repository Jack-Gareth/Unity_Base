using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float airControlPercent = 50f;
    [SerializeField] private float groundDrag = 10f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask surfacesMask;
    
    [Header("Physics Materials")]
    [SerializeField] private PhysicsMaterial2D groundFrictionMaterial;
    [SerializeField] private PhysicsMaterial2D airFrictionMaterial;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private Collider2D playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
<<<<<<< HEAD
        rb.bodyType = RigidbodyType2D.Dynamic;
        
=======
        wallClimb = GetComponent<PlayerWallClimb>();
        playerCollider = GetComponent<Collider2D>();
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        
        if (groundFrictionMaterial == null)
        {
            groundFrictionMaterial = new PhysicsMaterial2D("Ground Friction");
            groundFrictionMaterial.friction = 0.4f;
            groundFrictionMaterial.bounciness = 0f;
        }
        
        if (airFrictionMaterial == null)
        {
            airFrictionMaterial = new PhysicsMaterial2D("Air No Friction");
            airFrictionMaterial.friction = 0f;
            airFrictionMaterial.bounciness = 0f;
        }
>>>>>>> origin/main
    }

    public void SetMoveInput(Vector2 input) => moveInput = input;

    private void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, surfacesMask);
        }
        
        UpdatePhysicsMaterial();
    }
    
    private void UpdatePhysicsMaterial()
    {
        if (playerCollider == null)
            return;
            
        if (isGrounded)
        {
            playerCollider.sharedMaterial = groundFrictionMaterial;
        }
        else
        {
            playerCollider.sharedMaterial = airFrictionMaterial;
        }
    }

    private void FixedUpdate()
    {
<<<<<<< HEAD
        bool hasHorizontalInput = Mathf.Abs(moveInput.x) > 0.01f;
=======
        if (wallClimb != null && wallClimb.JustWallJumped)
            return;

        float targetVelocityX;
        bool hasInput = Mathf.Abs(moveInput.x) > 0.01f;
>>>>>>> origin/main

        if (isGrounded)
        {
            targetVelocityX = moveInput.x * moveSpeed;
            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        }
        else
        {
            if (hasInput)
            {
                float airControl = moveSpeed * (airControlPercent / 100f);
                targetVelocityX = moveInput.x * airControl;
                rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
            }
            else
            {
                float dragForce = rb.linearVelocity.x * groundDrag * Time.fixedDeltaTime;
                float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0, Mathf.Abs(dragForce));
                rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
            }
        }
    }
}
