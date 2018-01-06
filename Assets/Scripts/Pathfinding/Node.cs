using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public Vector3 worldPosition;
    public int gCost, hCost,
        gridX, gridY,
        heapIndex, movementPenalty;
    public bool walkable;
    public Node parent;

    public int fCost
    {
        get{ return hCost + gCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public Node(Vector3 _worldPos, int _gridX, int _gridY, int _penalty, bool _walkable)
    {
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
        walkable = _walkable;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
