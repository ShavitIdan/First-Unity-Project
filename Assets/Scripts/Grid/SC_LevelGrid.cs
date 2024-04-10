using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LevelGrid : MonoBehaviour
{
    #region Singleton

    public static SC_LevelGrid Instance { get; private set; }


    #endregion

    #region Variables

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private Grid<GridNode> grid;

    #endregion

    #region Events

    public event Action OnAnyUnitMoved;

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

        grid = new Grid<GridNode>(width, height, cellSize, (Grid<GridNode> g,GridPosition gridPosition) => new GridNode(g,gridPosition));
    }

    private void Start()
    {
        SC_Pathfinding.Instance.SetUp(width, height, cellSize);
    }

    #endregion

    #region Logic

    public void addUnitAtPos(GridPosition gridPosition, SC_Unit unit)
    {
        GridNode node = grid.GetGridNode(gridPosition);
        node.addUnit(unit);
    }

    public List<SC_Unit> getUnitListAtPos(GridPosition gridPosition)
    {
        GridNode node = grid.GetGridNode(gridPosition);
        return node.getUnitList();

    }

    public void RemoveUnitFromPos(GridPosition gridPosition, SC_Unit unit)
    {
        GridNode node = grid.GetGridNode(gridPosition);
        node.removeUnit(unit);
    }

    public GridPosition getGridPosition(Vector3 position)
    {
        return grid.GetGridPos(position);
    }

    public int getWidth()
    {
        return grid.getWidth();
    }

    public int getHeight()
    {
        return grid.getHeight();
    }
    public Vector3 getWorldPosition(GridPosition gridPosition)
    {
        return grid.GetWorldPos(gridPosition);
    }

    public bool isValidGridPosition(GridPosition gridPosition)
    {
        return grid.isValidGridPosition(gridPosition);
    }

    public bool isGridPositionEmpty(GridPosition gridPosition)
    {
        GridNode node = grid.GetGridNode(gridPosition);
        return node.hasUnit();

    }

    public SC_Unit getUnitAtGridPosition(GridPosition gridPosition)
    {
        GridNode node = grid.GetGridNode(gridPosition);
        return node.getUnit();

    }

    public void UnitMoved(SC_Unit unit , GridPosition from, GridPosition to )
    {
        RemoveUnitFromPos(from, unit);
        addUnitAtPos(to, unit);

        OnAnyUnitMoved?.Invoke();
    }

    #endregion


}
