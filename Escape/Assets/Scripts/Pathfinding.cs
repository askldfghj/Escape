using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;

    Grid grid;


    Heap<Node> openSet;
    HashSet<Node> closedSet;

    List<Node> path;

    List<Node> neighbor;

    List<Vector3> simplyfyWaypoints;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
        openSet = new Heap<Node>(grid.MaxSize);
        closedSet = new HashSet<Node>();

        path = new List<Node>();
        neighbor = new List<Node>();
        simplyfyWaypoints = new List<Vector3>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            //Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            openSet.Clear();
            closedSet.Clear();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet.RemoveFirst();

                closedSet.Add(node);

                if (node == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                neighbor = grid.GetNeighbours(node);
                for (int i = 0; i < neighbor.Count; i++)
                {
                    if (!neighbor[i].walkable || closedSet.Contains(neighbor[i]))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbor[i]) + neighbor[i].movementPenalty;
                    if (newCostToNeighbour < neighbor[i].gCost || !openSet.Contains(neighbor[i]))
                    {
                        neighbor[i].gCost = newCostToNeighbour;
                        neighbor[i].hCost = GetDistance(neighbor[i], targetNode);
                        neighbor[i].parent = node;

                        if (!openSet.Contains(neighbor[i]))
                            openSet.Add(neighbor[i]);
                        else
                            openSet.UpdateItem(neighbor[i]);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        path.Clear();
        Node currentNode = endNode;

        while (currentNode != startNode)
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
        simplyfyWaypoints.Clear();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                simplyfyWaypoints.Add(path[i].worldPosition);
            } 
            directionOld = directionNew;
        }
        return simplyfyWaypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
