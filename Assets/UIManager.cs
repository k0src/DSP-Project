using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public void ConnectNodes(Vector3 position)
    {
        UINode clickedNode = GetNodeAtPosition(position);
        Debug.Log("connect");
    }

    public void DeleteNode(Vector3 position)
    {
        UINode clickedNode = GetNodeAtPosition(position);
        if (clickedNode != null)
        {
            Destroy(clickedNode.gameObject);
        }
    }

    private UINode GetNodeAtPosition(Vector3 position)
    {
        UINode[] nodes = FindObjectsOfType<UINode>();
        foreach (UINode node in nodes)
        {
            Vector2 nodePosition2D = new Vector2(node.transform.position.x, node.transform.position.y);
            Vector2 targetPosition2D = new Vector2(position.x, position.y);
            if (Vector2.Distance(nodePosition2D, targetPosition2D) < 1.5f)
            {
                return node;
            }
        }

        return null;
    }
}
