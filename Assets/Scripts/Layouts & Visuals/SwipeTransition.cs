using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SwipeTransition : MonoBehaviour
{
    public static SwipeTransition Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float swipeDuration = 0.5f;
    [SerializeField] private float blackScreenDuration = 2f;
    [SerializeField] private AnimationCurve swipeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Canvas transitionCanvas;
    private RectTransform transitionPanel;
    private Image blackImage;
    
    private float screenWidth;
    private Vector2 offScreenLeft;
    private Vector2 onScreen;
    private Vector2 offScreenRight;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);

            CreateTransitionUI();
            SetupTransition();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void CreateTransitionUI()
    {
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);
        
        transitionCanvas = canvasObj.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 9999;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject panelObj = new GameObject("TransitionPanel");
        panelObj.transform.SetParent(canvasObj.transform);
        
        transitionPanel = panelObj.AddComponent<RectTransform>();
        transitionPanel.anchorMin = new Vector2(0, 0.5f);
        transitionPanel.anchorMax = new Vector2(0, 0.5f);
        transitionPanel.pivot = new Vector2(0, 0.5f);
        transitionPanel.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
        transitionPanel.anchoredPosition = Vector2.zero;
        
        blackImage = panelObj.AddComponent<Image>();
        blackImage.color = Color.black;
        blackImage.raycastTarget = true;
    }

    private void SetupTransition()
    {
        screenWidth = Screen.width;
        offScreenLeft = new Vector2(-screenWidth * 2, 0);
        onScreen = Vector2.zero;
        offScreenRight = new Vector2(screenWidth * 2, 0);

        transitionPanel.anchoredPosition = offScreenLeft;
        
        if (blackImage != null)
        {
            blackImage.enabled = false;
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionSequence(() => SceneManager.LoadScene(sceneName)));
        }
    }

    public void TransitionToScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionSequence(() => SceneManager.LoadScene(sceneIndex)));
        }
    }

    public void RestartCurrentScene()
    {
        if (!isTransitioning)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            StartCoroutine(TransitionSequence(() => SceneManager.LoadScene(currentScene.buildIndex)));
        }
    }

    public void PlayTransitionEffect(Action onBlackScreen)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionEffectOnly(onBlackScreen, blackScreenDuration));
        }
    }

    public void PlayTransitionEffect(Action onBlackScreen, float customBlackScreenDuration)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionEffectOnly(onBlackScreen, customBlackScreenDuration));
        }
    }

    private IEnumerator TransitionEffectOnly(Action onBlackScreen, float blackDuration)
    {
        isTransitioning = true;

        if (blackImage != null)
        {
            blackImage.enabled = true;
        }

        transitionPanel.anchoredPosition = offScreenLeft;

        yield return StartCoroutine(AnimatePosition(offScreenLeft, onScreen, swipeDuration));

        onBlackScreen?.Invoke();

        yield return new WaitForSecondsRealtime(blackDuration);

        yield return StartCoroutine(AnimatePosition(onScreen, offScreenRight, swipeDuration));

        if (blackImage != null)
        {
            blackImage.enabled = false;
        }
        
        transitionPanel.anchoredPosition = offScreenLeft;

        isTransitioning = false;
    }

    private IEnumerator TransitionSequence(Action onBlackScreen)
    {
        isTransitioning = true;

        if (blackImage != null)
        {
            blackImage.enabled = true;
        }

        transitionPanel.anchoredPosition = offScreenLeft;

        yield return StartCoroutine(AnimatePosition(offScreenLeft, onScreen, swipeDuration));

        onBlackScreen?.Invoke();

        yield return new WaitForSecondsRealtime(blackScreenDuration);

        yield return StartCoroutine(AnimatePosition(onScreen, offScreenRight, swipeDuration));

        if (blackImage != null)
        {
            blackImage.enabled = false;
        }
        
        transitionPanel.anchoredPosition = offScreenLeft;

        isTransitioning = false;
    }

    private IEnumerator AnimatePosition(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = swipeCurve.Evaluate(t);

            transitionPanel.anchoredPosition = Vector2.Lerp(from, to, curveValue);

            yield return null;
        }

        transitionPanel.anchoredPosition = to;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
