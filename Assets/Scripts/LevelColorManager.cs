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
    [Header("Color Settings")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color whiteColor = Color.white;
    
    [Header("Shader Settings")]
    [SerializeField] private string colorPropertyName = "_Color";
    
    private SpriteRenderer[] levelRenderers;
    private Material[] levelMaterials;
    private LevelColor currentColor = LevelColor.White;
    
    private void Awake()
    {
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
    }
    
    private void OnEnable()
    {
        Debug.Log("LevelColorManager: Setting up input callbacks");
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnRedColorInput += OnRedColorPressed;
            InputManager.Instance.OnBlueColorInput += OnBlueColorPressed;
            InputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            Debug.Log("LevelColorManager: Input callbacks connected");
        }
        else
        {
            Debug.LogWarning("LevelColorManager: InputManager instance not found!");
        }
    }
    
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnRedColorInput -= OnRedColorPressed;
            InputManager.Instance.OnBlueColorInput -= OnBlueColorPressed;
            InputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
        }
    }
    
    /// <summary>
    /// Finds all level objects and caches their renderers and materials
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
        levelRenderers = allRenderers;
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
        
        Debug.Log($"Applying color: {colorToApply} to {levelMaterials.Length} materials");
        ApplyColorToAllLevels(colorToApply);
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
                Debug.Log($"Setting color on material {i}: {levelMaterials[i].name}, shader: {levelMaterials[i].shader.name}");
                levelMaterials[i].SetColor(colorPropertyName, color);
            }
            else
            {
                Debug.LogWarning($"Material {i} is null!");
            }
        }
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