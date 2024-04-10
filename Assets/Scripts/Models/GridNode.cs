using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode 
{
    #region Variables
    private Grid<GridNode> grid;
    private GridPosition gridPosition;
    private List<SC_Unit> unitList;

    #endregion

    #region ctor
    public GridNode(Grid<GridNode> grid, GridPosition gridPosition)
    {
        this.grid = grid;
        this.gridPosition = gridPosition;
        unitList = new List<SC_Unit>();
    }
    #endregion


    #region Logic
    public override string ToString()
    {
        string unitNames = "\n";
        foreach (SC_Unit unit in unitList) {
            unitNames += unit + "\n";
        }
        return gridPosition.ToString() + unitNames;
    }

    public void addUnit(SC_Unit unit) { 
        unitList.Add(unit);
    }

    public void removeUnit(SC_Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<SC_Unit> getUnitList()
    {
        return unitList;
    }

    public bool hasUnit()
    {
        return unitList.Count > 0;
    }

    public SC_Unit getUnit()
    {
        if(hasUnit())  
            return unitList[0];
        
        return null;
    }

    #endregion

}
