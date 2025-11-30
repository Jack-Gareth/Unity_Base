using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// [RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(OpenCloseCanvasGroup))]
public class PopupPanel : TypeWriterText, IPointerClickHandler
{
    //[SerializeField] private Image _Icon;
    [SerializeField] private Image continueIcon;

    [SerializeField] private bool clickToContinue = true;

    private AudioSource audioSource;
    private OpenCloseCanvasGroup panelCanvas;
    private Coroutine flashRoutine;

    private void Awake()
    {
        panelCanvas = GetComponent<OpenCloseCanvasGroup>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnContinueDialogueInput += OnControllerContinue;
        }
    }

    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnContinueDialogueInput -= OnControllerContinue;
        }
    }

    private void OnControllerContinue()
    {
        if (panelCanvas.isShowing)
        {
            Continue();
        }
    }

    private void Start()
    {
        continueIcon.enabled = false;
    }

    public void ShowMessage(string message, Sprite characterSprite = null, AudioClip audioClip = null)
    {
        content = new List<Page> { new Page(message) };

        Reset();
        StopFlashing();

        continueIcon.enabled = false;

        // _Icon.enabled = characterSprite != null;
        // if (_Icon != null)
        //     _Icon.sprite = characterSprite;

        if (audioClip != null)
            content[0].sound = audioClip;

        panelCanvas.SetOpen(true);
        ShowPage();
    }

    protected override void ShowPage()
    {
        base.ShowPage();

        if (content[currentPage].image != null)
        {
            //  _Icon.sprite = content[currentPage].image;
            // _Icon.enabled = true;
        }

        if (continueIcon != null)
        {
            continueIcon.enabled = false;
            continueIcon.gameObject.SetActive(true);
            continueIcon.canvasRenderer.SetAlpha(0f);
        }
    }

    private void ShowContinueIcon()
    {
        if (continueIcon == null)
            return;

        continueIcon.enabled = true;
        continueIcon.canvasRenderer.SetAlpha(1f);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashContinueIcon());
    }

    private IEnumerator FlashContinueIcon()
    {
        while (true)
        {
            continueIcon.canvasRenderer.SetAlpha(1f);
            yield return new WaitForSeconds(0.5f);
            continueIcon.canvasRenderer.SetAlpha(0f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void StopFlashing()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (continueIcon != null)
        {
            continueIcon.canvasRenderer.SetAlpha(0f);
            continueIcon.enabled = false;
        }
    }

    public override void Continue()
    {
        if (!panelCanvas.isShowing)
            return;

        StopFlashing();
        base.Continue();
    }

    public override void NextPage()
    {
        content[currentPage].gameEvent?.Raise();
        base.NextPage();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (content[currentPage].isUnskippable)
            return;

        if (clickToContinue)
            Continue();
    }

    public void PlayPageAudio()
    {
        if (content[currentPage].sound != null)
            audioSource.PlayOneShot(content[currentPage].sound);
    }

    protected override void PageEnd()
    {
        base.PageEnd();

        if (!content[currentPage].isUnskippable)
        {
            ShowContinueIcon(); // Start flashing only after typewriter finishes
        }
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        StopFlashing();
        panelCanvas.SetOpen(false);
    }

    public bool IsPopupActive => panelCanvas.isShowing;
}
