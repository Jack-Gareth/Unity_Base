using UnityEngine;
using System.Collections;

public class PlayerScaler : MonoBehaviour
{
    [Header("Scaling")]
    [SerializeField] private Transform visualModel;
    [SerializeField][Range(0.1f, 1f)] private float shrinkScaleFactor = 0.6f;
    [SerializeField][Range(0.05f, 2f)] private float resizeDuration = 0.25f;

    private Vector3 defaultScale;
    private Vector3 shrunkScale;
    private bool isShrunken = false;
    private Coroutine scaleRoutine;

    private void Awake()
    {
        if (visualModel == null)
            visualModel = transform; // fallback

        defaultScale = visualModel.localScale;
        shrunkScale = defaultScale * shrinkScaleFactor;
    }

    private void OnEnable()
    {
        PlayerEvents.OnColorAbility += OnAbilityPressed;
        PlayerEvents.OnColorChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        PlayerEvents.OnColorAbility -= OnAbilityPressed;
        PlayerEvents.OnColorChanged -= OnColorChanged;
    }

    private void OnAbilityPressed()
    {
        // Only works if Green
        if (LevelColorManager.Instance.CurrentColor != LevelColor.Green)
            return;

        ToggleSize();
    }

    private void OnColorChanged(LevelColor newColor)
    {
        if (newColor != LevelColor.Green && isShrunken)
            Resize(defaultScale, false);
    }

    private void ToggleSize()
    {
        if (isShrunken)
            Resize(defaultScale, false);
        else
            Resize(shrunkScale, true);
    }

    private void Resize(Vector3 target, bool toShrunken)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        isShrunken = toShrunken;
        scaleRoutine = StartCoroutine(SmoothScale(target));
    }

    private IEnumerator SmoothScale(Vector3 target)
    {
        Vector3 start = visualModel.localScale;
        float t = 0f;

        while (t < resizeDuration)
        {
            t += Time.deltaTime;
            visualModel.localScale = Vector3.Lerp(start, target, t / resizeDuration);
            yield return null;
        }

        visualModel.localScale = target;
        scaleRoutine = null;
    }

    public void ResetSize()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        isShrunken = false;
        visualModel.localScale = defaultScale;
    }

}
