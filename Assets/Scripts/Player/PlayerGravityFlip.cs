using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Gravity Flip Settings")]
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private bool isFlipped;
    private bool isOnCooldown;
    private bool isInGravityFlipZone;

    private Vector3 normalGroundPos;
    private Vector3 invertedGroundPos;
    private float originalGravityScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = Mathf.Abs(rb.gravityScale);

        if (groundCheck != null)
        {
            normalGroundPos = groundCheck.localPosition;
            invertedGroundPos = new Vector3(
                normalGroundPos.x,
                -normalGroundPos.y,
                normalGroundPos.z);
        }
    }

    private void OnEnable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnColorAbilityInput += HandleFlipInput;
        }
        PlayerEvents.OnPlayerRespawn += ResetGravity;
    }

    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnColorAbilityInput -= HandleFlipInput;
        }
        PlayerEvents.OnPlayerRespawn -= ResetGravity;
    }

    private void Start()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnColorAbilityInput += HandleFlipInput;
        }
    }

    private void Update()
    {
        CheckForZone();
    }

    private void CheckForZone()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        isInGravityFlipZone = false;

        foreach (Collider2D col in colliders)
        {
            LevelMechanicsManager zone = col.GetComponent<LevelMechanicsManager>();
            if (zone != null && zone.IsYellowMechanicEnabled && zone.IsPlayerInZone)
            {
                isInGravityFlipZone = true;
                break;
            }
        }
    }

    private void HandleFlipInput()
    {
        if (!isInGravityFlipZone)
            return;

        if (isOnCooldown)
            return;

        FlipGravity();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(1f);
        isOnCooldown = false;
    }

    private void FlipGravity()
    {
        isFlipped = !isFlipped;

        rb.gravityScale = isFlipped ? -originalGravityScale : originalGravityScale;

        float direction = isFlipped ? 1f : -1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f * direction);

        if (groundCheck != null)
        {
            groundCheck.localPosition = isFlipped ? invertedGroundPos : normalGroundPos;
        }
    }

    private void ResetGravity()
    {
        rb.gravityScale = originalGravityScale;
        isFlipped = false;
        isOnCooldown = false;
        
        if (groundCheck != null)
        {
            groundCheck.localPosition = normalGroundPos;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (isInGravityFlipZone)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        if (isFlipped)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
        }
    }
}
