using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeLoadingScreen : MonoBehaviour
{

    [SerializeField] CanvasGroup cg;
    [SerializeField] bool StartFade;
    [Range(0.1f, 3.0f)][SerializeField] internal float defaultAnimationTime = 1.0f;
    [SerializeField] LevelManager lm;


    public void OnEnable()
    {
        lm.StartScreenHide += CoverScreen;
        
    }

    public void OnDisable()
    {
        lm.StartScreenHide -= CoverScreen;
    }

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = StartFade ? 1f : 0f;
        if (StartFade) UncoverScreen();
    }


    public void UncoverScreen(Action _onComplete = null)
    {
        cg.alpha = 1f;
        StopAllCoroutines();
        StartCoroutine(FadeCG(0f, cg, defaultAnimationTime, () => { _onComplete?.Invoke(); lm.InvokeScreenVisible(); }));
    }

    public void CoverScreen(Action _onComplete = null)
    {
        cg.alpha = 0f;
        StopAllCoroutines();
        StartCoroutine(FadeCG(1f, cg, defaultAnimationTime, _onComplete));
    }

    IEnumerator FadeCG(float targetAlpha, CanvasGroup cg, float fadeTime, Action _onComplete = null)
    {
        float timeElapsed = 0f;
        float originalAlpha = cg.alpha;
        while (cg.alpha != targetAlpha)
        {
            timeElapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(originalAlpha, targetAlpha, (timeElapsed / fadeTime));
            if (Mathf.Abs(cg.alpha - targetAlpha) < 0.01f) cg.alpha = targetAlpha;
            yield return null;
        }
        _onComplete?.Invoke();

    }



}
