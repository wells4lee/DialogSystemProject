using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class NodeBasedEditor : EditorWindow
{
    private List<Node> nodes;
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle responseStyle;
    private GUIStyle selectedResponseStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    private GUIStyle buttonStyle;
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;
    private Vector2 offset;
    private Vector2 drag;
    private float zoomLevel = 0;
    private string saveString = "";
    private bool firstLoad = true;
    private bool saveMenuOpen = false;
    private bool exportMenuOpen = false;

    /// <summary>
    /// Allows new Node Graph Windows to be created under Window-> Node Based Editor
    /// </summary>
    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Map Editor");
    }

    /// <summary>
    /// Loads style and format for Nodes
    /// </summary>
    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        responseStyle = new GUIStyle();
        responseStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node1.png") as Texture2D;
        responseStyle.border = new RectOffset(12, 12, 12, 12);
        selectedResponseStyle = new GUIStyle();
        selectedResponseStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node1 on.png") as Texture2D;
        selectedResponseStyle.border = new RectOffset(12, 12, 12, 12);
        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);
        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    /// <summary>
    /// Handles side buttons for Save, Load, Import, and Export
    /// </summary>
    private void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 30, 20), "SAVE"))
        {
            saveMenuOpen = true;
            exportMenuOpen = false;
        }

        if (GUI.Button(new Rect(10, 45, 30, 20), "LOAD"))
        {
            saveMenuOpen = false;
            exportMenuOpen = false;

            string path = EditorUtility.OpenFilePanel("Select Dialog", Application.persistentDataPath, "json");
            if (path.Length != 0)
            {
                LoadEditorFromFile(path);
            }
        }

        if (GUI.Button(new Rect(10, 80, 30, 20), "Import"))
        {
            exportMenuOpen = false;
            saveMenuOpen = false;

            string path = EditorUtility.OpenFilePanel("Select Textfile", Application.persistentDataPath, "txt");
            if (path.Length != 0)
            {
                ImportNodesFromText(path);
            }
        }

        if (GUI.Button(new Rect(10, 115, 30, 20), "Export"))
        {
            exportMenuOpen = true;
            saveMenuOpen = false;
        }


        if (saveMenuOpen)
        {
            Rect saveRect = new Rect(focusedWindow.position.width / 4, focusedWindow.position.height / 4, 320, 150);
            GUI.Box(saveRect, "");

            saveString = GUI.TextField(new Rect(saveRect.x + 10, saveRect.y + 50, saveRect.width - 100, 25), saveString);

            if(GUI.Button(new Rect(saveRect.x + 50, saveRect.y + 80, 30, 20), "SAVE"))
            {
                NodeEditorSaveManager.SaveEditorToFile(saveString, nodes);
            }

            if (GUI.Button(new Rect(saveRect.x + saveRect.width - 20, saveRect.y, 20, 20), "X"))
            {
                saveMenuOpen = false;
            }

        }
        
        
        if (exportMenuOpen)
        {
            Rect saveRect = new Rect(focusedWindow.position.width / 4, focusedWindow.position.height / 4, 320, 150);
            GUI.Box(saveRect, "");
            saveString = GUI.TextField(new Rect(saveRect.x + 10, saveRect.y + 50, saveRect.width - 100, 25), saveString);

            if (GUI.Button(new Rect(saveRect.x + 50, saveRect.y + 80, 30, 20), "Export"))
            {
                NodeEditorSaveManager.ExportEditorToScriptableObject(saveString, nodes);
            }

            if (GUI.Button(new Rect(saveRect.x + saveRect.width - 20, saveRect.y, 20, 20), "X"))
            {
                exportMenuOpen = false;
            }

        }

        // Handles first load case where zoom is normalized
        if (firstLoad)
        {
            firstLoad = false;
            buttonStyle = GUI.skin.button;
            buttonStyle.fontSize = 8;
        }
        ProcessNodeEvents(Event.current);
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawNodes();
        DrawConnectionLine(Event.current);
        ProcessEvents(Event.current);
        if (GUI.changed) Repaint();
    }

    // Draws asthetic grid in background
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        gridSpacing += zoomLevel;
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    /// <summary>
    /// Draws all nodes in graph
    /// </summary>
    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    /// <summary>
    /// Handles user input such as clicking, dragging, zooming
    /// </summary>
    /// <param name="e"></param>
    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;

            case EventType.KeyUp:
                if(e.keyCode == KeyCode.Escape)
                {
                    DebugPropertyValues();
                }
                break;
            case EventType.ScrollWheel:
                if(e.delta.y < 0 && zoomLevel <= -123)
                {
                    break;
                } else if(e.delta.y > 0 && zoomLevel >= 1152)
                {
                    break;
                }

                zoomLevel += e.delta.y;

                if (nodes != null)
                {
                    for(int i = 0; i < nodes.Count; i++)
                    {
                        Vector2 dist = new Vector2();
                        if (i > 0)
                        {
                            Vector2 prevNode = new Vector2(nodes[i - 1].rect.position.x + nodes[i - 1].rect.width, nodes[i - 1].rect.position.y + nodes[i - 1].rect.height);
                            dist = prevNode - nodes[i].rect.position;
                        }
                        nodes[i].Zoom(e.delta.y);
                        nodes[i].Drag(new Vector2(e.delta.y, e.delta.y));

                        if(i > 0)
                        {
                            Vector2 prevNode = new Vector2(nodes[i - 1].rect.position.x + nodes[i - 1].rect.width, nodes[i - 1].rect.position.y + nodes[i - 1].rect.height);
                            Vector2 newDist = prevNode - nodes[i].rect.position;
                            if(newDist.x < dist.x)
                            {
                                nodes[i].Drag(new Vector2(dist.x - newDist.x, 0));
                            }
                            else
                            {
                                nodes[i].Drag(new Vector2(dist.x - newDist.x, 0));
                            }
                            if (newDist.y < dist.y)
                            {
                                nodes[i].Drag(new Vector2(0 , dist.y - newDist.y));
                            }
                            else
                            {
                                nodes[i].Drag(new Vector2(0, dist.y - newDist.y));
                            }
                        }
                    }
                }

                GUI.changed = true;
                break;
        }
    }

    /// <summary>
    /// Used for checking if connections are fully formed
    /// </summary>
    private void DebugPropertyValues()
    {
        Debug.Log("\n\n\n");
        Debug.Log("Only printing non-null connection objects\n");
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Node tmpNode = nodes[i];
                if(tmpNode.InConnection == null)
                {
                    Debug.Log("LIST IS NULL!!");
                }
                if (tmpNode.InConnection != null && tmpNode.InConnection.Count > 0)
                {
                    Debug.Log("In Connection is non-null on ... " + tmpNode.promptText);

                    for(int k = 0; k < tmpNode.InConnection.Count; k++)
                    {
                        Debug.Log("Connection " + k + " links to " + tmpNode.InConnection[k].outPoint.response.text);
                    }
                }
                if (tmpNode.responses != null)
                {
                    for (int j = 0; j < tmpNode.responses.Count; j++)
                    {
                        NodeResponse tmpResponse = tmpNode.responses[j];
                        if (tmpResponse.responseConnection != null)
                        {
                            Debug.Log("Out connection is non-null on ..." + tmpResponse.text);
                            Debug.Log(tmpResponse.responseConnection.inPoint.node.promptText);
                        }
                    }
                }
            }
        }
        Debug.Log("\nDebug has concluded!\n");
    }

    /// <summary>
    /// Sends node delegates their events
    /// </summary>
    /// <param name="e"></param>
    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }

                ProcessResponseNodeEvents(e, nodes[i]);
            }
        }
    }

    /// <summary>
    /// Sends node response delegates their events
    /// </summary>
    /// <param name="e"></param>
    /// <param name="parent"></param>
    private void ProcessResponseNodeEvents(Event e, Node parent)
    {
        for(int i = parent.responses.Count - 1; i >= 0; i--)
        {
            bool guiChanged = parent.responses[i].ProcessEvents(e);

            if (guiChanged)
            {
                GUI.changed = true;
            }
        }
    }
    /// <summary>
    /// Draws lines between in/out of node/response
    /// </summary>
    /// <param name="e"></param>
    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    /// <summary>
    /// Handles menu to add node when right clicking on graph
    /// </summary>
    /// <param name="mousePosition"></param>
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Handles dragging the editor by moving nodes in window
    /// </summary>
    /// <param name="delta"></param>
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Adds a node when context menu item is clickec
    /// </summary>
    /// <param name="mousePosition"></param>
    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(mousePosition, 200 + zoomLevel, 200 + zoomLevel, nodeStyle, selectedNodeStyle, responseStyle, selectedResponseStyle, inPointStyle, outPointStyle, buttonStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    /// <summary>
    /// Adds a node at 0,0 position
    /// Used when importing node data
    /// </summary>
    /// <returns></returns>
    private Node ImportNode()
    {
        Vector2 pos = new Vector2(0, 0);
        if (nodes == null)
        {
            nodes = new List<Node>();
        }
        Node newNode = new Node(pos, 200 + zoomLevel, 200 + zoomLevel, nodeStyle, selectedNodeStyle, responseStyle, selectedResponseStyle, inPointStyle, outPointStyle, buttonStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        nodes.Add(newNode);
        return newNode;
    }
    /// <summary>
    /// When the Node in point box is clicked, begin drawing a line to show part of the connection
    /// </summary>
    /// <param name="inPoint"></param>
    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
    /// <summary>
    /// Form the connection if valid
    /// </summary>
    private void CreateConnection()
    {
        if(selectedOutPoint.response.responseConnection != null)
        {
            OnClickRemoveConnection(selectedOutPoint.response.responseConnection);
        }
        selectedInPoint.node.InConnection.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        selectedOutPoint.response.responseConnection = selectedInPoint.node.InConnection[selectedInPoint.node.InConnection.Count - 1];
    }

    /// <summary>
    /// Handles removing connections between node/response
    /// </summary>
    /// <param name="connection"></param>
    public void OnClickRemoveConnection(Connection connection)
    {
        connection.outPoint.response.responseConnection = null;
        connection.inPoint.node.InConnection.Remove(connection);
    }

    /// <summary>
    /// Handles input on response out box to begin forming a connection
    /// </summary>
    /// <param name="outPoint"></param>
    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
    /// <summary>
    /// Handles removing a node from the graph
    /// </summary>
    /// <param name="node"></param>
    private void OnClickRemoveNode(Node node)
    {
        //Removes incoming connections to this node
        if(node.InConnection != null)
        {
            for (int i = node.InConnection.Count - 1; i >= 0; i--)
            {
                OnClickRemoveConnection(node.InConnection[i]);
                
            }

        }
        
        //Removes outgoing connections from this node's responses
        foreach (NodeResponse response in node.responses)
        {
            if (response.responseConnection != null)
            {
                OnClickRemoveConnection(response.responseConnection);
            }
        }

        nodes.Remove(node);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void ClearEditor()
    {
        nodes = new List<Node>();
        offset = new Vector2();
        GUI.changed = true;
    }
    /// <summary>
    /// Loads a JSON file using a pathName
    /// Sends to helper method to fill editor
    /// </summary>
    /// <param name="pathName"></param>
    private void LoadEditorFromFile(string pathName)
    {
        if (System.IO.File.Exists(pathName))
        {
            string fileText = System.IO.File.ReadAllText(pathName);
            SaveableListOfNodes jsonCollection = JsonUtility.FromJson<SaveableListOfNodes>(fileText);
            RebuildEditorFromJSON(jsonCollection);
        }
    }
    /// <summary>
    /// Helper method to fill editor with nodes from JSON data
    /// </summary>
    /// <param name="jsonCollection"></param>
    private void RebuildEditorFromJSON(SaveableListOfNodes jsonCollection)
    {
        // Clear Editor
        ClearEditor();

        // Create nodes and give them their content
        for (int i = 0; i < jsonCollection.saveableNodeList.Count; i++)
        {
            OnClickAddNode(jsonCollection.saveableNodeList[i].position);
            nodes[i].promptText = jsonCollection.saveableNodeList[i].promptText;

            for(int j = 0; j < jsonCollection.saveableNodeList[i].nodeResponses.Count; j++)
            {
                nodes[i].AddResponse();
                nodes[i].responses[j].text = jsonCollection.saveableNodeList[i].nodeResponses[j].responseText;
                
                if (jsonCollection.saveableNodeList[i].nodeResponses[j].restrictions != null)
                {
                    for (int k = 0; k < jsonCollection.saveableNodeList[i].nodeResponses[j].restrictions.Count; k++)
                    {
                        nodes[i].responses[j].AddNewResponseRestriction();
                        nodes[i].responses[j].restrictions[k].currentCommand = jsonCollection.saveableNodeList[i].nodeResponses[j].restrictions[k].command;
                        nodes[i].responses[j].restrictions[k].currentItemType = jsonCollection.saveableNodeList[i].nodeResponses[j].restrictions[k].itemType;
                        nodes[i].responses[j].restrictions[k].commandString = jsonCollection.saveableNodeList[i].nodeResponses[j].restrictions[k].restrictionText;
                    }
                }
                
            }
        }
        // link nodes with connections
        for(int i = 0; i < jsonCollection.saveableNodeList.Count; i++)
        {
            if (jsonCollection.saveableNodeList[i].inboundConnections != null)
            {
                for (int m = 0; m < jsonCollection.saveableNodeList[i].inboundConnections.Count; m++)
                {
                    selectedInPoint = nodes[i].inPoint;
                    selectedOutPoint = nodes[jsonCollection.saveableNodeList[i].inboundConnections[m].responseParent].responses[jsonCollection.saveableNodeList[i].inboundConnections[m].responsePoint].outPoint;
                    CreateConnection();
                }
            }
        }
    }
    /// <summary>
    /// Loads text and creates nodes to represent their prompts and responses
    /// uses '-' symbol for responses
    /// </summary>
    /// <param name="pathName"></param>
    private void ImportNodesFromText(string pathName)
    {
        ClearEditor();

        if (System.IO.File.Exists(pathName))
        {
            string[] fileText = System.IO.File.ReadAllLines(pathName);
            Node parentNode = null;
            for(int i = 0; i < fileText.Length; i++)
            {
                if(fileText[i] != "" && fileText[i].Length > 1)
                {
                    if (fileText[i].StartsWith("-") && parentNode != null)
                    {

                        string tmpText = fileText[i].Substring(1, fileText[i].Length - 1);
                        parentNode.AddResponse();
                        parentNode.responses[parentNode.responses.Count - 1].text = tmpText;
                    }
                    else
                    {
                        parentNode = ImportNode();
                        parentNode.promptText = fileText[i];
                    }

                }
                
            }
            
        }
    }

}
