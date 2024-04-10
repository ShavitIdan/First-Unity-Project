using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Pathfinding : MonoBehaviour
{
    #region Singleton
    public static SC_Pathfinding Instance { get; private set; }
    #endregion

    #region Consts

    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;
    #endregion

    #region Variables
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;
    
    private int width;
    private int height;
    private float cellSize;
    private Grid<PathNode> grid;

    #endregion

    #region MonoBehaviour
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }


    }

    #endregion

    #region Logic

    public void SetUp(int width, int  height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        grid = new Grid<PathNode>(width, height, cellSize, (Grid<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //grid.CreateDebugObjects(gridDebugObjectPrefab); // to see grid number (x,z)

        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = SC_LevelGrid.Instance.getWorldPosition(gridPosition);
                float raycastOffset = 5f;
                if(Physics.Raycast(worldPosition + Vector3.down * raycastOffset, Vector3.up, raycastOffset * 2, obstaclesLayerMask))
                {
                    getNode(x,z).SetIsWalkable(false);
                }
            }
        }



    }

    public List<GridPosition> FindPath(GridPosition startPosition, GridPosition endPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = grid.GetGridNode(startPosition);
        PathNode endNode = grid.GetGridNode(endPosition);
        openList.Add(startNode);

        for (int x = 0; x < grid.getWidth(); x++)
        {
            for(int z = 0; z < grid.getHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = grid.GetGridNode(gridPosition);

                pathNode.setGChost(int.MaxValue);
                pathNode.setHCost(0);
                pathNode.calculateFCost();
                pathNode.resetPrevPathNode();


            }
        }

        startNode.setGChost(0);
        startNode.setHCost(calculateDistance(startPosition,endPosition));
        startNode.calculateFCost();
        
        while (openList.Count > 0)
        {
            PathNode currentNode = getBestNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.getFCost();
                return calculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);
        
            foreach(PathNode neighbourNode in getNeighbours(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                    continue;

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tempGCost = currentNode.getGCost() + calculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if(tempGCost < neighbourNode.getGCost())
                {
                    neighbourNode.SetPrevNode(currentNode);
                    neighbourNode.setGChost(tempGCost);
                    neighbourNode.setHCost(calculateDistance(neighbourNode.GetGridPosition(), endPosition));
                    neighbourNode.calculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);

                }

            }

        }

        pathLength = 0;
        return null;
    }

    private List<GridPosition> calculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes= new List<PathNode>();
        pathNodes.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetPrevNode() != null)
        {
            pathNodes.Add(currentNode.GetPrevNode());
            currentNode = currentNode.GetPrevNode();
        }

        pathNodes.Reverse();

        List<GridPosition> gridPositions = new List<GridPosition>();

        foreach (PathNode pathNode in pathNodes)
        {
            gridPositions.Add(pathNode.GetGridPosition());
        }

        return gridPositions;

    }

    public int calculateDistance(GridPosition a, GridPosition b)
    {
        GridPosition gridPositionDistance = a - b;
        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MoveDiagonalCost * Mathf.Min(xDistance,zDistance) + MoveStraightCost * remaining;
    }

    private PathNode getBestNode(List<PathNode> pathNodes)
    {
        PathNode bestNode = pathNodes[0];

        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].getFCost() < bestNode.getFCost())
                bestNode = pathNodes[i];
        }
        return bestNode;
    }
    
    private PathNode getNode(int x, int z)
    {
        return grid.GetGridNode(new GridPosition(x, z));
    }

    private List<PathNode> getNeighbours (PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();


        GridPosition gridPosition = currentNode.GetGridPosition();
        
        if(gridPosition.x - 1  >= 0)
        {
            neighbourList.Add(getNode(gridPosition.x - 1, gridPosition.z + 0));
            
            if (gridPosition.z  - 1 >= 0)
                neighbourList.Add(getNode(gridPosition.x - 1, gridPosition.z - 1));
            
            if(gridPosition.z + 1 < grid.getHeight())
                neighbourList.Add(getNode(gridPosition.x - 1, gridPosition.z + 1));

        }
        if (gridPosition.x + 1 < grid.getWidth()) 
        {
            neighbourList.Add(getNode(gridPosition.x + 1, gridPosition.z + 0));

            if (gridPosition.z - 1 >= 0)    
                neighbourList.Add(getNode(gridPosition.x + 1, gridPosition.z - 1));
            
            if(gridPosition.z + 1 < grid.getHeight())
                neighbourList.Add(getNode(gridPosition.x + 1, gridPosition.z + 1));
            
        }
        if (gridPosition.z - 1 >= 0)
            neighbourList.Add(getNode(gridPosition.x + 0, gridPosition.z - 1));
        
        if(gridPosition.z + 1 < grid.getHeight())
            neighbourList.Add(getNode(gridPosition.x + 0, gridPosition.z + 1));

        return neighbourList;
    }

    public bool IsWalkable(GridPosition gridPosition)
    {
        return grid.GetGridNode(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition,endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    #endregion
}
