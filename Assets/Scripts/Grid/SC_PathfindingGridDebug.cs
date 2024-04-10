using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PathfindingGridDebug : SC_GridDebug
{

    #region Variable
    private PathNode pathNode;
    #endregion

    #region override
    public override void SetGridNode(object g)
    {
        base.SetGridNode(g);
        pathNode = (PathNode)g;
    }
    #endregion
}
