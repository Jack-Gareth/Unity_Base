using UnityEngine;
using System;

public class AudioEvents : Singleton<AudioEvents>
{
    public event Action OnButtonPress;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
            DontDestroyOnLoad(gameObject);
    }

    public void RaiseCorrectLabel() => OnButtonPress?.Invoke();
}
