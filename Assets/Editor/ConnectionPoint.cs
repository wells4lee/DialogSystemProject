using System;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    public Node node;
    public NodeResponse response;
    public GUIStyle style;

    public Action<ConnectionPoint> OnClickConnectionPoint;

    public ConnectionPoint(Node node, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.response = null;
        this.type = ConnectionPointType.In;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 30f, 35f);
    }

    public ConnectionPoint(NodeResponse response, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = null;
        this.response = response;
        this.type = ConnectionPointType.Out;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 30f, 35f);
    }

    public void Draw()
    {

        switch (type)
        {
            case ConnectionPointType.In:
                rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
                rect.x = node.rect.x - rect.width ;
                break;

            case ConnectionPointType.Out:
                rect.y = response.rect.y + (response.rect.height * 0.5f) - rect.height * 0.5f;
                rect.x = response.rect.x + response.rect.width;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}