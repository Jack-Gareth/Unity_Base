using System;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _pauseMenu;

    private float _previousTimeScale = 1f;

    public static bool IsPaused { get; private set; }

    public UnityEvent onPaused;
    public UnityEvent onResumed;

    public static event Action GamePaused;
    public static event Action GameResumed;

    private void Awake()
    {
        IsPaused = false;
        SetMenuVisible(false);
    }

    private void OnEnable()
    {
        onPaused.AddListener(InvokeGamePaused);
        onResumed.AddListener(InvokeGameResumed);
    }

    private void OnDisable()
    {
        onPaused.RemoveListener(InvokeGamePaused);
        onResumed.RemoveListener(InvokeGameResumed);
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (IsPaused) return;

        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        IsPaused = true;

        SetMenuVisible(true);
        onPaused?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;

        Time.timeScale = _previousTimeScale;
        IsPaused = false;

        SetMenuVisible(false);
        onResumed?.Invoke();
    }

    private void SetMenuVisible(bool visible)
    {
        if (_pauseMenu == null) return;

        _pauseMenu.alpha = visible ? 1f : 0f;
        _pauseMenu.interactable = visible;
        _pauseMenu.blocksRaycasts = visible;
    }

    private void InvokeGamePaused() => GamePaused?.Invoke();
    private void InvokeGameResumed() => GameResumed?.Invoke();
}
