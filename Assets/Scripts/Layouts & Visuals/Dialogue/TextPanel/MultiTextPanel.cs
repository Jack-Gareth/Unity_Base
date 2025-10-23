using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class MultiTextPanel : OpenCloseCanvasGroup
{

    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text contentArea;
    [SerializeField] Image imageArea;
    [SerializeField] PageCollection startContent;
    [SerializeField] Button nextButton;
    [SerializeField] Button backButton;
    List<Page> content;
    AudioSource audioSource;
    public int currentContent;
    


    // Start is called before the first frame update
    private void Start()
    {      
        if (startContent != null) content = startContent.pages;
        else content = new List<Page>();
        audioSource = GetComponent<AudioSource>();
        UpdatePage();
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(BackPage);
        OnCloseComplete.AddListener(Reset);
    }

    private void NextPage()
    {
        if (currentContent < content.Count - 1)
        {
            currentContent++;
            UpdatePage();
        }
        else
        {
            SetOpen(false);
        }
    }

    private void BackPage()
    {
        if (currentContent > 0)
        {
            currentContent--;
            UpdatePage();
        }
    }


    /// <summary>
    /// Replace the string array content, also resets back to 0
    /// </summary>
    /// <param name="text"></param>
    public void UpdateContent(PageCollection contentObj)
    {
        content = contentObj.pages;
        currentContent = 0;
        contentArea.text = content[0].text;
        imageArea.sprite = content[0].image;
    }

    public void UpdateContent(string contentText)
    {
        content = new List<Page>();
        content.Add(new Page(contentText));
        contentArea.text = contentText;
    }

    void PlaySound(AudioClip ac)
    {
        audioSource.Stop();
        if (ac)
        {
            //audioSource.clip = ac;
            audioSource.PlayOneShot(ac);
        }
    }

    void UpdatePage()
    {
        title.text = content[currentContent].title;
        contentArea.text = content[currentContent].text;
        if (content[currentContent].image) { imageArea.sprite = content[currentContent].image; imageArea.gameObject.SetActive(true); }
        else { imageArea.gameObject.SetActive(false); }
        if (isShowing) PlaySound(content[currentContent].sound);
        RefreshButtonText();
    }

    private void RefreshButtonText()
    {
        if (currentContent == content.Count - 1)
        {
            nextButton.GetComponentInChildren<TMP_Text>().text = "Close";
        }
        else
        {
            nextButton.GetComponentInChildren<TMP_Text>().text = "Next";
        }

        if (currentContent == 0)
        {
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }
    }

    private void Reset()
    {
        currentContent = 0;
        title.text = content[currentContent].title;
        contentArea.text = content[currentContent].text;
        if (content[currentContent].image) { imageArea.sprite = content[currentContent].image; imageArea.gameObject.SetActive(true); }
        else { imageArea.gameObject.SetActive(false); }
        RefreshButtonText();
    }

    public void OpenPanel()
    {
        base.SetOpen(true);
    }

 

}




