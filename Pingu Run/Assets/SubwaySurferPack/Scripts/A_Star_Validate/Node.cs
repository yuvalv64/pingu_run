using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : IHeapItem<Node> {

    public bool walkable;
    public bool walkableObs;
    public Vector3 worldPosition;
    public Collider posCollider;
    public int gCost;
    public int hCost;
    public Node parent;

    public int gridX;
    public int gridY;

    int heapIndex;


    public Node(Collider _collider, bool _walkableRamsp, bool _walkable , Vector3 _worldPoss , int _gridX , int _gridY)
    {
        posCollider = _collider;
        walkableObs = _walkableRamsp;
        walkable = _walkable;
        worldPosition = _worldPoss;
        gridX = _gridX;
        gridY = _gridY;
    }


    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
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


    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
