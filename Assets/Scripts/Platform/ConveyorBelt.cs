using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float _beltSpeed = 2f;
    [SerializeField] private bool _reverseDirection = false;
    [SerializeField] private SurfaceEffector2D _topEffector;
    [SerializeField] private SurfaceEffector2D _bottomEffector;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.gravityScale = 0f;
        _rb.simulated = true;
        ApplyEffectorSpeeds();
    }

    private void ApplyEffectorSpeeds()
    {
        float dir = _reverseDirection ? -1f : 1f;
        _topEffector.speed = _beltSpeed * dir;
        _topEffector.useContactForce = true;

        _bottomEffector.speed = -_beltSpeed * dir;
        _bottomEffector.useContactForce = true;
    }

    public void SetReverse(bool reversed)
    {
        if (_reverseDirection == reversed) return;
        _reverseDirection = reversed;
        ApplyEffectorSpeeds();
    }

    public void SetSpeed(float newSpeed)
    {
        if (Mathf.Approximately(_beltSpeed, newSpeed)) return;
        _beltSpeed = Mathf.Max(0f, newSpeed);
        ApplyEffectorSpeeds();
    }
}
