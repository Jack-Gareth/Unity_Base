using UnityEngine;
using UnityEngine.InputSystem;

public class LevelColorManager : Singleton<LevelColorManager>
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
    private BoxCollider2D[] levelColliders;
    private LevelColor currentColor = LevelColor.White;
    private bool inputManagerConnected = false;

    public LevelColor CurrentColor => currentColor;

    protected override void Awake()
    {
        base.Awake();

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
        if (levelRenderers == null || levelRenderers.Length == 0)
        {
            FindLevelObjects();
        }

        if (InputManager.Instance == null)
        {
            // InputManager not ready yet, will retry in Update
        }
        TryConnectToInputManager();
    }

    private void Update()
    {
        if (!inputManagerConnected && InputManager.Instance != null)
        {
            TryConnectToInputManager();
        }
    }

    private void OnEnable()
    {
        TryConnectToInputManager();
    }

    private void TryConnectToInputManager()
    {
        if (InputManager.Instance != null && !inputManagerConnected)
        {
            DisconnectFromInputManager();

            InputManager.Instance.OnRedColorInput += OnRedColorPressed;
            InputManager.Instance.OnBlueColorInput += OnBlueColorPressed;
            InputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            inputManagerConnected = true;
        }
        else if (InputManager.Instance == null)
        {
            inputManagerConnected = false;
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
                levelMaterials[i] = levelRenderers[i].material;
            }
        }
    }

    /// <summary>
    /// Changes all level objects to the specified color
    /// </summary>
    private void ChangeToColor(LevelColor targetColor)
    {
        if (currentColor == targetColor)
        {
            ChangeToColor(LevelColor.White);
            return;
        }

        Color colorToApply = GetColorValue(targetColor);
        currentColor = targetColor;

        if (targetColor == LevelColor.Red)
        {
            colorToApply.a = 0.5f;
            ApplyColorToAllLevels(colorToApply);
            SetLevelCollidersEnabled(false);
        }
        else
        {
            colorToApply.a = 1.0f;
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
                levelMaterials[i].SetColor(colorPropertyName, color);
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
            }
        }
    }

    private void OnRedColorPressed()
    {
        ChangeToColor(LevelColor.Red);
    }

    private void OnBlueColorPressed()
    {
        ChangeToColor(LevelColor.Blue);
    }

    private void OnGreenColorPressed()
    {
        ChangeToColor(LevelColor.Green);
    }
}
