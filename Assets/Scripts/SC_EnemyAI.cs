using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SC_EnemyAI : MonoBehaviour
{
    #region StateMachine vars
    private enum State
    {
        WaitingForTurn,
        TakingTurn,
        OnAction
    }

    private State state;
    #endregion

    #region Variables
    private float timer;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        state = State.WaitingForTurn;
    }

    private void Start()
    {
        SC_TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }



    private void OnDisable()
    {
        SC_TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;

    }

    

    private void Update()
    {
        if (SC_GameManager.Instance.getIsMultiplayer())
            return;

        if (SC_TurnSystem.Instance.IsPlayerTurn())
            return;
        
        switch (state)
        {
            case State.WaitingForTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(setStateTakingTurn))
                        state = State.OnAction;
                    else
                        SC_TurnSystem.Instance.NextTurn();
                }
                break;
            case State.OnAction:
                break;
        }

       
     }
    #endregion


    #region Event Functions
    private void OnTurnChanged()
    {
        if (!SC_TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;

        }
    }

   
    #endregion

    #region Logic
    private void setStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn; 
    }

    private bool TryTakeEnemyAIAction(Action OnEnemyAIActionComplete)
    {

        foreach (SC_Unit enemyUnit in SC_UnitManager.Instance.getEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, OnEnemyAIActionComplete))
                return true;
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(SC_Unit enemyUnit,Action OnEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        SC_BaseAction bestAction = null;

        foreach(SC_BaseAction baseAction in enemyUnit.getBaseActions())
        {
            if (!enemyUnit.CanTakeAction(baseAction))
            {
                continue;
            }
            if (bestEnemyAIAction == null)
            {

                bestEnemyAIAction = baseAction.getBestAIAction();
                bestAction = baseAction;
             }
            else
            {

                EnemyAIAction testAction = baseAction.getBestAIAction();
                if( testAction != null && testAction.actionValue > bestEnemyAIAction.actionValue) {

                    bestEnemyAIAction = testAction;
                    bestAction = baseAction;
                }
            }
        }
        if (bestEnemyAIAction != null && enemyUnit.TryTakeAction(bestAction))
        {
            bestAction.TakeAction(bestEnemyAIAction.gridPosition, OnEnemyAIActionComplete);
            return true;
        }
        else
        {

            return false;
        }


    }

    #endregion
}
