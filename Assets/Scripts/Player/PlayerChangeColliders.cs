using UnityEngine;
using System.Collections;

public class PlayerChangeColliders : MonoBehaviour
{
    [SerializeField] private float redPhaseDuration = 1f;
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private string colorPropertyName = "_Color";

    private SpriteRenderer[] levelRenderers;
    private Material[] levelMaterials;
    private BoxCollider2D[] levelColliders;
    private Coroutine redPhaseCoroutine;
    private bool isRedPhaseActive;

    private void Awake()
    {
        FindLevelObjects();
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += HandleAbilityInput;
        PlayerEvents.OnColorChanged += HandleColorChange;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= HandleAbilityInput;
        PlayerEvents.OnColorChanged -= HandleColorChange;
    }

    private void HandleAbilityInput()
    {
        // Only trigger if we are in Red color mode
        if (LevelColorManager.Instance == null) return;
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Red) return;

        // Restart red phase if pressed again
        if (redPhaseCoroutine != null)
            StopCoroutine(redPhaseCoroutine);

        redPhaseCoroutine = StartCoroutine(RedPhaseRoutine());
    }

    private void HandleColorChange(LevelColor newColor)
    {
        // If we leave Red mode, end the red phase instantly
        if (newColor != LevelColor.Red && isRedPhaseActive)
        {
            StopAllCoroutines();
            EndRedPhase();
            isRedPhaseActive = false;
        }
    }

    private IEnumerator RedPhaseRoutine()
    {
        isRedPhaseActive = true;

        // Make red transparent and disable colliders
        var transparent = redColor;
        transparent.a = 0.5f;
        ApplyColor(transparent);
        SetCollidersEnabled(false);

        yield return new WaitForSecondsRealtime(redPhaseDuration);

        EndRedPhase();
        isRedPhaseActive = false;
        redPhaseCoroutine = null;
    }

    private void EndRedPhase()
    {
        // Restore solid red and re-enable colliders
        var solid = redColor;
        solid.a = 1f;
        ApplyColor(solid);
        SetCollidersEnabled(true);
    }

    private void ApplyColor(Color color)
    {
        foreach (var mat in levelMaterials)
        {
            if (mat != null)
                mat.SetColor(colorPropertyName, color);
        }
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (var col in levelColliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }

    private void FindLevelObjects()
    {
        int ground = LayerMask.NameToLayer("Ground");
        int wall = LayerMask.NameToLayer("Wall");

        var renderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var colliders = FindObjectsByType<BoxCollider2D>(FindObjectsSortMode.None);

        var filteredRenderers = new System.Collections.Generic.List<SpriteRenderer>();
        var filteredColliders = new System.Collections.Generic.List<BoxCollider2D>();

        foreach (var r in renderers)
            if (r.gameObject.layer == ground || r.gameObject.layer == wall)
                filteredRenderers.Add(r);

        foreach (var c in colliders)
            if (c.gameObject.layer == ground || c.gameObject.layer == wall)
                filteredColliders.Add(c);

        levelRenderers = filteredRenderers.ToArray();
        levelColliders = filteredColliders.ToArray();

        levelMaterials = new Material[levelRenderers.Length];
        for (int i = 0; i < levelRenderers.Length; i++)
            levelMaterials[i] = levelRenderers[i].material;
    }
}
