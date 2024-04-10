using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MoveAction : SC_BaseAction
{
    #region Variables
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;

    private List<Vector3> positionList;
    private int currentPosition;

    #endregion

    #region Events

    public event Action OnStartMoving;
    public event Action OnStopMoving;

    #endregion

    #region MonoBehaviour

    private void Update()
    {
        if (!isActive) 
            return;

        Vector3 targetPosition = positionList[currentPosition];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

        float stopDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

        }
        else
        {
            currentPosition++;
            if(currentPosition >= positionList.Count)
            {
                OnStopMoving?.Invoke();
                ActionComplete();
            }
            

        }

        

    }
    #endregion

    #region MainLogic

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = SC_Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPosition = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(SC_LevelGrid.Instance.getWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke();
        ActionStart(onActionComplete);

    }



    public override List<GridPosition> getValidPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        GridPosition unitPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testPosition = unitPosition + offset;

                if (!SC_LevelGrid.Instance.isValidGridPosition(testPosition))
                    continue;

                if (testPosition == unitPosition)
                    continue;

                if(SC_LevelGrid.Instance.isGridPositionEmpty(testPosition)) 
                    continue;

                if (!SC_Pathfinding.Instance.IsWalkable(testPosition))
                    continue;

                if (!SC_Pathfinding.Instance.HasPath(unitPosition,testPosition))
                    continue;

                int pathfindingMultiplier = 10;
                if (SC_Pathfinding.Instance.GetPathLength(unitPosition, testPosition) > maxMoveDistance * pathfindingMultiplier)
                    continue;

                validPositions.Add(testPosition);
            }
        }

        return validPositions;
    }



    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int numOfTargets = unit.getAction<SC_ShootAction>().GetTargetsAtPosition(gridPosition);
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = numOfTargets * 10 };
    }

    #endregion

}
