using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    Node[,] grid;
    int gridSizeX, gridSizeY;
    Grid gridComponent;
    int directionChangePenalty = 5;


    public void CreateGrid(int sizeX, int sizeY) 
    {
        gridSizeX = sizeX;
        gridSizeY = sizeY;
        grid = new Node[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                
                grid[x, y] = new Node(true, new Vector2(x, y), x, y);
            }
        }
    }
    public void InitializeGridFromMap()
    {
        GameObject map = GameObject.FindGameObjectWithTag("Map");
        if (map != null)
        {
            gridComponent = map.GetComponent<Grid>();
            if (gridComponent != null)
            {
                
                Tilemap tilemap = GameObject.FindGameObjectWithTag("Map").transform.Find("Background").GetComponent<Tilemap>();
    
                BoundsInt bounds = tilemap.cellBounds;
                gridSizeX = bounds.size.x;
                gridSizeY = bounds.size.y;
                Debug.Log($"Grid size: {gridSizeX}x{gridSizeY}");

                

                grid = new Node[gridSizeX, gridSizeY];

                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        Vector2 worldPoint = new Vector2(x, y); 
                        grid[x, y] = new Node(true, worldPoint, x, y);
                    }
                }

            }
        }
    }
    public void CreateDynamicGrid(Vector2 startPos, Vector2 targetPos)
    {
        int minX = Mathf.FloorToInt(Mathf.Min(startPos.x, targetPos.x));
        int maxX = Mathf.CeilToInt(Mathf.Max(startPos.x, targetPos.x));
        int minY = Mathf.FloorToInt(Mathf.Min(startPos.y, targetPos.y));
        int maxY = Mathf.CeilToInt(Mathf.Max(startPos.y, targetPos.y));

        gridSizeX = maxX - minX + 1;
        gridSizeY = maxY - minY + 1;

        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = new Vector2(minX + x, minY + y);
                grid[x, y] = new Node(true, worldPoint, x, y);
            }
        }
    }

    private Node WorldPointToNode(Vector2 worldPosition, Vector2 gridOrigin)
    {
        int x = Mathf.RoundToInt(worldPosition.x - gridOrigin.x);
        int y = Mathf.RoundToInt(worldPosition.y - gridOrigin.y);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return grid[x, y];
    }


    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        CreateDynamicGrid(startPos, targetPos);

        Vector2 gridOrigin = new Vector2(Mathf.Min(startPos.x, targetPos.x), Mathf.Min(startPos.y, targetPos.y));
        Node startNode = WorldPointToNode(startPos, gridOrigin);
        Node targetNode = WorldPointToNode(targetPos, gridOrigin);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                
                if (currentNode.parent != null &&
                    (neighbour.gridX - currentNode.gridX != currentNode.gridX - currentNode.parent.gridX ||
                     neighbour.gridY - currentNode.gridY != currentNode.gridY - currentNode.parent.gridY))
                {
                    newCostToNeighbour += directionChangePenalty;
                }
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                
                if (x == 0 && y == 0 || x != 0 && y != 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

}

