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
    

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
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
        bool hasHorizontalInput = Mathf.Abs(moveInput.x) > 0.01f;

        float targetVelocityX;
        bool hasInput = Mathf.Abs(moveInput.x) > 0.01f;


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
