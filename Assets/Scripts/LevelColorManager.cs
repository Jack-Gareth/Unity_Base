using UnityEngine;
using UnityEngine.InputSystem;

public enum LevelColor
{
    White,
    Red,
    Blue,
    Green
}

public class LevelColorManager : MonoBehaviour
{
    public static LevelColorManager Instance { get; private set; }
    
    [Header("Color Settings")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color whiteColor = Color.white;
    
    [Header("Shader Settings")]
    [SerializeField] private string colorPropertyName = "_Color";
    
    private SpriteRenderer[] levelRenderers;
    private Material[] levelMaterials;
    private BoxCollider2D[] levelColliders;
    private LevelColor currentColor = LevelColor.White;
    private bool inputManagerConnected = false;
    
    public LevelColor CurrentColor => currentColor;
    
    private void Awake()
    {
        // Set up singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple LevelColorManager instances found!");
        }
        
        // Wait a frame to ensure LevelColorSetup has run first
        StartCoroutine(DelayedSetup());
    }
    
    private System.Collections.IEnumerator DelayedSetup()
    {
        yield return null; // Wait one frame
        FindLevelObjects();
    }
    
    private void Start()
    {
        // Backup method in case coroutine doesn't work
        if (levelRenderers == null || levelRenderers.Length == 0)
        {
            FindLevelObjects();
        }
        
        // Retry connecting to InputManager if it wasn't available in OnEnable
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager still not found in Start(), retrying connection...");
        }
        TryConnectToInputManager();
    }
    
    private void Update()
    {
        // As a last resort, keep trying to connect to InputManager until successful
        if (!inputManagerConnected && InputManager.Instance != null)
        {
            Debug.Log("LevelColorManager: InputManager found in Update, connecting now...");
            TryConnectToInputManager();
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("LevelColorManager: Setting up input callbacks");
        TryConnectToInputManager();
    }
    
    private void TryConnectToInputManager()
    {
        if (InputManager.Instance != null && !inputManagerConnected)
        {
            // Disconnect first to avoid duplicate connections
            DisconnectFromInputManager();
            
            InputManager.Instance.OnRedColorInput += OnRedColorPressed;
            InputManager.Instance.OnBlueColorInput += OnBlueColorPressed;
            InputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            inputManagerConnected = true;
            Debug.Log("LevelColorManager: Input callbacks connected successfully");
        }
        else if (InputManager.Instance == null)
        {
            inputManagerConnected = false;
            Debug.LogWarning("LevelColorManager: InputManager instance not found! Will retry in Start()");
        }
    }
    
    private void DisconnectFromInputManager()
    {
        if (InputManager.Instance != null && inputManagerConnected)
        {
            InputManager.Instance.OnRedColorInput -= OnRedColorPressed;
            InputManager.Instance.OnBlueColorInput -= OnBlueColorPressed;
            InputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            inputManagerConnected = false;
        }
    }
    
    private void OnDisable()
    {
        DisconnectFromInputManager();
    }
    
    /// <summary>
    /// Finds all level objects and caches their renderers, materials, and colliders
    /// </summary>
    private void FindLevelObjects()
    {
        GameObject levelParent = GameObject.Find("Level");
        if (levelParent == null)
        {
            Debug.LogError("Level parent object not found!");
            return;
        }
        
        SpriteRenderer[] allRenderers = levelParent.GetComponentsInChildren<SpriteRenderer>();
        BoxCollider2D[] allColliders = levelParent.GetComponentsInChildren<BoxCollider2D>();
        
        levelRenderers = allRenderers;
        levelColliders = allColliders;
        levelMaterials = new Material[levelRenderers.Length];
        
        for (int i = 0; i < levelRenderers.Length; i++)
        {
            if (levelRenderers[i].material != null)
            {
                Debug.Log($"Renderer {i}: {levelRenderers[i].name}, current material: {levelRenderers[i].material.name}, shader: {levelRenderers[i].material.shader.name}");
                levelMaterials[i] = levelRenderers[i].material;
            }
            else
            {
                Debug.LogWarning($"Renderer {i} has no material!");
            }
        }
        
        Debug.Log($"Found {levelRenderers.Length} level objects for color changing");
        Debug.Log($"Found {levelColliders.Length} level colliders for collision control");
    }
    
    /// <summary>
    /// Changes all level objects to the specified color
    /// </summary>
    private void ChangeToColor(LevelColor targetColor)
    {
        Debug.Log($"Attempting to change to color: {targetColor}, current color: {currentColor}");
        
        if (currentColor == targetColor)
        {
            Debug.Log($"Same color pressed, reverting to white");
            ChangeToColor(LevelColor.White);
            return;
        }
        
        Color colorToApply = GetColorValue(targetColor);
        currentColor = targetColor;
        
        // Apply special mechanics for red color (50% opacity and disabled collision)
        if (targetColor == LevelColor.Red)
        {
            colorToApply.a = 0.5f; // Set to 50% opacity
            Debug.Log($"Applying red color with 50% opacity and disabling collisions");
            ApplyColorToAllLevels(colorToApply);
            SetLevelCollidersEnabled(false);
        }
        else
        {
            colorToApply.a = 1.0f; // Ensure full opacity for other colors
            Debug.Log($"Applying color: {colorToApply} with full opacity and enabling collisions");
            ApplyColorToAllLevels(colorToApply);
            SetLevelCollidersEnabled(true);
        }
    }
    
    /// <summary>
    /// Gets the color value for the specified level color
    /// </summary>
    private Color GetColorValue(LevelColor levelColor)
    {
        return levelColor switch
        {
            LevelColor.Red => redColor,
            LevelColor.Blue => blueColor,
            LevelColor.Green => greenColor,
            LevelColor.White => whiteColor,
            _ => whiteColor
        };
    }
    
    /// <summary>
    /// Applies the color to all level object materials
    /// </summary>
    private void ApplyColorToAllLevels(Color color)
    {
        for (int i = 0; i < levelMaterials.Length; i++)
        {
            if (levelMaterials[i] != null)
            {
                Debug.Log($"Setting color on material {i}: {levelMaterials[i].name}, shader: {levelMaterials[i].shader.name}, alpha: {color.a}");
                levelMaterials[i].SetColor(colorPropertyName, color);
            }
            else
            {
                Debug.LogWarning($"Material {i} is null!");
            }
        }
    }
    
    /// <summary>
    /// Enables or disables all level object colliders
    /// </summary>
    private void SetLevelCollidersEnabled(bool enabled)
    {
        for (int i = 0; i < levelColliders.Length; i++)
        {
            if (levelColliders[i] != null)
            {
                levelColliders[i].enabled = enabled;
                Debug.Log($"Collider {i} ({levelColliders[i].name}): {(enabled ? "enabled" : "disabled")}");
            }
            else
            {
                Debug.LogWarning($"Collider {i} is null!");
            }
        }
        
        Debug.Log($"All level colliders {(enabled ? "enabled" : "disabled")}");
    }
    
    private void OnRedColorPressed()
    {
        Debug.Log("Red color key (E) pressed!");
        ChangeToColor(LevelColor.Red);
    }
    
    private void OnBlueColorPressed()
    {
        Debug.Log("Blue color key (R) pressed!");
        ChangeToColor(LevelColor.Blue);
    }
    
    private void OnGreenColorPressed()
    {
        Debug.Log("Green color key (T) pressed!");
        ChangeToColor(LevelColor.Green);
    }
}