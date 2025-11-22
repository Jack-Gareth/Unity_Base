using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Gravity Flip Settings")]
    [SerializeField] private Transform _groundCheck;
    [Tooltip("Speed at which the player travels when gravity flip is activated")]
    [SerializeField] private float _gravityFlipTravelSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private float lengthTolerance = 0.1f;

    private Rigidbody2D _rb;
    private bool _isFlipped;
    private bool _isFlipping;
    private Vector3 _normalGroundCheckLocalPos;
    private Vector3 _invertedGroundCheckLocalPos;
    private float _originalGravityScale;
    private Coroutine _activeTransportCoroutine;
    private bool _autoResetInProgress = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _originalGravityScale = Mathf.Abs(_rb.gravityScale);
        _normalGroundCheckLocalPos = _groundCheck.localPosition;
        _invertedGroundCheckLocalPos = new Vector3(_normalGroundCheckLocalPos.x, -_normalGroundCheckLocalPos.y, _normalGroundCheckLocalPos.z);
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += HandleColorAbility;
        PlayerEvents.OnColorChanged += HandleColorChange;
        PlayerEvents.OnPlayerRespawn += ResetGravity;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= HandleColorAbility;
        PlayerEvents.OnColorChanged -= HandleColorChange;
        PlayerEvents.OnPlayerRespawn -= ResetGravity;
    }

    private void FixedUpdate()
    {
        if (_isFlipped && !_isFlipping && !_autoResetInProgress)
        {
            CheckForAutoGravityReset();
        }
    }

    private void HandleColorAbility()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Yellow)
            return;

        if (_autoResetInProgress || _isFlipping)
            return;

        if (_activeTransportCoroutine != null)
            StopCoroutine(_activeTransportCoroutine);

        _activeTransportCoroutine = StartCoroutine(TransportPlayerRoutine());
    }

    private void CheckForAutoGravityReset()
    {
        Vector2 playerPosition = transform.position;
        RaycastHit2D hitAbove = Physics2D.Raycast(playerPosition, Vector2.up, raycastDistance, groundLayer);

        if (hitAbove.collider == null)
        {
            StartCoroutine(AutoResetGravityRoutine());
        }
    }

    private IEnumerator AutoResetGravityRoutine()
    {
        _autoResetInProgress = true;

        if (_activeTransportCoroutine != null)
        {
            StopCoroutine(_activeTransportCoroutine);
            _activeTransportCoroutine = null;
        }

        _rb.gravityScale = _originalGravityScale;
        _isFlipped = false;
        _groundCheck.localPosition = _normalGroundCheckLocalPos;

        yield return new WaitForSeconds(0.1f);
        _autoResetInProgress = false;
    }

    private bool CanFlipGravity()
    {
        Vector2 playerPosition = transform.position;

        RaycastHit2D hitBelow = Physics2D.Raycast(playerPosition, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitAbove = Physics2D.Raycast(playerPosition, Vector2.up, raycastDistance, groundLayer);

        return hitBelow.collider != null && hitAbove.collider != null;
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
            yield return null;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        FlipGravity();

        _isFlipping = false;
        _activeTransportCoroutine = null;
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Yellow)
        {
            if (_activeTransportCoroutine != null)
            {
                StopCoroutine(_activeTransportCoroutine);
                _activeTransportCoroutine = null;
            }

            if (_isFlipped || _rb.gravityScale < 0)
                ResetGravity();
        }
    }

    private void FlipGravity()
    {
        _isFlipped = !_isFlipped;
        _rb.gravityScale = _isFlipped ? -_originalGravityScale : _originalGravityScale;
        _groundCheck.localPosition = _isFlipped ? _invertedGroundCheckLocalPos : _normalGroundCheckLocalPos;
    }

    private void ResetGravity()
    {
        _rb.gravityScale = _originalGravityScale;
        _isFlipped = false;
        _groundCheck.localPosition = _normalGroundCheckLocalPos;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vector2 playerPosition = transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerPosition, playerPosition + Vector2.down * raycastDistance);
        Gizmos.DrawLine(playerPosition, playerPosition + Vector2.up * raycastDistance);
    }
}
