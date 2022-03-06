using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResponseRestriction
{
    private GUIStyle buttonStyle;

    public int id;
    public NodeResponse node;
    public Action<ResponseRestriction> OnDeleteRestriction;
    public Commands currentCommand = Commands.REQUEST;
    public ItemTypes currentItemType = ItemTypes.NONE;
    public string commandString = "";
    
    public ResponseRestriction(NodeResponse node, int id, GUIStyle buttonStyle, Action<ResponseRestriction> OnDeleteRestriction)
    {
        this.node = node;
        this.id = id;
        this.buttonStyle = buttonStyle;
        this.OnDeleteRestriction = OnDeleteRestriction;
    }

    public void Draw()
    {
        if (GUI.Button(new Rect(node.rect.x + 10, node.rect.y + 50 + (30 * id), node.rect.width/4 - 10, 25), currentCommand.ToString(), buttonStyle))
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("MODIFY"), false, () => SetCommandTo(Commands.MODIFY));
            genericMenu.AddItem(new GUIContent("REQUEST"), false, () => SetCommandTo(Commands.REQUEST));
            genericMenu.AddItem(new GUIContent("GIVE"), false, () => SetCommandTo(Commands.GIVE));
            genericMenu.AddItem(new GUIContent("GREATER"), false, () => SetCommandTo(Commands.GREATER));
            genericMenu.AddItem(new GUIContent("LESS"), false, () => SetCommandTo(Commands.LESS));
            genericMenu.AddItem(new GUIContent("OPEN"), false, () => SetCommandTo(Commands.OPEN));
            genericMenu.ShowAsContext();
        }

        if(GUI.Button(new Rect(node.rect.x + node.rect.width / 4, node.rect.y + 50 + (30 * id), node.rect.width / 4 - 10, 25), currentItemType.ToString(), buttonStyle))
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("NONE"), false, () => SetItemTypeTo(ItemTypes.NONE));
            genericMenu.AddItem(new GUIContent("GOLD"), false, () => SetItemTypeTo(ItemTypes.GOLD));
            genericMenu.AddItem(new GUIContent("QUEST"), false, () => SetItemTypeTo(ItemTypes.QUEST));
            //genericMenu.AddItem(new GUIContent("FOODSUP"), false, () => SetItemTypeTo(ItemTypes.FOODSUP));
            //genericMenu.AddItem(new GUIContent("FOODCAP"), false, () => SetItemTypeTo(ItemTypes.FOODCAP));
            //genericMenu.AddItem(new GUIContent("IRONSUP"), false, () => SetItemTypeTo(ItemTypes.IRONSUP));
            //genericMenu.AddItem(new GUIContent("IRONCAP"), false, () => SetItemTypeTo(ItemTypes.IRONCAP));
            //genericMenu.AddItem(new GUIContent("DEFENSE"), false, () => SetItemTypeTo(ItemTypes.DEFENSE));
            //genericMenu.AddItem(new GUIContent("RESENTMENT"), false, () => SetItemTypeTo(ItemTypes.RESENTMENT));
            genericMenu.ShowAsContext();
        }

        commandString = GUI.TextField(new Rect(node.rect.x + node.rect.width/2 + 10, node.rect.y + 50 + (30 * id), node.rect.width / 3, 25), commandString);

        if (GUI.Button(new Rect(node.rect.x + node.rect.width - 30, node.rect.y + 50 + (30 * id), 20, 25), "X")){
            OnDeleteRestriction(this);
        }
    }

    public void SetCommandTo(Commands command)
    {
        currentCommand = command;
    }

    public void SetItemTypeTo(ItemTypes type)
    {
        currentItemType = type;
    }

    
}
