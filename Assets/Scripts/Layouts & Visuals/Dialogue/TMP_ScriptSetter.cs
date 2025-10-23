using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMP_ScriptSetter : MonoBehaviour
{
    [SerializeField] PageCollection pageCollection;
    [SerializeField] int page;

    public void SetText()
    {
        if (pageCollection == null) return;
        if (page >= pageCollection.pages.Count)
        {
            page = pageCollection.pages.Count - 1;
            return;
        }
        else 
        {
            if (page < 0)
            {
                page = 0;
                return;
            }
        }
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = pageCollection.pages[page].text;
    }

    void OnValidate()
    {
        SetText();
    }
}

