using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Commands { MODIFY, REQUEST, GIVE, GREATER, LESS, OPEN };

public enum ItemTypes { NONE, GOLD, QUEST };

[Serializable]
public class SaveableNode
{
    public List<SaveableConnection> inboundConnections;
    public string promptText;
    public List<SaveableResponse> nodeResponses;
    public Vector2 position;
}

[Serializable]
public class SaveableResponse
{
    public string responseText;
    public List<SaveableRestriction> restrictions;
    public SaveableConnection outboundConnection;
}

[Serializable]
public class SaveableRestriction
{
    public Commands command;
    public ItemTypes itemType;
    public string restrictionText;
}

[Serializable]
public class SaveableConnection
{
    public int nodePoint;
    public int responseParent;
    public int responsePoint;
}

[Serializable]
public class SaveableListOfNodes
{
    public List<SaveableNode> saveableNodeList;
}
