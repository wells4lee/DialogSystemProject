using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class QuestLedger : MonoBehaviour
{
    public List<Quest> currentQuests = new List<Quest>();
    

    public Transform logPanelTransform;
    public GameObject QuestPrefabPanel;
    public Outliner outliner;
    public Text titleText;
    public Text descText;
    public Text objectivesText;
    public Text rewardsText;
    public Type[] referencedQuests = new Type[1] { typeof(DestroyBanditsEasy) };

    private List<GameObject> prefabPanels = new List<GameObject>();
    private Quest SelectedQuest;

    /// <summary>
    /// Adds the quest to the ledger and updates
    /// </summary>
    /// <param name="newQuest"></param>
    public void AcceptQuest(Quest newQuest)
    {
        newQuest.Init();
        currentQuests.Add(newQuest);
        if(SelectedQuest == null)
        {
            SelectQuest(newQuest);
        }
        RefreshUI();
    }

    /// <summary>
    /// Removes the currently selected quest
    /// </summary>
    public void RemoveQuest()
    {
        if(SelectedQuest != null)
        {
            currentQuests.Remove(SelectedQuest);
            ClearSelectedQuest();
        }
        RefreshUI();
    }
    /// <summary>
    /// Removes quest from ledger if it exists and is selected
    /// </summary>
    /// <param name="quest"></param>
    public void RemoveQuest(Quest quest)
    {

        if (quest != null)
        {
            if (SelectedQuest != null && SelectedQuest == quest)
            {
                ClearSelectedQuest();
            }
            currentQuests.Remove(quest);
        }
        RefreshUI();
    }
    /// <summary>
    /// Loads quest information into details panel of ledger
    /// Updates outliner with objectives
    /// </summary>
    /// <param name="quest"></param>
    public void SelectQuest(Quest quest)
    {
        if(quest == null)
        {
            ClearSelectedQuest();
            return;
        }
        SelectedQuest = quest;
        titleText.text = SelectedQuest.title;
        descText.text = SelectedQuest.desc;
        StringBuilder tmpStr = new StringBuilder(SelectedQuest.objectives.Count * 32 + 32);

        for(int i = 0; i < SelectedQuest.objectives.Count; i++)
        {
            tmpStr.Append("-- ");
            tmpStr.Append(SelectedQuest.objectives[i].desc);
            tmpStr.Append("\n");
        }
        objectivesText.text = tmpStr.ToString();
        rewardsText.text = "";
        outliner.OutlineSelectedQuest(SelectedQuest);
    }

    /// <summary>
    /// Resets Ledger text to empty
    /// Clears outliner
    /// </summary>
    public void ClearSelectedQuest()
    {
        SelectedQuest = null;
        titleText.text = "";
        descText.text = "";
        objectivesText.text = "";
        rewardsText.text = "";
        outliner.OutlineSelectedQuest(null);
        RefreshUI();
    }

    /// <summary>
    /// Recreate the UI objects
    /// </summary>
    public void RefreshUI()
    {
        for(int i = prefabPanels.Count - 1; i >= 0; i--)
        {
            Destroy(prefabPanels[i].gameObject);
        }
        prefabPanels = new List<GameObject>();

        for(int k = 0; k < currentQuests.Count; k++)
        {
           GameObject panel = Instantiate(QuestPrefabPanel, logPanelTransform);
            panel.GetComponent<QuestPrefabPanel>().InitQuestPanel(this, currentQuests[k], SelectedQuest == currentQuests[k]);
            prefabPanels.Add(panel);
            // add prefab panel info update
            //panel.GetComponentInChildren<Text>().text = currentQuests[k].title;
        }
    }

    public void CloseLedger()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        RefreshUI();
    }

}
