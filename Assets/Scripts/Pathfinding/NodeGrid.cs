using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public Vector2 gridSize;
    public bool showGridGizmos;

    public Node[,] nodeGrid;
    public Vector3 startNode;

    public int gridMaxSize
    {
        get { return (int)(gridSize.x * gridSize.y); }
    }

    private void Awake()
    {
        GenerateGrid();
    }
    
    public void GenerateGrid()
    {
        nodeGrid = new Node[(int)gridSize.x, (int)gridSize.y];

        /*string holderName = "Node Grid";
        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform gridholder = new GameObject(holderName).transform;
        gridholder.parent = transform;*/

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 nodePosition = new Vector3(-gridSize.x/2 + transform.position.x + x * 4, transform.position.y, -gridSize.y/2 + transform.position.z + y * 4);
                int movementPenalty = 0;
                nodeGrid[x, y] = new Node(nodePosition, x, y, movementPenalty);
                Debug.LogWarning("New node created at: " + nodeGrid[x,y].worldPosition);
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y) //Check if the node exist within the grid
                    neighbors.Add(nodeGrid[checkX, checkY]);
            }
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (showGridGizmos)
        {
            if (nodeGrid != null)
            {
                foreach (Node n in nodeGrid)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (1.5f));
                    //Gizmos.color = (n.tileNode.walkable) ? Color.white : Color.red;
                }
            }
        }
    }
}