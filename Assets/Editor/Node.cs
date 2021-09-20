using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Node
{
    private GUIStyle responseStyle;
    private GUIStyle selectedResponseStyle;
    private GUIStyle buttonStyle;

    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;
    public ConnectionPoint inPoint;
    public List<Connection> InConnection = new List<Connection>();
    public GUIStyle inPointStyle;
    public GUIStyle outPointStyle;
    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public Action<Node> OnRemoveNode;
    public Action<ConnectionPoint> OnClickOutPoint;
    public List<NodeResponse> responses = new List<NodeResponse>();
    public string promptText = "DEFAULT TEXT";

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle,
        GUIStyle responseStyle, GUIStyle selectedResponseStyle, GUIStyle inPointStyle, GUIStyle outPointStyle,
        GUIStyle buttonStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, inPointStyle, OnClickInPoint);
        this.OnClickOutPoint = OnClickOutPoint;
        defaultNodeStyle = nodeStyle;
        this.inPointStyle = inPointStyle;
        this.outPointStyle = outPointStyle;
        selectedNodeStyle = selectedStyle;
        this.responseStyle = responseStyle;
        this.selectedResponseStyle = selectedResponseStyle;
        this.buttonStyle = buttonStyle;
        OnRemoveNode = OnClickRemoveNode;
    }

    /// <summary>
    /// Handles dragging nodes around on graph space
    /// </summary>
    /// <param name="delta"></param>
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    /// <summary>
    /// Resizes nodes with mouse scroll
    /// </summary>
    /// <param name="zoom"></param>
    public void Zoom(float zoom)
    {
        rect.width += zoom;
        rect.height += zoom;
    }

    /// <summary>
    /// Draws Node and add response button
    /// </summary>
    public void Draw()
    {
        inPoint.Draw();
        GUI.Box(rect, title, style);
        GUI.Label(new Rect(rect.x + rect.width/2 - style.CalcSize(new GUIContent("Prompt Text")).x/2, rect.y + 5, rect.width, rect.height), "Prompt Text");
        promptText = GUI.TextArea(new Rect(rect.x + 10, rect.y + 20, rect.width - 20, rect.height - 50), promptText);
        
        if(GUI.Button(new Rect(rect.x + 10, rect.y + rect.height - 30, 20, 20), "+"))
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Response"), false, AddResponse);
            genericMenu.ShowAsContext();
        }

        DrawResponses();
        DrawConnections();
    }

    /// <summary>
    /// Handles input on nodes
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    /// <summary>
    /// Handles X button context menu for deleting nodes
    /// </summary>
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Calls OnRemoveNode delegate
    /// </summary>
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    /// <summary>
    /// Adds a response to this node
    /// </summary>
    public void AddResponse()
    {
        Node node = this;
        if (responses == null)
        {
            responses = new List<NodeResponse>();
        }

        responses.Add(new NodeResponse(node, responseStyle, selectedResponseStyle, buttonStyle, responses.Count + 1, OnClickRemoveResponse, OnClickOutPoint));
    }

    /// <summary>
    /// Draws lines between in/out of responses/nodes
    /// </summary>
    private void DrawConnections()
    {
        if(InConnection != null)
            for(int i = 0; i < InConnection.Count; i++)
            {
                InConnection[i].Draw();
            }
    }

    /// <summary>
    /// Handles the responses draws
    /// </summary>
    private void DrawResponses()
    {
        if (responses != null)
        {
            for (int i = 0; i < responses.Count; i++)
            {
                responses[i].Draw();
            }
        }
    }

    /// <summary>
    /// Handles removing a node's response
    /// </summary>
    /// <param name="response"></param>
    private void OnClickRemoveResponse(NodeResponse response)
    {
        if(response.responseConnection != null)
        {
            response.responseConnection.inPoint.node.InConnection.Remove(response.responseConnection);
            response.responseConnection = null;
        }
        
        responses.Remove(response);
        for(int i = 0; i < responses.Count; i++)
        {
            responses[i].id = i + 1;
        }
        GUI.changed = true;
    }
}