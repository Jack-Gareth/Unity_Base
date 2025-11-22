using UnityEngine;

public class LevelMechanicsManager : MonoBehaviour
{
    [Header("Blue Mechanic (Wall Climb & Wall Jump)")]
    [Tooltip("Enable wall climbing and wall jumping mechanics")]
    [SerializeField] private bool enableBlueMechanic = false;

    [Header("Red Mechanic (Phase Through Platforms)")]
    [Tooltip("Enable phase through platform mechanics")]
    [SerializeField] private bool enableRedMechanic = false;

    [Header("Wall Jump Height Requirement")]
    [Tooltip("Minimum height player must climb before wall jump is enabled")]
    [SerializeField] private float minimumClimbHeight = 1f;

    public bool IsBlueMechanicEnabled => enableBlueMechanic;
    public bool IsRedMechanicEnabled => enableRedMechanic;
    public bool IsPlayerInZone => isPlayerInZone;
    public bool CanWallJump => canWallJump;
    public Bounds ZoneBounds => zoneBounds;

    private bool previousBlueMechanic = false;
    private bool previousRedMechanic = false;
    private bool isPlayerInZone = false;
    private bool canWallJump = false;
    private float entryHeight = 0f;
    private Transform playerTransform;
    private Bounds zoneBounds;
    private BoxCollider2D zoneCollider;

    private void Awake()
    {
        previousBlueMechanic = enableBlueMechanic;
        previousRedMechanic = enableRedMechanic;

        zoneCollider = GetComponent<BoxCollider2D>();
        if (zoneCollider != null)
        {
            zoneBounds = zoneCollider.bounds;
        }
    }

    private void Update()
    {
        if (zoneCollider != null)
        {
            zoneBounds = zoneCollider.bounds;
        }

        if (isPlayerInZone && playerTransform != null && !canWallJump)
        {
            float heightClimbed = playerTransform.position.y - entryHeight;
            if (heightClimbed >= minimumClimbHeight)
            {
                canWallJump = true;
            }
        }
    }

    private void OnValidate()
    {
        EnforceSingleCheckbox();
    }

    private void EnforceSingleCheckbox()
    {
        if (enableBlueMechanic && !previousBlueMechanic)
        {
            enableRedMechanic = false;
            previousBlueMechanic = true;
            previousRedMechanic = false;
        }
        else if (enableRedMechanic && !previousRedMechanic)
        {
            enableBlueMechanic = false;
            previousBlueMechanic = false;
            previousRedMechanic = true;
        }
        else if (!enableBlueMechanic && previousBlueMechanic)
        {
            previousBlueMechanic = false;
        }
        else if (!enableRedMechanic && previousRedMechanic)
        {
            previousRedMechanic = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            playerTransform = other.transform;
            entryHeight = other.transform.position.y;
            
            bool isGrounded = CheckIfPlayerIsGrounded(other);
            canWallJump = !isGrounded;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            canWallJump = false;
            playerTransform = null;
        }
    }

    private bool CheckIfPlayerIsGrounded(Collider2D playerCollider)
    {
        PlayerJump playerJump = playerCollider.GetComponent<PlayerJump>();
        if (playerJump != null)
        {
            return playerJump.GetType()
                .GetField("isGrounded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(playerJump) is bool grounded && grounded;
        }
        return false;
    }

    public void SetBlueMechanic(bool enabled)
    {
        if (enabled)
        {
            enableBlueMechanic = true;
            enableRedMechanic = false;
            previousBlueMechanic = true;
            previousRedMechanic = false;
        }
        else
        {
            enableBlueMechanic = false;
            previousBlueMechanic = false;
        }
    }

    public void SetRedMechanic(bool enabled)
    {
        if (enabled)
        {
            enableRedMechanic = true;
            enableBlueMechanic = false;
            previousRedMechanic = true;
            previousBlueMechanic = false;
        }
        else
        {
            enableRedMechanic = false;
            previousRedMechanic = false;
        }
    }
}
