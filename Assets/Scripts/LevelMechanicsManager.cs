using UnityEngine;

public class LevelMechanicsManager : MonoBehaviour
{
    [Header("Blue Mechanic (Wall Climb & Wall Jump)")]
    [Tooltip("Enable wall climbing and wall jumping mechanics")]
    [SerializeField] private bool enableBlueMechanic = false;

    [Header("Red Mechanic (Phase Through Platforms)")]
    [Tooltip("Enable phase through platform mechanics")]
    [SerializeField] private bool enableRedMechanic = false;

    [Header("Green Mechanic (Player Resize)")]
    [Tooltip("Enable player resize mechanics")]
    [SerializeField] private bool enableGreenMechanic = false;
    
    [SerializeField] private GreenMechanicMode greenMode = GreenMechanicMode.Entry;
    
    [Tooltip("Scale factor when player enters (0.3 = 30% of default size)")]
    [SerializeField][Range(0.3f, 1f)] private float greenScaleFactor = 0.6f;

    [Header("Yellow Mechanic (Gravity Flip)")]
    [Tooltip("Enable gravity flip mechanics")]
    [SerializeField] private bool enableYellowMechanic = false;

    [Tooltip("Velocity boost applied when flipping gravity")]
    [SerializeField] private float flipPushSpeed = 10f;

    [Tooltip("Cooldown duration between gravity flips")]
    [SerializeField] private float gravityFlipCooldown = 1f;

    [Header("Wall Jump Height Requirement")]
    [Tooltip("Minimum height player must climb before wall jump is enabled")]
    [SerializeField] private float minimumClimbHeight = 1f;

    [Header("Editor Visualization")]
    [Tooltip("Color for Blue Mechanic zones (Scene view only)")]
    [SerializeField] private Color blueMechanicColor = new Color(0.3f, 0.6f, 1f, 0.5f);
    
    [Tooltip("Color for Red Mechanic zones (Scene view only)")]
    [SerializeField] private Color redMechanicColor = new Color(1f, 0.3f, 0.3f, 0.5f);
    
    [Tooltip("Color for Green Mechanic zones (Scene view only)")]
    [SerializeField] private Color greenMechanicColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    
    [Tooltip("Color for Yellow Mechanic zones (Scene view only)")]
    [SerializeField] private Color yellowMechanicColor = new Color(1f, 0.9f, 0.3f, 0.5f);
    
    [Tooltip("Color when no mechanic is enabled (Scene view only)")]
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    public bool IsBlueMechanicEnabled => enableBlueMechanic;
    public bool IsRedMechanicEnabled => enableRedMechanic;
    public bool IsGreenMechanicEnabled => enableGreenMechanic;
    public bool IsYellowMechanicEnabled => enableYellowMechanic;
    public GreenMechanicMode GreenMode => greenMode;
    public float GreenScaleFactor => greenScaleFactor;
    public float FlipPushSpeed => flipPushSpeed;
    public float GravityFlipCooldown => gravityFlipCooldown;
    public bool IsPlayerInZone => isPlayerInZone;
    public bool CanWallJump => canWallJump;
    public Bounds ZoneBounds => zoneBounds;

    private bool previousBlueMechanic = false;
    private bool previousRedMechanic = false;
    private bool previousGreenMechanic = false;
    private bool previousYellowMechanic = false;
    private bool isPlayerInZone = false;
    private bool canWallJump = false;
    private bool greenExitUsed = false;
    private float entryHeight = 0f;
    private Transform playerTransform;
    private Bounds zoneBounds;
    private BoxCollider2D zoneCollider;
    private SpriteRenderer editorVisualRenderer;

    private void Awake()
    {
        previousBlueMechanic = enableBlueMechanic;
        previousRedMechanic = enableRedMechanic;
        previousGreenMechanic = enableGreenMechanic;
        previousYellowMechanic = enableYellowMechanic;

        zoneCollider = GetComponent<BoxCollider2D>();
        if (zoneCollider != null)
        {
            zoneBounds = zoneCollider.bounds;
        }

        HideEditorVisualization();
    }

