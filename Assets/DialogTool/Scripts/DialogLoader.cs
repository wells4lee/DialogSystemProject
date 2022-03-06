using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogLoader : MonoBehaviour
{
    
    public Text promptField;
    public GameObject responseButtonPrefab;
    public GameObject responseParent;
    // Can add objects to UIObjects for dialog that opens other menus
    public List<GameObject> UIObjects;
    [HideInInspector]
    public SaveableListOfNodes currentDialogObject;
    private List<SaveableNode> dialogTree;
    private SaveableNode currentNode;

    /// <summary>
    /// Updates the dialogBoxes with the first node of the dialog tree
    /// </summary>
    /// <param name="dialogObj"></param>
    public void Init(SaveableListOfNodes dialogObj)
    {
        currentDialogObject = dialogObj;
        dialogTree = currentDialogObject.saveableNodeList;
        currentNode = dialogTree[0];
        UpdateToNewDialog();
    }

    /// <summary>
    /// Clears previous Ui
    /// Creates buttons for responses
    /// Changes response interactability based on if the restrictions are met
    /// Adds listeners to interactable buttons that can call other scripts/methods
    /// </summary>
    public void UpdateToNewDialog()
    {
        ClearDialog();
        promptField.text = currentNode.promptText;

        if (currentNode.nodeResponses.Count == 0)
        {
            GameObject newButton = Instantiate(responseButtonPrefab, responseParent.transform);
            newButton.GetComponentInChildren<Text>().text = "Proceed";
            newButton.GetComponent<Button>().onClick.AddListener(() => ClearDialog(-1));
        }
        else
        {
            for (int i = 0; i < currentNode.nodeResponses.Count; i++)
            {
                GameObject newButton = Instantiate(responseButtonPrefab, responseParent.transform);
                newButton.GetComponentInChildren<Text>().text = currentNode.nodeResponses[i].responseText;
                if (currentNode.nodeResponses[i].restrictions != null)
                {
                    bool insufficient = false;
                    for (int k = 0; k < currentNode.nodeResponses[i].restrictions.Count; k++)
                    {
                        
                        //Modify commands
                        if (currentNode.nodeResponses[i].restrictions[k].command == Commands.MODIFY)
                        {
                            switch (currentNode.nodeResponses[i].restrictions[k].itemType)
                            {
                                case ItemTypes.GOLD:
                                    int goldCode = Int32.Parse(currentNode.nodeResponses[i].restrictions[k].restrictionText);
                                    //newButton.GetComponentInChildren<Text>().text += " [" + gameLoop.myStats.perkList[goldCode].name + " gold required]";
                                    //if (playerModule.gold + goldCode < 0)
                                    //{
                                    //    insufficient = true;
                                    //    newButton.GetComponent<Button>().interactable = false;
                                    //}
                                    //newButton.GetComponent<Button>().onClick.AddListener(() => playerModule.ChangeGold(goldCode));
                                    break;
                            }
                        }
                        //Request command - simple check if player meets some requirements
                        else if(currentNode.nodeResponses[i].restrictions[k].command == Commands.REQUEST)
                        {
                            
                        } 
                        //Give Command
                        else if (currentNode.nodeResponses[i].restrictions[k].command == Commands.GIVE)
                        {
                            switch (currentNode.nodeResponses[i].restrictions[k].itemType)
                            {

                                case ItemTypes.QUEST:
                                    int code = Int32.Parse(currentNode.nodeResponses[i].restrictions[k].restrictionText);

                                    dynamic newQuest = ScriptableObject.CreateInstance(ObjectStorage.Instance.ledger.referencedQuests[code]);
                                    newButton.GetComponent<Button>().onClick.AddListener(() => AddQuest(newQuest));
                                    break;
                            }
                        } 
                        //Open command - opens other menus referenced in UIObjects
                        else if (currentNode.nodeResponses[i].restrictions[k].command == Commands.OPEN)
                        {
                            int commandCode = Int32.Parse(currentNode.nodeResponses[i].restrictions[k].restrictionText);
                            newButton.GetComponent<Button>().onClick.AddListener(() => ActivateGameObject(commandCode));

                        }
                    }
                    // adds some flavor text when a restriction isn't met
                    if(insufficient)
                        newButton.GetComponentInChildren<Text>().text += " <Insufficient>";
                }
                if (newButton.GetComponent<Button>().interactable)
                {
                    int localIndex = i;
                    newButton.GetComponent<Button>().onClick.AddListener(() => ResponseSelected(localIndex));
                }
            }
        }
    }

    /// <summary>
    /// Sets gameobject in UIObjects[i] to active
    /// Useful for popups during/after dialog
    /// </summary>
    /// <param name="i"></param>
    public void ActivateGameObject(int i)
    {
        if(UIObjects.Count > i)
        {
            switch (i)
            {
                // Can handle specific activation calls
                case 0:
                    //UIObjects[i].GetComponent<RosterMenuController>().Init(playerModule);
                    break;
                case 1:

                    break;
            }
            UIObjects[i].SetActive(true);
        }
    }

    public void AddQuest(Quest newQuest)
    {
        ObjectStorage.Instance.ledger.AcceptQuest(newQuest);
        ObjectStorage.Instance.ledger.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates dialogboxes when a response is clicked, disables dialogboxes if that was the last dialog
    /// </summary>
    /// <param name="responseIndex"></param>
    public void ResponseSelected(int responseIndex)
    {
        //Check if response has a valid connection
        if (currentNode.nodeResponses[responseIndex].outboundConnection.nodePoint != -1)
        {
            int tmpIndex = currentNode.nodeResponses[responseIndex].outboundConnection.nodePoint;
            currentNode = dialogTree[tmpIndex];
            UpdateToNewDialog();
        } else
        {
            // Disables the gameobject when last response is clicked
            ClearDialog(-1);
        }
        
    }

    /// <summary>
    /// Destroys all the children buttons
    /// </summary>
    /// <param name="k"></param>
    private void ClearDialog(int k = 0)
    {
        promptField.text = "";
        Button[] buttons = responseParent.GetComponentsInChildren<Button>();
        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            if (buttons[i].gameObject.activeSelf)
            {
                Destroy(buttons[i].gameObject);
            }
        }
        if(k == -1)
        {
            gameObject.SetActive(false);
        }
    }
}
