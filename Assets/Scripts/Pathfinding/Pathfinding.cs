using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;
    public NodeGrid grid;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    /// <summary>
    /// A* implementation of pathfinding. 
    /// </summary>
    /// <param name="startNode">Starting point for calculation</param>
    /// <param name="targetNode">Endpoint for calculation</param>
    public IEnumerator FindPath(Vector3 start, Vector3 target)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(start);
        Node targetNode = grid.NodeFromWorldPoint(target);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> OpenSet = new Heap<Node>(grid.gridMaxSize);
            HashSet<Node> ClosedSet = new HashSet<Node>();

            OpenSet.Add(startNode);

            while (OpenSet.Count > 0)
            {
                Node currentNode = OpenSet.RemoveFirst();
                ClosedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
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
        yield return null;
        if(pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(wayPoints, pathSuccess);
    }

    public void StartFindPath(Vector3 start, Vector3 end)
    {
        StartCoroutine(FindPath(start, end));
    }

    /// <summary>
    /// Sorts the path so that it is in the correct order 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    Vector3[] RetracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while(currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
        }
        return waypoints.ToArray();
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