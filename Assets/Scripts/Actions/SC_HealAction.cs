using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HealAction : SC_BaseAction
{
    #region Variables

    [SerializeField] private int maxHealDistance = 1;
    [SerializeField] private int healAmount = 20;
    
    private float actionTimer = 2f;
    private SC_Unit targetUnit;

    #endregion

    #region Evetns
    public event Action OnStartHealing;
    public event Action OnFinishHealing;
    #endregion

    #region MonoBehaviour
    private void Update()
    {
        if (!isActive)
            return;

        actionTimer -= Time.deltaTime;
        if (targetUnit == null || targetUnit.Equals(null))
        {
            return;
        }

        if (this.gameObject != targetUnit.gameObject)
        {
            Vector3 aimDirection = (targetUnit.getWorldPosition() - unit.getWorldPosition()).normalized;
            float rotateSpeed = 200f;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);// smooth rotate
        }

        if (actionTimer <= 0f)
        {
            OnFinishHealing?.Invoke();
            ActionComplete();
        }
        

    }
    #endregion

    #region MainLogic

    public override string GetActionName()
    {
        return "Heal";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 50 }; // highest prio after shooting
    }

    public override List<GridPosition> getValidPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        GridPosition unitPosition = unit.GetGridPosition();

        for (int x = -maxHealDistance; x <= maxHealDistance; x++)
        {
            for (int z = -maxHealDistance; z <= maxHealDistance; z++)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testPosition = unitPosition + offset;

                if (!SC_LevelGrid.Instance.isValidGridPosition(testPosition))
                    continue;

                SC_Unit targetUnit = SC_LevelGrid.Instance.getUnitAtGridPosition(testPosition);
                if(targetUnit != null)
                {
                    if (targetUnit.getUnitOwner() != unit.getUnitOwner()) // cant heal enemy
                    {
                        continue;
                    }
                    if (!targetUnit.getCanBeHealed()) // cant heal full hp
                    {
                        continue;
                    }
                }

                if (!SC_LevelGrid.Instance.isGridPositionEmpty(testPosition)) // gridPosition empty
                    continue;

                if (!SC_Pathfinding.Instance.IsWalkable(testPosition)) // if there is obstacle
                    continue;

                validPositions.Add(testPosition);
            }
        }

        return validPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action actionComplete)
    {
        
        actionTimer = 2f;
        targetUnit = SC_LevelGrid.Instance.getUnitAtGridPosition(gridPosition);
        if (targetUnit == null || targetUnit.Equals(null))
        {
            actionComplete();
            unit.setActionPoints(unit.getActionPoints() + 1);
            return;
        }
        OnStartHealing?.Invoke();
        targetUnit.Heal(healAmount);


        ActionStart(actionComplete);

    }
    #endregion

}
