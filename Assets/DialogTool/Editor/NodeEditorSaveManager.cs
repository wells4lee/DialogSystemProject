using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;
using UnityEditor;

public class NodeEditorSaveManager : MonoBehaviour, ISerializationCallbackReceiver
{
    //Creates the SaveableListOfNodes representation of the editorNodes
    private static SaveableListOfNodes CreateObjectFromEditor(List<Node> editorNodes)
    {
        List<SaveableNode> result = new List<SaveableNode>();

        for (int i = 0; i < editorNodes.Count; i++)
        {
            SaveableNode tmpNode = new SaveableNode();
            tmpNode.promptText = editorNodes[i].promptText;
            tmpNode.position = editorNodes[i].rect.position;
            tmpNode.inboundConnections = new List<SaveableConnection>();
            tmpNode.nodeResponses = new List<SaveableResponse>();

            if (editorNodes[i].responses != null)
            {
                for (int j = 0; j < editorNodes[i].responses.Count; j++)
                {
                    NodeResponse response = editorNodes[i].responses[j];
                    SaveableResponse tmpResponse = new SaveableResponse();
                    tmpResponse.responseText = response.text;
                    tmpResponse.restrictions = new List<SaveableRestriction>();
                    tmpResponse.outboundConnection = new SaveableConnection();

                    if (response.restrictions != null)
                    {
                        for (int k = 0; k < response.restrictions.Count; k++)
                        {
                            SaveableRestriction tmpRestriction = new SaveableRestriction();
                            tmpRestriction.command = response.restrictions[k].currentCommand;
                            tmpRestriction.itemType = response.restrictions[k].currentItemType;
                            tmpRestriction.restrictionText = response.restrictions[k].commandString;

                            tmpResponse.restrictions.Add(tmpRestriction);
                        }
                    }
                    tmpNode.nodeResponses.Add(tmpResponse);
                }
            }

            result.Add(tmpNode);
        }

        for (int i = 0; i < editorNodes.Count; i++)
        {
            LinkSaveableNodes(editorNodes[i], result, editorNodes);
        }

        SaveableListOfNodes saveableJSONList = new SaveableListOfNodes();
        saveableJSONList.saveableNodeList = new List<SaveableNode>();

        for (int i = 0; i < result.Count; i++)
        {
            saveableJSONList.saveableNodeList.Add(result[i]);
        }
        return saveableJSONList;
    }

    //Saves the editor to a json file
    public static void SaveEditorToFile(string filename, List<Node> editorNodes)
    {
        SaveableListOfNodes saveableJSONList = CreateObjectFromEditor(editorNodes);
        string tmpText = JsonUtility.ToJson(saveableJSONList);
        string tmpName = "/" + filename + ".json";
        System.IO.File.WriteAllText(Application.persistentDataPath + tmpName, tmpText);
        //Debug.Log(Application.persistentDataPath + tmpName);
    }

    //Saves the SaveableListOfNodes to a scriptable object for use in Unity
    public static void ExportEditorToScriptableObject(string assetName, List<Node> editorNodes)
    {
        SaveableListOfNodes saveableJSONList = CreateObjectFromEditor(editorNodes);
        DialogBank tmpDialogBank = ScriptableObject.CreateInstance<DialogBank>();
        tmpDialogBank.dialogObject = saveableJSONList;
        tmpDialogBank.name = assetName;
        string tmpName = "/" + assetName + ".asset";
        AssetDatabase.CreateAsset(tmpDialogBank, "Assets/Resources" + tmpName);

    }

    //Connects the saveable node objects to represent the editor's connections
    private static void LinkSaveableNodes(Node node, List<SaveableNode> saveableNodes, List<Node> editorNodes)
    {
        int index = editorNodes.IndexOf(node);

        if(node.InConnection != null && node.InConnection.Count > 0)
        {
            for(int i = 0; i < node.InConnection.Count; i++)
            {
                SaveableConnection tmpConnection = new SaveableConnection();
                tmpConnection.nodePoint = index;
                tmpConnection.responseParent = editorNodes.IndexOf(node.InConnection[i].outPoint.response.node);
                tmpConnection.responsePoint = node.InConnection[i].outPoint.response.id - 1;
                saveableNodes[index].inboundConnections.Add(tmpConnection);
            }
        }

        if (node.responses != null && node.responses.Count > 0)
        {
            for(int i = 0; i < node.responses.Count; i++)
            {
                if (node.responses[i].responseConnection != null)
                {
                    SaveableConnection tmpConnection = new SaveableConnection();
                    Node childNode = node.responses[i].responseConnection.inPoint.node;
                    tmpConnection.responseParent = index;
                    tmpConnection.responsePoint = i;
                    tmpConnection.nodePoint = editorNodes.IndexOf(childNode);
                    saveableNodes[index].nodeResponses[i].outboundConnection = tmpConnection;
                } else
                {
                    SaveableConnection tmpConnection = new SaveableConnection();
                    tmpConnection.nodePoint = -1;
                    saveableNodes[index].nodeResponses[i].outboundConnection = tmpConnection;
                }
            }
        }
    }

    public void OnBeforeSerialize()
    {
        throw new NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        throw new NotImplementedException();
    }

}
