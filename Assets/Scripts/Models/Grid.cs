using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    #region Variables
    private int width;
    private int height;
    private float cellSize;
    private T[,] gridArr;
    #endregion

    #region ctor
    public Grid(int width, int height, float cellSize, Func<Grid<T>,GridPosition,T> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArr = new T[width, height]; 

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                gridArr[x,z] = createGridObject(this, gridPosition);
            }
        }
        
    }

    #endregion

    #region Logic
    public Vector3 GetWorldPos(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0 , gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPos(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt( worldPosition.x/cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab,GetWorldPos(gridPosition),Quaternion.identity);
                SC_GridDebug gridDebugNode = debugTransform.GetComponent<SC_GridDebug>();
                gridDebugNode.SetGridNode(GetGridNode(gridPosition));
            }

        }
    }

    public T GetGridNode(GridPosition gridPosition)
    {
        return gridArr[gridPosition.x, gridPosition.z];
    }

    public bool isValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
    #endregion


}
