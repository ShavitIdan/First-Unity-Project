using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SC_BaseAction : MonoBehaviour
{

    #region Variables

    protected SC_Unit unit;
    protected bool isActive;
    protected Action onActionComplete;
    #endregion

    #region Events

    public static event Action<object> OnActionStarted;
    public static event Action<object> OnActionCompleted;
    #endregion

    #region MonoBehaviour
    protected virtual void Awake()
    {
        
        unit = GetComponent<SC_Unit>();
    }
    #endregion

    #region Logic
    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action actionComplete);

    public virtual bool isValidPosition(GridPosition gridPosition)
    {
        List<GridPosition> validPositions = getValidPositions();
        return validPositions.Contains(gridPosition);
    }

    public abstract List<GridPosition> getValidPositions();

    public virtual int GetActionCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnActionStarted?.Invoke(this);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete?.Invoke();

        OnActionCompleted?.Invoke(this);
    }

    public SC_Unit getUnit()
    {
        return unit;
    }

    public EnemyAIAction getBestAIAction()
    {
        List<EnemyAIAction> enemyAIActions = new List<EnemyAIAction>();
        List<GridPosition> validActionGridPositions = getValidPositions();
        
        foreach (GridPosition position in validActionGridPositions)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(position);
            enemyAIActions.Add(enemyAIAction);
        }

        if (enemyAIActions.Count > 0) { 
            enemyAIActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActions[0];
        }
        else
        {
            return null;
        }

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    #endregion

}
