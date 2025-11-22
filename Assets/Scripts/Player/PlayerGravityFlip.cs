using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Gravity Flip Settings")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float flipPushSpeed = 10f;
    [SerializeField] private float cooldownDuration = 1f;

    private Rigidbody2D _rb;
    private bool _isFlipped;
    private bool _isOnCooldown;

    private Vector3 _normalGroundPos;
    private Vector3 _invertedGroundPos;
    private float _originalGravityScale;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _originalGravityScale = Mathf.Abs(_rb.gravityScale);

        _normalGroundPos = _groundCheck.localPosition;
        _invertedGroundPos = new Vector3(
            _normalGroundPos.x,
            -_normalGroundPos.y,
            _normalGroundPos.z);
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += HandleFlip;
        PlayerEvents.OnColorChanged += HandleColorChange;
        PlayerEvents.OnPlayerRespawn += ResetGravity;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= HandleFlip;
        PlayerEvents.OnColorChanged -= HandleColorChange;
        PlayerEvents.OnPlayerRespawn -= ResetGravity;
    }

    private void HandleFlip()
    {
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Yellow)
            return;

        if (_isOnCooldown)
            return;

        FlipGravity();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        _isOnCooldown = false;
    }

    private void FlipGravity()
    {
        _isFlipped = !_isFlipped;

        _rb.gravityScale = _isFlipped ? -_originalGravityScale : _originalGravityScale;

        // Give a push so the player doesn't sit motionless mid-flip
        float direction = _isFlipped ? 1f : -1f;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, flipPushSpeed * direction);

        _groundCheck.localPosition = _isFlipped ? _invertedGroundPos : _normalGroundPos;
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Yellow)
            ResetGravity();
    }

    private void ResetGravity()
    {
        _rb.gravityScale = _originalGravityScale;
        _isFlipped = false;
        _isOnCooldown = false;
        _groundCheck.localPosition = _normalGroundPos;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
