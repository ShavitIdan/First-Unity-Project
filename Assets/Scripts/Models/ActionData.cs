using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionData
{
    #region Variables

    public GridPosition unitPosition;
    public GridPosition targetPosition;

    #endregion

    #region ctor

    public ActionData(GridPosition unit, GridPosition target)
    {
        this.unitPosition = unit;
        this.targetPosition = target;
    }

    #endregion

    #region overrides

    public override string ToString()
    {
        return "{ \"unitPosition\": " + unitPosition.ToString() + ", \"targetPosition\": " + targetPosition.ToString() + " }";
    }

    #endregion
}
