using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private const float _jumpForce = 20f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.linearVelocity; 
            velocity.y = _jumpForce;
            rb.linearVelocity = velocity;            
        }
    }
}
