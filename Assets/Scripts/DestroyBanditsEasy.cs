using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class DestroyBanditsEasy : Quest
{
    public int killCount;
    public int killRequirement;

    public DestroyBanditsEasy()
    {
        id = 1;
        title = "Patrol the Farmlands";
        desc = "There have been reports of bandits moving into the area. You are tasked with destroying atleast 3 of them.";
        killCount = 0;
        killRequirement = 3;
    }

    public override void CompleteQuest()
    {
        ObjectStorage.Instance.ledger.RemoveQuest(this);
    }

    public override void OnEnemyDeath()
    {
        if(++killCount >= killRequirement)
        {
            currentStage = 10;

        }

        RefreshQuest();
    }

    public override void RefreshQuest() // Add an ObjectiveLoader object to update objective outliner
    {
        switch (currentStage)
        {
            case 0:
                objectives = new List<Objective>();
                objectives.Add(new Objective(this, "Destroy 3 Bandits " + killCount + "/" + killRequirement, killCount, killRequirement));
                break;

            case 10:
                CompleteQuest();
                break;
        }

        ObjectStorage.Instance.outliner.RefreshUI();
    }


}
