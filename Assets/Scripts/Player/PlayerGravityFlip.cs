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
    
    private void FixedUpdate()
    {
        if (_isFlipped && !_isFlipping && !_autoResetInProgress)
        {
            CheckForAutoGravityReset();
        }
    }
    
    private void CheckForAutoGravityReset()
    {
        Vector2 playerPosition = transform.position;
        RaycastHit2D hitAbove = Physics2D.Raycast(playerPosition, Vector2.up, raycastDistance, groundLayer);
        
        if (hitAbove.collider == null)
        {
            Debug.Log("PlayerGravityFlip: No ceiling detected while flipped - auto-resetting gravity");
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
        
        Debug.Log("PlayerGravityFlip: Auto-reset completed - player falling to ground");
        
        yield return new WaitForSeconds(0.1f);
        
        _autoResetInProgress = false;
    }

    private void HandleGravityFlip()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Yellow)
        {
            Debug.Log("PlayerGravityFlip: Not in yellow mode");
            return;
        }
        
        if (_autoResetInProgress)
        {
            Debug.Log("PlayerGravityFlip: Auto-reset in progress, cannot flip");
            return;
        }

        if (!CanFlipGravity())
        {
            Debug.Log("PlayerGravityFlip: Cannot flip gravity - requirements not met");
            return;
        }

        if (!_isFlipping)
        {
            if (_activeTransportCoroutine != null)
                StopCoroutine(_activeTransportCoroutine);

            _activeTransportCoroutine = StartCoroutine(TransportPlayerRoutine());
        }
    }
    
    private bool CanFlipGravity()
    {
        Vector2 playerPosition = transform.position;
        
        RaycastHit2D hitBelow = Physics2D.Raycast(playerPosition, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitAbove = Physics2D.Raycast(playerPosition, Vector2.up, raycastDistance, groundLayer);
        
        if (hitBelow.collider == null)
        {
            Debug.Log("PlayerGravityFlip: No ground detected below player");
            return false;
        }
        
        if (hitAbove.collider == null)
        {
            Debug.Log("PlayerGravityFlip: No ground detected above player");
            return false;
        }
        
        BoxCollider2D colliderBelow = hitBelow.collider.GetComponent<BoxCollider2D>();
        BoxCollider2D colliderAbove = hitAbove.collider.GetComponent<BoxCollider2D>();
        
        if (colliderBelow == null || colliderAbove == null)
        {
            Debug.Log("PlayerGravityFlip: Ground objects missing BoxCollider2D");
            return false;
        }
        
        float lengthBelow = colliderBelow.size.x * hitBelow.transform.lossyScale.x;
        float lengthAbove = colliderAbove.size.x * hitAbove.transform.lossyScale.x;
        
        if (Mathf.Abs(lengthBelow - lengthAbove) > lengthTolerance)
        {
            Debug.Log($"PlayerGravityFlip: Ground lengths don't match. Below: {lengthBelow}, Above: {lengthAbove}");
            return false;
        }
        
        Debug.Log($"PlayerGravityFlip: Can flip! Ground below: {hitBelow.collider.name}, Above: {hitAbove.collider.name}, Length: {lengthBelow}");
        return true;
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
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
            
        Vector2 playerPosition = transform.position;
        
        RaycastHit2D hitBelow = Physics2D.Raycast(playerPosition, Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitAbove = Physics2D.Raycast(playerPosition, Vector2.up, raycastDistance, groundLayer);
        
        Gizmos.color = hitBelow.collider != null ? Color.green : Color.red;
        Gizmos.DrawLine(playerPosition, playerPosition + Vector2.down * raycastDistance);
        
        Gizmos.color = hitAbove.collider != null ? Color.green : Color.red;
        Gizmos.DrawLine(playerPosition, playerPosition + Vector2.up * raycastDistance);
    }
}
