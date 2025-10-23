using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class EventOnCollision : MonoBehaviour
{
    public event Action<string> OnCollisionEvent;
    public UnityEvent<string> OnCollisionEventUnity;

    //You can use this script to allow other game objects to subscribe to the OnCollisionEnter event of this game object

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEvent?.Invoke(collision.collider.tag);
        OnCollisionEventUnity?.Invoke(collision.collider.tag);
    }
}
