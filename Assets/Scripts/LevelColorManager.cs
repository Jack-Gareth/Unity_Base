using UnityEngine;
using UnityEngine.InputSystem;

public class LevelColorManager : Singleton<LevelColorManager>
{
    [Header("Color Settings")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color pinkColor = new Color(1f, 0.75f, 0.8f);
    [SerializeField] private Color brownColor = new Color(0.6f, 0.4f, 0.2f);

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

        StartCoroutine(DelayedSetup());
    }

    private System.Collections.IEnumerator DelayedSetup()
    {
        yield return null;
        FindLevelObjects();
    }

    private void Start()
    {
        if (levelRenderers == null || levelRenderers.Length == 0)
        {
            FindLevelObjects();
        }

        if (GameInputManager.Instance == null)
        {
        }
        TryConnectToInputManager();
    }

    private void Update()
    {
        if (!inputManagerConnected && GameInputManager.Instance != null)
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
        if (GameInputManager.Instance != null && !inputManagerConnected)
        {
            DisconnectFromInputManager();

            GameInputManager.Instance.OnRedColorInput += OnRedColorPressed;
            GameInputManager.Instance.OnBlueColorInput += OnBlueColorPressed;
            GameInputManager.Instance.OnGreenColorInput += OnGreenColorPressed;
            GameInputManager.Instance.OnYellowColorInput += OnYellowColorPressed;
            GameInputManager.Instance.OnPinkColorInput += OnPinkColorPressed;
            GameInputManager.Instance.OnBrownColorInput += OnBrownColorPressed;
            inputManagerConnected = true;
        }
        else if (GameInputManager.Instance == null)
        {
            inputManagerConnected = false;
        }
    }

    private void DisconnectFromInputManager()
    {
        if (GameInputManager.Instance != null && inputManagerConnected)
        {
            GameInputManager.Instance.OnRedColorInput -= OnRedColorPressed;
            GameInputManager.Instance.OnBlueColorInput -= OnBlueColorPressed;
            GameInputManager.Instance.OnGreenColorInput -= OnGreenColorPressed;
            GameInputManager.Instance.OnYellowColorInput -= OnYellowColorPressed;
            GameInputManager.Instance.OnPinkColorInput -= OnPinkColorPressed;
            GameInputManager.Instance.OnBrownColorInput -= OnBrownColorPressed;
            inputManagerConnected = false;
        }
    }

    private void OnDisable()
    {
        DisconnectFromInputManager();
    }

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

        PlayerEvents.TriggerColorChange(currentColor);
    }

    private Color GetColorValue(LevelColor levelColor)
    {
        return levelColor switch
        {
            LevelColor.Red => redColor,
            LevelColor.Blue => blueColor,
            LevelColor.Green => greenColor,
            LevelColor.Yellow => yellowColor,
            LevelColor.Pink => pinkColor,
            LevelColor.Brown => brownColor,
            LevelColor.White => whiteColor,
            _ => whiteColor
        };
    }

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

    private void OnYellowColorPressed()
    {
        ChangeToColor(LevelColor.Yellow);
    }

    private void OnPinkColorPressed()
    {
        ChangeToColor(LevelColor.Pink);
    }

    private void OnBrownColorPressed()
    {
        ChangeToColor(LevelColor.Brown);
    }

    public void ResetToWhite()
    {
        if (currentColor != LevelColor.White)
        {
            ChangeToColor(LevelColor.White);
        }
    }
}
