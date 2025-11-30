using UnityEngine;
using UnityEngine.Events;

public class SurfaceCollider : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<Collision2D> onGroundHit;
    public UnityEvent<Collision2D> onWallHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void ProcessCollision(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            
            if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x))
            {
                onGroundHit?.Invoke(collision);
            }
            else
            {
                onWallHit?.Invoke(collision);
            }
        }
    }
}
