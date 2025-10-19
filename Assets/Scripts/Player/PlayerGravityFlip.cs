using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _flipSpeedMultiplier = 5f;   
    [SerializeField] private float _flipDuration = 1f;       

    private Rigidbody2D _rb;
    private bool _isFlipped;
    private bool _isFlipping;
    private Vector3 _normalGroundCheckLocalPos;
    private Vector3 _invertedGroundCheckLocalPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _normalGroundCheckLocalPos = _groundCheck.localPosition;
        _invertedGroundCheckLocalPos = new Vector3(_normalGroundCheckLocalPos.x, -_normalGroundCheckLocalPos.y,_normalGroundCheckLocalPos.z
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
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Yellow)
            return;

        if (!_isFlipping)
            StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        _isFlipping = true;
  
        float direction = _isFlipped ? -1f : 1f;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, direction * _flipSpeedMultiplier);

        yield return new WaitForSeconds(_flipDuration);
  
        FlipGravity();
        _isFlipping = false;
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
