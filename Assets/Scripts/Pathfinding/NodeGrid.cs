using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public Vector2 gridBoundSize;
    public bool showGridGizmos;

    public Node[,] nodeTraversalGrid;
    public LayerMask unwalkable;
    public float nodeRadius;
    public Vector3 bottomLeft;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public int gridMaxSize
    {
        get { return (int)(gridBoundSize.x * gridBoundSize.y); }
    }

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridBoundSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridBoundSize.y / nodeDiameter);
        GenerateGrid();
    }
    
    public void GenerateGrid()
    {
        nodeTraversalGrid = new Node[(int)gridBoundSize.x, (int)gridBoundSize.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridBoundSize.x / 2 - Vector3.forward * gridBoundSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                int movementPenalty = 0;
                bool walkable = !(Physics.CheckSphere(nodePosition, nodeRadius, unwalkable));
                nodeTraversalGrid[x, y] = new Node(nodePosition, x, y, movementPenalty, walkable);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        Vector3 localPosition = worldPos - (transform.position - Vector3.left * gridBoundSize.x / 2 - Vector3.forward * gridBoundSize.y / 2);
        float percentX = (localPosition.x + gridBoundSize.x / 2) / gridBoundSize.x;
        float percentY = (localPosition.z + gridBoundSize.y / 2) / gridBoundSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return nodeTraversalGrid[x, y];
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

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) //Check if the node exist within the grid
                    neighbors.Add(nodeTraversalGrid[checkX, checkY]);
            }
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridBoundSize.x, 1, gridBoundSize.y));
        if (showGridGizmos)
        {
            if (nodeTraversalGrid != null)
            {
                foreach (Node n in nodeTraversalGrid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-0.1f));
                }
            }
        }
    }
}