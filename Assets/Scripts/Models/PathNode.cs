using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    #region Variables
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode prevPathNode;
    private bool isWalkable = true;
    #endregion

    #region ctor
    public PathNode (GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    #endregion


    #region Logic
    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int getGCost()
    { 
        return gCost; 
    }
    public int getFCost()
    {
        return fCost;
    }
    public int getHCost()
    {
        return hCost;
    }

    public void setGChost(int gCost)
    {
        this.gCost = gCost;
    }

    public void setHCost(int hCost)
    {
        this.hCost = hCost;
    }

    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void resetPrevPathNode()
    {
        prevPathNode = null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void SetPrevNode(PathNode prevPathNode)
    {
        this.prevPathNode = prevPathNode;
    }

    public PathNode GetPrevNode()
    {
        return prevPathNode;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    #endregion

}
