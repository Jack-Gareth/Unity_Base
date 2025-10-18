using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [SerializeField] private Transform _groundCheck;

    private Rigidbody2D _rb;
    private bool _isFlipped;
    private Vector3 _normalGroundCheckLocalPos;
    private Vector3 _invertedGroundCheckLocalPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _normalGroundCheckLocalPos = _groundCheck.localPosition;
        _invertedGroundCheckLocalPos = new Vector3(
            _normalGroundCheckLocalPos.x,
            -_normalGroundCheckLocalPos.y,
            _normalGroundCheckLocalPos.z
        );
    }

    private void OnEnable()
    {
        PlayerEvents.OnGravityFlip += HandleGravityFlip;
        PlayerEvents.OnColorChanged += HandleColorChange;
        PlayerEvents.OnPlayerRespawn += ResetGravity;
    }

    private void OnDisable()
    {
        PlayerEvents.OnGravityFlip -= HandleGravityFlip;
        PlayerEvents.OnColorChanged -= HandleColorChange;
        PlayerEvents.OnPlayerRespawn -= ResetGravity;
    }

    private void HandleGravityFlip()
    {
        LevelColorManager colorManager = LevelColorManager.Instance;
        if (colorManager.CurrentColor != LevelColor.Yellow)
            return;

        FlipGravity();
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Yellow && _isFlipped)
            ResetGravity();
    }

    private void FlipGravity()
    {
        _rb.gravityScale = -_rb.gravityScale;
        _isFlipped = _rb.gravityScale < 0;
        _groundCheck.localPosition = _isFlipped ? _invertedGroundCheckLocalPos : _normalGroundCheckLocalPos;
    }

    private void ResetGravity()
    {
        _rb.gravityScale = Mathf.Abs(_rb.gravityScale);
        _isFlipped = false;
        _groundCheck.localPosition = _normalGroundCheckLocalPos;
    }
}
