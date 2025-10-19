using UnityEngine;

public class OrbitingPlatform : MonoBehaviour
{
    [SerializeField] private Transform _centerObject;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private bool _clockwise = true;

    private Quaternion _initialRotation;
    private float _radius;

    private void Start()
    {
        _initialRotation = transform.rotation;
        _radius = Vector2.Distance(transform.position, _centerObject.position);
    }

    private void FixedUpdate()
    {
        float direction = _clockwise ? -1f : 1f;

        transform.RotateAround(
            _centerObject.position,
            Vector3.forward,
            _rotationSpeed * direction * Time.fixedDeltaTime
        );

        transform.rotation = _initialRotation;
    }
}
