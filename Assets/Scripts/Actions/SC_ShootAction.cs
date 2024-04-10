using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ShootAction : SC_BaseAction
{
    #region State Enum
    private enum State
    {
        Aiming,
        Shooting,
        delay
    }

    #endregion

    #region Variables

    private State state;    
    [SerializeField] private int maxShootDistance = 7;
    private float stateTimer;
    private SC_Unit targetUnit;
    private bool canShootBullet;
    [SerializeField] private int rifleDamage = 40;
    [SerializeField] private LayerMask obstacleLayerMask;
    #endregion

    #region Event
    public event Action<SC_Unit> OnShoot;
    #endregion


    #region MonoBehaviour
    private void Update()
    {
        if (!isActive)
            return;
        //state machine
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.getWorldPosition() - unit.getWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);

                break; 

            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;

            case State.delay:
                break;
        }

        if (stateTimer <= 0f)
            nextState();

    }

    #endregion

    #region Logic
    private void nextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
              
                break;

            case State.Shooting:
                
                state = State.delay;
                float delayStateTime = 0.5f;
                stateTimer = delayStateTime;
          
                break;

            case State.delay:
                
                ActionComplete();
    
                break;
        }
    }


    private void Shoot()
    {
        OnShoot?.Invoke(targetUnit);
        targetUnit.Damage(rifleDamage);
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> getValidPositions()
    {
        GridPosition unitPosition = unit.GetGridPosition();

        return getValidPositions(unitPosition);
    }


    public List<GridPosition> getValidPositions(GridPosition unitPosition)
    {
        List<GridPosition> validPositions = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testPosition = unitPosition + offset;

                if (!SC_LevelGrid.Instance.isValidGridPosition(testPosition))
                    continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); //circle
                if (testDistance > maxShootDistance) // out of range
                    continue;

                if (!SC_LevelGrid.Instance.isGridPositionEmpty(testPosition)) // there is no unit on grid position
                    continue;

                SC_Unit targetUnit =  SC_LevelGrid.Instance.getUnitAtGridPosition(testPosition);

                if (targetUnit == null || targetUnit.getUnitOwner() == unit.getUnitOwner()) //targetUnit.IsEnemy() == unit.IsEnemy()) // same team
                {
                    continue;
                }

                Vector3 unitWorldPosition = SC_LevelGrid.Instance.getWorldPosition(unitPosition);
                Vector3 shootDirection = (targetUnit.getWorldPosition() - unitWorldPosition).normalized;
                float unitHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitHeight , 
                    shootDirection, Vector3.Distance(unitWorldPosition, targetUnit.getWorldPosition()), obstacleLayerMask)) // shoot on the head instad of feet
                {
                    continue;
                }

                validPositions.Add(testPosition);
            }
        }

        return validPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action actionComplete)
    {


        targetUnit = SC_LevelGrid.Instance.getUnitAtGridPosition(gridPosition);


        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;
        ActionStart(actionComplete);

    }

    public SC_Unit getTargetUnit()
    {
        return targetUnit;
    }

    public int getMaxShootDistance()
    {
        return maxShootDistance;
    }

    
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        SC_Unit target = SC_LevelGrid.Instance.getUnitAtGridPosition(gridPosition);

        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 100 + Mathf.RoundToInt((1 - target.getHealth()) * 100f) };
        //getting higher priority on the lower hp unit
    }

    public int GetTargetsAtPosition(GridPosition gridPosition)
    {
        return getValidPositions(gridPosition).Count; 
    }

    #endregion



}
