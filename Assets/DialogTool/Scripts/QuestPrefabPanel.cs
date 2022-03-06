using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPrefabPanel : MonoBehaviour
{

    private QuestLedger ledger;
    private Quest quest;

    public void InitQuestPanel(QuestLedger parentLedger, Quest assignedQuest, bool selected = false)
    {
        if (selected)
        {
            GetComponent<Button>().Select();
        }
        ledger = parentLedger;
        quest = assignedQuest;
        gameObject.GetComponentInChildren<Text>().text = quest.title;
    }

    public void OnPanelClicked()
    {
        ledger.SelectQuest(quest);
    }

}
