using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Quest : ScriptableObject
{
    private DialogLoader dialogLoader;

    public int id;
    public string title;
    public string desc;
    public int lastStage = 10;
    public int currentStage;
    public List<Objective> objectives;
    public void Init(int customStage = 0)
    {   
        SetQuestStage(customStage);
    }

    public void SetQuestStage(int s)
    {
        currentStage = s;
        if(currentStage >= lastStage)
        {
            CompleteQuest();
        }
        RefreshQuest();
    }

    public abstract void RefreshQuest();

    public abstract void CompleteQuest();

    public abstract void OnEnemyDeath();
}


