using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outliner : MonoBehaviour
{
    public Transform objectivesParent;
    public GameObject objectivePrefab;
    public Text questText; 
    private List<GameObject> objectiveObjects = new List<GameObject>();
    private Quest selectedQuest;
    public void OutlineSelectedQuest(Quest quest)
    {
        selectedQuest = quest;
        RefreshUI();
    }

    public void RefreshUI()
    {
        gameObject.SetActive(true);
        for (int i = objectiveObjects.Count - 1; i >= 0; i--)
        {
            Destroy(objectiveObjects[i]);
        }
        objectiveObjects = new List<GameObject>();

        if(selectedQuest == null)
        {
            gameObject.SetActive(false);
            return;
        }

        questText.text = selectedQuest.title;
        for (int k = 0; k < selectedQuest.objectives.Count; k++)
        {
            GameObject objective = Instantiate(objectivePrefab, objectivesParent);
            objectiveObjects.Add(objective);
            objective.GetComponent<Text>().text = selectedQuest.objectives[k].desc;
        }
    }
}
