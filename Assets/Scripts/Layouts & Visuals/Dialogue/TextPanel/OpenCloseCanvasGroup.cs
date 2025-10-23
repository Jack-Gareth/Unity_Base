using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class OpenCloseCanvasGroup : MonoBehaviour
{
    [Header("TextPanel")]
    protected CanvasGroup cg;
    public bool isShowing { get; protected set; }

    [SerializeField] protected bool ShowOnStart = false;
    [SerializeField] float FadeInSeconds = 1f;
    [SerializeField] float FadeOutSeconds = 0.2f;
    public UnityEvent OnOpen;
    public UnityEvent OnCloseComplete;

    protected virtual void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg.alpha == 1f) { isShowing = true; };
        if (!isShowing && ShowOnStart) { SetOpen(true); }
    }

    public void SetOpen(bool _show, Action _onComplete = null)
    {
        if (isShowing == _show) return;

        if (_show)
        {
            ShowPanel(_onComplete);
        }
        else
        {
            HidePanel(_onComplete);
        }
    }

    public void ToggleVisible(Action _onComplete = null)
    {
        SetOpen(!isShowing, _onComplete);
    }

    public void ToggleVisible()
    {
        SetOpen(!isShowing);
    }

    private void ShowPanel(Action _onComplete = null)
    {
        if (cg == null) { cg = GetComponent<CanvasGroup>(); }

        if (!gameObject.activeSelf)
        {
            cg.alpha = 0;
            gameObject.SetActive(true);
        }

        isShowing = true;

        cg.blocksRaycasts = true;
        OnOpen?.Invoke();

        StopAllCoroutines();
        StartCoroutine(FadeCG(1f, cg, FadeInSeconds, () =>
        {
            _onComplete?.Invoke();
        }));
    }

    private void HidePanel(Action _onComplete = null)
    {
        if (!gameObject.activeInHierarchy) return;

        cg.blocksRaycasts = false;

        Action onComplete = () =>
        {
            isShowing = false;
            _onComplete?.Invoke();
            gameObject.SetActive(false);
            OnCloseComplete?.Invoke();
        };

        StopAllCoroutines();
        StartCoroutine(FadeCG(0f, cg, FadeOutSeconds, onComplete));
    }

    public void ForceSetShowing(bool value)
    {
        isShowing = value;
    }

    private IEnumerator FadeCG(float targetAlpha, CanvasGroup cg, float duration, Action onComplete = null)
    {
        if (cg == null) yield break;

        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}
