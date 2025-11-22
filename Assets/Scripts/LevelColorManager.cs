using UnityEngine;
using System.Collections;
using System.Linq;

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

    [Header("Red Phase Ability Settings")]
    [SerializeField] private float redPhaseDuration = 1f;

    private SpriteRenderer[] levelRenderers;
    private Material[] levelMaterials;
    private BoxCollider2D[] levelColliders;

    private LevelColor currentColor = LevelColor.White;
    private bool inputConnected;
    private bool isRedPhaseActive;
    private Coroutine redPhaseCoroutine;

    private readonly LevelColor[] cyclableColors =
    {
        LevelColor.Red, LevelColor.Blue, LevelColor.Green,
        LevelColor.Yellow, LevelColor.Pink, LevelColor.Brown
    };

    public LevelColor CurrentColor => currentColor;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(DelayedSetup());
    }

    private IEnumerator DelayedSetup()
    {
        yield return null;
        FindLevelObjects();
        TryConnectInput();
    }

    private void Update()
    {
        if (!inputConnected && GameInputManager.Instance != null)
            TryConnectInput();
    }

    private void OnEnable()
    {
        TryConnectInput();
        PlayerEvents.OnPlayerRespawn += ResetToWhite;
    }

    private void OnDisable()
    {
        DisconnectInput();
        PlayerEvents.OnPlayerRespawn -= ResetToWhite;
    }

    private void TryConnectInput()
    {
        if (inputConnected || GameInputManager.Instance == null)
            return;

        var input = GameInputManager.Instance;
        input.OnColorChangeInput += HandleColorChange;
        input.OnRedPhaseAbilityInput += HandleRedPhaseAbility;
        input.OnCycleColorLeftInput += () => CycleColor(-1);
        input.OnCycleColorRightInput += () => CycleColor(1);
        input.OnActivateAbilityInput += HandleRedPhaseAbility;
        input.OnResetToWhiteInput += ResetToWhite;

        inputConnected = true;
    }

    private void DisconnectInput()
    {
        if (!inputConnected || GameInputManager.Instance == null)
            return;

        var input = GameInputManager.Instance;
        input.OnColorChangeInput -= HandleColorChange;
        input.OnRedPhaseAbilityInput -= HandleRedPhaseAbility;
        input.OnCycleColorLeftInput -= () => CycleColor(-1);
        input.OnCycleColorRightInput -= () => CycleColor(1);
        input.OnActivateAbilityInput -= HandleRedPhaseAbility;
        input.OnResetToWhiteInput -= ResetToWhite;

        inputConnected = false;
    }

    // --- Color System ---

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor == currentColor)
            ChangeToColor(LevelColor.White);
        else
            ChangeToColor(newColor);
    }

    private void ChangeToColor(LevelColor target)
    {
        currentColor = target;
        Color c = GetColor(target);
        ApplyColor(c);
        SetCollidersEnabled(true);
        PlayerEvents.TriggerColorChange(currentColor);
    }

    private void ApplyColor(Color c)
    {
        foreach (var mat in levelMaterials)
            if (mat != null) mat.SetColor(colorPropertyName, c);
    }

    private Color GetColor(LevelColor lc) => lc switch
    {
        LevelColor.Red => redColor,
        LevelColor.Blue => blueColor,
        LevelColor.Green => greenColor,
        LevelColor.Yellow => yellowColor,
        LevelColor.Pink => pinkColor,
        LevelColor.Brown => brownColor,
        _ => whiteColor
    };

    // --- Red Phase ---

    private void HandleRedPhaseAbility()
    {
        if (currentColor != LevelColor.Red) return;

        if (redPhaseCoroutine != null)
            StopCoroutine(redPhaseCoroutine);

        redPhaseCoroutine = StartCoroutine(RedPhaseRoutine());
    }

    private IEnumerator RedPhaseRoutine()
    {
        isRedPhaseActive = true;

        var transparent = redColor;
        transparent.a = 0.5f;
        ApplyColor(transparent);
        SetCollidersEnabled(false);

        yield return new WaitForSecondsRealtime(redPhaseDuration);

        isRedPhaseActive = false;

        if (currentColor == LevelColor.Red)
        {
            var solid = redColor;
            solid.a = 1f;
            ApplyColor(solid);
            SetCollidersEnabled(true);
        }

        redPhaseCoroutine = null;
    }

    // --- Reset / Respawn ---

    public void ResetToWhite()
    {
        if (redPhaseCoroutine != null)
            StopCoroutine(redPhaseCoroutine);

        isRedPhaseActive = false;
        currentColor = LevelColor.White;

        ApplyColor(whiteColor);
        SetCollidersEnabled(true);
        PlayerEvents.TriggerColorChange(LevelColor.White);
    }

    // --- Utilities ---

    private void FindLevelObjects()
    {
        int ground = LayerMask.NameToLayer("Ground");
        int wall = LayerMask.NameToLayer("Wall");

        levelRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
            .Where(r => r.gameObject.layer == ground || r.gameObject.layer == wall)
            .ToArray();

        levelColliders = FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None)
            .Where(c => c.gameObject.layer == ground || c.gameObject.layer == wall)
            .ToArray();

        levelMaterials = levelRenderers.Select(r => r.material).ToArray();
    }

    private void SetCollidersEnabled(bool enable)
    {
        foreach (var col in levelColliders)
            if (col != null) col.enabled = enable;
    }

    private void CycleColor(int dir)
    {
        int i = System.Array.IndexOf(cyclableColors, currentColor);
        if (i == -1) i = 0;
        else i = (i + dir + cyclableColors.Length) % cyclableColors.Length;

        ChangeToColor(cyclableColors[i]);
    }
}
