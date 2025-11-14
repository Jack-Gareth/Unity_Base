using UnityEngine;
using System.Collections;

public class PlayerScaler : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField][Range(0.1f, 1f)] private float shrinkScaleFactor = 0.6f;
    [SerializeField][Range(0.01f, 2f)] private float transitionDuration = 0.3f;

    private Vector3 defaultScale;
    private Vector3 shrunkScale;
    private bool isShrunken;
    private Coroutine resizeCoroutine;
    private bool isInResizeTrigger;

    private void Awake()
    {
        defaultScale = transform.localScale;
        shrunkScale = defaultScale * shrinkScaleFactor;
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
        // Only activate in Green color mode
        if (LevelColorManager.Instance == null || LevelColorManager.Instance.CurrentColor != LevelColor.Green)
            return;

        if (!isInResizeTrigger)
            return;

        ToggleSize();
    }

    private void HandleColorChange(LevelColor newColor)
    {
        if (newColor != LevelColor.Green && isShrunken)
            ResizeToDefault();
    }

    public void SetInResizeTrigger(bool inTrigger)
    {
        isInResizeTrigger = inTrigger;
    }

    public bool IsShrunken => isShrunken;

    public void ForceResizeToDefault()
    {
        if (isShrunken)
            ResizeToDefault();
    }

    public void ToggleSize()
    {
        if (isShrunken)
            ResizeToDefault();
        else
            ShrinkPlayer();
    }

    private void ShrinkPlayer()
    {
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        resizeCoroutine = StartCoroutine(ResizeCoroutine(shrunkScale));
        isShrunken = true;
    }

    private void ResizeToDefault()
    {
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        resizeCoroutine = StartCoroutine(ResizeCoroutine(defaultScale));
        isShrunken = false;
    }

    private IEnumerator ResizeCoroutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        resizeCoroutine = null;
    }

    public void ResetSize()
    {
        if (isShrunken)
            ResizeToDefault();
    }
}
