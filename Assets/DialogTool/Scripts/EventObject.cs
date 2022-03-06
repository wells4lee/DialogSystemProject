using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EventObject : ScriptableObject
{
    private List<Action> listeners = new List<Action>();

    public void Raise()
    {
        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i]();
        }
    }

    public void RegisterListener(Action listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(Action listener)
    {
        listeners.Remove(listener);
    }
}
