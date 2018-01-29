using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public NodeGrid grid;
    List<Node> foundPath;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    /// <summary>
    /// A* implementation of pathfinding. 
    /// </summary>
    /// <param name="startNode">Starting point for calculation</param>
    /// <param name="targetNode">Endpoint for calculation</param>
    /// <param name="callback">Function to be called by the requesting unit</param>
    public void FindPath(Node startNode, Node targetNode, Action<List<Node>, bool> callback)
    {
        //Debug.Log(string.Format("Starting to find path at: start {0},{1}, target {2},{3}", 
        //    startNode.gridX, startNode.gridY, targetNode.gridX, targetNode.gridY));

        Heap<Node> OpenSet = new Heap<Node>(grid.gridMaxSize);
        HashSet<Node> ClosedSet = new HashSet<Node>();

        OpenSet.Add(startNode);

        while (OpenSet.Count > 0)
        {
            Node currentNode = OpenSet.RemoveFirst();
            ClosedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                callback(foundPath, true);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                //if ((neighbor.occupier && neighbor.occupier.playerID != ID) || ClosedSet.Contains(neighbor)) //skipping nodes occupied by enemies or already checked
                 //   continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;
                if (newMovementCostToNeighbor < neighbor.gCost || !OpenSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if (!OpenSet.Contains(neighbor))
                        OpenSet.Add(neighbor);
                    else OpenSet.UpdateItem(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Sorts the path so that it is in the correct order 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    void RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while(currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        foundPath = path;
    }

    int GetDistance(Node start, Node finish)
    {
        int distX = Mathf.Abs(start.gridX - finish.gridX);
        int distY = Mathf.Abs(start.gridY - finish.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}