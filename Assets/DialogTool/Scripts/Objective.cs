using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective
{
    public bool completed;
    public string desc;
    public Quest parentQuest;
    public int currentCount;
    public int maxCount;
    public Objective(Quest quest, string description, int current = 0, int max = -1, bool finished = false)
    {
        parentQuest = quest;
        desc = description;
        completed = finished;
        currentCount = current;
        maxCount = max;

    }

}
