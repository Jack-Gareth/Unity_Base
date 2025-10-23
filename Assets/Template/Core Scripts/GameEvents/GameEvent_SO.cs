using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "Events/Game Event", order = 51)]
public class GameEvent_SO : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();
    public event System.Action OnEventRaised;

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
        OnEventRaised?.Invoke();
    }

    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
