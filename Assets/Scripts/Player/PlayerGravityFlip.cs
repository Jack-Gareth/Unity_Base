using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Gravity Flip Settings")]
    [SerializeField] private Transform _groundCheck;
    [Tooltip("Speed at which the player travels when gravity flip is activated")]
    [SerializeField] private float _gravityFlipTravelSpeed = 10f;

    private Rigidbody2D _rb;
    private bool _isFlipped;
    private bool _isFlipping;
    private Vector3 _normalGroundCheckLocalPos;
    private Vector3 _invertedGroundCheckLocalPos;
    private float _originalGravityScale;
    private Coroutine _activeTransportCoroutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _originalGravityScale = Mathf.Abs(_rb.gravityScale);
        _normalGroundCheckLocalPos = _groundCheck.localPosition;
        _invertedGroundCheckLocalPos = new Vector3(_normalGroundCheckLocalPos.x, -_normalGroundCheckLocalPos.y, _normalGroundCheckLocalPos.z);
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
        {
            if (_activeTransportCoroutine != null)
                StopCoroutine(_activeTransportCoroutine);

            _activeTransportCoroutine = StartCoroutine(TransportPlayerRoutine());
        }
    }

    private IEnumerator TransportPlayerRoutine()
    {
        _isFlipping = true;

        _rb.gravityScale = 0f;

        float travelDirection = _isFlipped ? -1f : 1f;
        float targetSpeed = Mathf.Abs(_gravityFlipTravelSpeed) * travelDirection;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, targetSpeed);

        yield return new WaitForSeconds(0.05f);

        while (Mathf.Abs(_rb.linearVelocity.y) > 0.1f)
        {
            yield return null;
        }

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);

        FlipGravity();

        _isFlipping = false;
        _activeTransportCoroutine = null;
    }

    private void HandleColorChange(LevelColor newColor)
    {
        Debug.Log($"PlayerGravityFlip: Color changed to {newColor}, isFlipped: {_isFlipped}, gravityScale: {_rb.gravityScale}");

        if (newColor != LevelColor.Yellow)
        {
            if (_activeTransportCoroutine != null)
            {
                StopCoroutine(_activeTransportCoroutine);
                _activeTransportCoroutine = null;
            }

            _isFlipping = false;

            if (_isFlipped || _rb.gravityScale < 0)
            {
                Debug.Log("PlayerGravityFlip: Resetting gravity to normal");
                ResetGravity();
            }
        }
    }

    private void FlipGravity()
    {
        _isFlipped = !_isFlipped;
        _rb.gravityScale = _isFlipped ? -_originalGravityScale : _originalGravityScale;
        _groundCheck.localPosition = _isFlipped ? _invertedGroundCheckLocalPos : _normalGroundCheckLocalPos;

        Debug.Log($"PlayerGravityFlip: Gravity flipped. isFlipped: {_isFlipped}, gravityScale: {_rb.gravityScale}");
    }

    private void ResetGravity()
    {
        _rb.gravityScale = _originalGravityScale;
        _isFlipped = false;
        _groundCheck.localPosition = _normalGroundCheckLocalPos;

        Debug.Log($"PlayerGravityFlip: Gravity reset. gravityScale: {_rb.gravityScale}");
    }
}
