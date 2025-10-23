using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable][CreateAssetMenu(fileName = "Data", menuName = "TextPanel/Page Collection", order = 1)]
public class PageCollection : ScriptableObject
{
    public List<Page> pages;

    public PageCollection()
    {
        pages = new List<Page>();
    }

    public void UpdatePage(int i, Page page)
    {
        pages[i] = page;
    }
}

[Serializable]
public class Page
    {
        public string title;
        public Sprite image;
        [TextArea] public string text;
        public AudioClip sound;
        public GameEvent_SO gameEvent;
        public bool isUnskippable = false;

        public Page(string text)
        {
            this.text = text;
            title = "";
            image = null;
            sound = null;
            gameEvent = null;
        }

        public Page(string title, string text)
        {
            this.title = title;
            this.text = text;
            image = null;
            sound = null;
            gameEvent = null;
        }

        public Page(string title, string text, Sprite image)
        {
            this.title = title;
            this.text = text;
            this.image = image;
            sound = null;
            gameEvent = null;
        }

        public Page(string title, string text, Sprite image, AudioClip sound)
        {
            this.title = title;
            this.text = text;
            this.image = image;
            this.sound = sound;
            gameEvent = null;
        }

        public Page(string title, string text, Sprite image, AudioClip sound, GameEvent_SO gameEvent)
        {
            this.title = title;
            this.text = text;
            this.image = image;
            this.sound = sound;
            this.gameEvent = gameEvent;
        }
    }
