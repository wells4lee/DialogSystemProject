using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeResponse
{
    private GUIStyle responseStyle;
    private GUIStyle selectedResponseStyle;
    private GUIStyle style;
    private GUIStyle buttonStyle;
    private bool isSelected;

    public Rect rect;
    public Node node;
    public string text = "DEFAULT TEXT";
    public int id;
    public Action<NodeResponse> OnClickRemoveResponse;
    public ConnectionPoint outPoint;
    public Connection responseConnection;
    public Action<ConnectionPoint> OnClickOutPoint;
    public List<ResponseRestriction> restrictions;
    public int adjustment = 0;

    public NodeResponse(Node node, GUIStyle responseStyle, GUIStyle selectedResponseStyle, GUIStyle buttonStyle, int id, Action<NodeResponse> OnClickRemoveResponse, Action<ConnectionPoint> OnClickOutPoint)
    {
        this.node = node;
        this.responseStyle = responseStyle;
        style = responseStyle;
        this.buttonStyle = buttonStyle;
        this.selectedResponseStyle = selectedResponseStyle;
        this.id = id;
        outPoint = new ConnectionPoint(this, node.outPointStyle, OnClickOutPoint);
        this.OnClickRemoveResponse = OnClickRemoveResponse;
        this.OnClickOutPoint = OnClickOutPoint;
    }

    public void Draw()
    {
        outPoint.Draw();
        if (restrictions != null && restrictions.Count > 0)
        {
            adjustment = restrictions.Count * 30;

            if (node.responses.Count > 1 && id > 1)
            {
                int fullAdjust = 0;
                for (int i = 0; i < id - 1; i++)
                {
                    fullAdjust += node.responses[i].adjustment;
                }
                rect = new Rect(node.rect.x, node.rect.y + node.rect.height + fullAdjust + ((node.rect.height + 5) * (id - 1)), node.rect.width, node.rect.height + adjustment);
            }
            else
            {
                rect = new Rect(node.rect.x, node.rect.y + node.rect.height + ((node.rect.height + 5) * (id - 1)), node.rect.width, node.rect.height + adjustment);
            }
        }
        else
        {
            if (node.responses.Count > 1 && id > 1)
            {
                int fullAdjust = 0;
                for(int i = 0; i < id - 1; i++)
                {
                    fullAdjust += node.responses[i].adjustment;
                }
                rect = new Rect(node.rect.x, node.rect.y + node.rect.height + fullAdjust + ((node.rect.height + 5) * (id - 1)), node.rect.width, node.rect.height + adjustment);
            }
            else
            {
                rect = new Rect(node.rect.x, node.rect.y + node.rect.height + ((node.rect.height + 5) * (id - 1)), node.rect.width, node.rect.height + adjustment);
            }
        }

        GUI.Box(rect, "", style);
        GUI.Label(new Rect(rect.x + rect.width / 4 + 15, rect.y + 5, 100, 20), "Response");
        if(GUI.Button(new Rect(rect.x + rect.width/4 + 25, rect.y + 20, 30, 20), "+="))
        {
            AddNewResponseRestriction();
        }
        
        text = GUI.TextArea(new Rect(rect.x + 10, rect.y + 50 + adjustment, rect.width - 20, rect.height - 60 - adjustment), text);
        if (restrictions != null)
        {
            for (int i = 0; i < restrictions.Count; i++)
            {
                restrictions[i].Draw();
            }
        }
    }


    public void AddNewResponseRestriction()
    {
        if(restrictions == null || restrictions.Count <= 0)
        {
            restrictions = new List<ResponseRestriction>();
            restrictions.Add(new ResponseRestriction(this, 0, buttonStyle, OnRemoveRestriction));
        } else
        {
            restrictions.Add(new ResponseRestriction(this, restrictions[restrictions.Count-1].id + 1, buttonStyle, OnRemoveRestriction));
        }
        
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isSelected = true;
                        style = selectedResponseStyle;
                        GUI.changed = true;
                    }
                    else
                    {
                        
                        isSelected = false;
                        style = responseStyle;
                        GUI.changed = true;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove Response"), false, Remove);
        genericMenu.ShowAsContext();
    }

    private void Remove()
    {
        OnClickRemoveResponse(this);
    }

    private void OnRemoveRestriction(ResponseRestriction restriction)
    {
        int tmpID = restriction.id;
        restrictions.RemoveAt(tmpID);
        for (int i = restrictions.Count - 1; i >= tmpID; i-- )
        {
            restrictions[i].id--;
        }
    }

}
