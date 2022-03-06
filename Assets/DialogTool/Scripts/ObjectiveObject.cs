using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class ObjectiveObject : ScriptableObject
{
    private bool passed;
    private bool failed;
    private bool completed;
    public string desc;
    public int count = 1; // How many times something must be done

    public EventObject targetEvent;


}
