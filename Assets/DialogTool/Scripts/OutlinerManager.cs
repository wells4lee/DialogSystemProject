using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlinerManager : MonoBehaviour
{
    public Text headerText;
    public Transform objectivesParent;
    public Text objectiveTextPrefab;
    public ObjectiveObject starterObjective;
    private List<ObjectiveObject> objectiveList = new List<ObjectiveObject>();
    private List<Text> objectiveUIObjects = new List<Text>();

    // Start is called before the first frame update
    void Start()
    {
        SetOutliner("Day 1", starterObjective);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOutliner(string header, List<ObjectiveObject> objectives)
    {
        headerText.text = header;
        for(int i = 0; i < objectives.Count; i++)
        {
            objectiveList.Add(Instantiate(objectives[i]));
            //objectiveList[objectiveList.Count - 1].targetEvent.RegisterListener();
            objectiveUIObjects.Add(Instantiate(objectiveTextPrefab, objectivesParent));
            objectiveUIObjects[i].text = objectiveList[i].desc;

        }
    }

    public void SetOutliner(string header, ObjectiveObject objective)
    {
        headerText.text = header;
        objectiveList.Add(Instantiate(objective));
        objectiveUIObjects.Add(Instantiate(objectiveTextPrefab, objectivesParent));
        objectiveUIObjects[objectiveUIObjects.Count-1].text = objectiveList[objectiveUIObjects.Count - 1].desc;
    }

}
