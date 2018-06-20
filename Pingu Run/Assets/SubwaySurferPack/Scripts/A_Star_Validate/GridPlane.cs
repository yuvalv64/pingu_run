using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlane : MonoBehaviour {

    public bool onlyDisplayPathGizmos;
    public LayerMask unwalkableMask;
    public LayerMask walkableObsMask;
    public Vector2 gridWorldSize;
    Node[,] grid;
    public float nodeRadius;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    Collider[] colliders;


    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt( gridWorldSize.x/ nodeDiameter);
        gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward  * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                bool walkableRamp = !(Physics.CheckSphere(worldPoint, nodeRadius, walkableObsMask));
                
                colliders = Physics.OverlapSphere(worldPoint, nodeRadius);
                
                grid[x, y] = new Node(colliders[0], walkableRamp,walkable, worldPoint , x , y);
             
            }
        }
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }

    

    public List<Node> GetNeigbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1 ; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
             if(x== 0 && y== 0)
                {
                    continue;
                }

                int CheckX = node.gridX + x;
                int CheckY = node.gridY + y;

                if(CheckX >=0 && CheckX < gridSizeX && CheckY >=0 && CheckY < gridSizeY)
                {
                    neighbours.Add(grid[CheckX, CheckY]);
                }
            }

        }
        return neighbours;

    }

    public List<Node> path;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path!=null)
            {
                foreach (Node n in path)
                {
                    
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {
            if (grid != null)
            {
                //for test
                //Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    
                    if (n.walkable)
                    {
                        if ((n.walkableObs))
                        {
                            
                            Gizmos.color = Color.blue;
                        }
                       
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        
                        Gizmos.color = Color.red;
                    }
                    // for test 
                    //if (playerNode == n)
                    //{
                    //    Gizmos.color = Color.cyan;
                    //}
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));

                }
            }
        }

        
    }

}
