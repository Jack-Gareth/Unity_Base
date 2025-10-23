using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

//Used for displaying text in a typewriter fashion, one letter at a time. If you want to modify this class, make a new class that inherits from this one.
public class TypeWriterText : MonoBehaviour
{
    [Header("Text Behaviour")]
    [SerializeField] TMP_Text textArea;
    [SerializeField] PageCollection startingContent;
    protected List<Page> content;
    [SerializeField] float textSpeed = 0.05f;
    public GameEvent_SO onDialogueEnd;
    [SerializeField] bool disableOnFinish = true;
    protected int currentPage;
    Coroutine currentCoroutine;

    public virtual void StartDialogue()
    {
        Reset();
        if (content == null) content = startingContent.pages;
        gameObject.SetActive(true);
        currentPage = 0;
        ShowPage();
    }

    public virtual void StartDialogue(PageCollection contentToDisplay)
    {
        Reset();
        content = contentToDisplay.pages;
        gameObject.SetActive(true);
        currentPage = 0;
        ShowPage();
    }

    public void UpdateContent(PageCollection content)
    {
        this.content = content.pages;
    }

    public virtual void DisplayString(string text)
    {
        content = new List<Page>();
        content.Add(new Page(text));
        Reset();
        ShowPage();
    }

    protected virtual void OnFinish()
    {
        gameObject.SetActive(!disableOnFinish);
        currentPage = 0;
        onDialogueEnd?.Raise();
    }

    protected void Reset()
    { 
        textArea.text = "";
        currentPage = 0;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
    }

    protected virtual void PageEnd()
    {
        //Useful in case we want to do something when a page ends
    }

    //If the coroutine is active, it will skip to the end of the current page, if the page is already finished, it will skip to the next page.
    public virtual void Continue()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            textArea.maxVisibleCharacters = content[currentPage].text.Length;
            PageEnd();
            return;
        }
        NextPage();
    }

    //Ends any active coroutines and shows the next page
    public virtual void NextPage()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentPage++;
        if (currentPage > content.Count - 1)
        {
            OnFinish();
            return;
        }
        ShowPage();
    }

    protected virtual void ShowPage()
    {
        currentCoroutine = StartCoroutine(ShowTextByLetter(content[currentPage].text));
    }


    IEnumerator ShowTextByLetter(string text)
    {
        textArea.text = text;
        textArea.maxVisibleCharacters = 0;
        foreach (char letter in text.ToCharArray())
        {
            textArea.maxVisibleCharacters++;
            yield return new WaitForSeconds(textSpeed);
        }
        PageEnd();
        currentCoroutine = null;
    }
    
}