    private void HideEditorVisualization()
    {
        editorVisualRenderer = GetComponent<SpriteRenderer>();
        if (editorVisualRenderer != null)
        {
            editorVisualRenderer.enabled = false;
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
            enableGreenMechanic = false;
            enableYellowMechanic = false;
            previousBlueMechanic = true;
            previousRedMechanic = false;
            previousGreenMechanic = false;
            previousYellowMechanic = false;
        }
        else if (enableRedMechanic && !previousRedMechanic)
        {
            enableBlueMechanic = false;
            enableGreenMechanic = false;
            enableYellowMechanic = false;
            previousBlueMechanic = false;
            previousRedMechanic = true;
            previousGreenMechanic = false;
            previousYellowMechanic = false;
        }
        else if (enableGreenMechanic && !previousGreenMechanic)
        {
            enableBlueMechanic = false;
            enableRedMechanic = false;
            enableYellowMechanic = false;
            previousBlueMechanic = false;
            previousRedMechanic = false;
            previousGreenMechanic = true;
            previousYellowMechanic = false;
        }
        else if (enableYellowMechanic && !previousYellowMechanic)
        {
            enableBlueMechanic = false;
            enableRedMechanic = false;
            enableGreenMechanic = false;
            previousBlueMechanic = false;
            previousRedMechanic = false;
            previousGreenMechanic = false;
            previousYellowMechanic = true;
        }
        else if (!enableBlueMechanic && previousBlueMechanic)
        {
            previousBlueMechanic = false;
        }
        else if (!enableRedMechanic && previousRedMechanic)
        {
            previousRedMechanic = false;
        }
        else if (!enableGreenMechanic && previousGreenMechanic)
        {
            previousGreenMechanic = false;
        }
        else if (!enableYellowMechanic && previousYellowMechanic)
        {
            previousYellowMechanic = false;
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

            if (enableGreenMechanic)
            {
                PlayerScaler scaler = other.GetComponent<PlayerScaler>();
                if (scaler != null)
                {
                    if (greenMode == GreenMechanicMode.Entry)
                    {
                        scaler.ResizeToScale(greenScaleFactor);
                    }
                    else if (greenMode == GreenMechanicMode.Exit && !greenExitUsed)
                    {
                        scaler.ResetToDefaultSize();
                        greenExitUsed = true;
                    }
                }
            }
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
            enableGreenMechanic = false;
            previousBlueMechanic = true;
            previousRedMechanic = false;
            previousGreenMechanic = false;
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
            enableGreenMechanic = false;
            previousRedMechanic = true;
            previousBlueMechanic = false;
            previousGreenMechanic = false;
        }
        else
        {
            enableRedMechanic = false;
            previousRedMechanic = false;
        }
    }

    public void SetGreenMechanic(bool enabled)
    {
        if (enabled)
        {
            enableGreenMechanic = true;
            enableBlueMechanic = false;
            enableRedMechanic = false;
            enableYellowMechanic = false;
            previousGreenMechanic = true;
            previousBlueMechanic = false;
            previousRedMechanic = false;
            previousYellowMechanic = false;
        }
        else
        {
            enableGreenMechanic = false;
            previousGreenMechanic = false;
        }
    }

    public void SetYellowMechanic(bool enabled)
    {
        if (enabled)
        {
            enableYellowMechanic = true;
            enableBlueMechanic = false;
            enableRedMechanic = false;
            enableGreenMechanic = false;
            previousYellowMechanic = true;
            previousBlueMechanic = false;
            previousRedMechanic = false;
            previousGreenMechanic = false;
        }
        else
        {
            enableYellowMechanic = false;
            previousYellowMechanic = false;
        }
    }

    public void ResetGreenExit()
    {
        greenExitUsed = false;
    }
}

public enum GreenMechanicMode
{
    Entry,
    Exit
}
