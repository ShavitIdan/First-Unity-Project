using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SC_Unit : MonoBehaviour
{

    #region Variables

    [SerializeField] private const int MAX_ACTION_POINTS = 2;
    [SerializeField] private bool isEnemy;
    private string unitOwner;

    private GridPosition gridPosition;
    private SC_BaseAction[] baseActions;
    private SC_UnitHealth unitHealth;
    private int actionPoints = MAX_ACTION_POINTS;

    #endregion

    #region Events

    public static event Action OnActionPointsChanged;
    public static event Action<object> OnAnyUnitSpawn;
    public static event Action<object> OnAnyUnitDeath;

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        baseActions = GetComponents<SC_BaseAction>();
        unitHealth = GetComponent<SC_UnitHealth>();
    }

    private void Start()
    {
        gridPosition = SC_LevelGrid.Instance.getGridPosition(transform.position);
        SC_LevelGrid.Instance.addUnitAtPos(gridPosition, this);
        SC_TurnSystem.Instance.OnTurnChanged += OnTurnChanged;

        OnAnyUnitSpawn?.Invoke(this);

    }

    private void OnEnable()
    {
        unitHealth.OnDead += OnDead;
    }



    private void OnDisable()
    {
        SC_TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        unitHealth.OnDead -= OnDead;


    }

    void Update()
    {
        GridPosition newGridPosition = SC_LevelGrid.Instance.getGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            SC_LevelGrid.Instance.UnitMoved(this, oldGridPosition, newGridPosition);

        }
    }
    #endregion

    #region Logic
    public T getAction<T>() where T : SC_BaseAction
    {
        foreach (SC_BaseAction baseAction in baseActions)
        {
            if (baseAction is T)
                return (T)baseAction;

        }
        return null;
    }
    
    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 getWorldPosition()
    {
        return transform.position;
    }

    public SC_BaseAction[] getBaseActions()
    {
        return baseActions;
    }
    public bool CanTakeAction(SC_BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionCost();
    }

    public bool TryTakeAction(SC_BaseAction baseAction)
    {
        if (CanTakeAction(baseAction))
        {
            useActionPoints(baseAction.GetActionCost());
            return true;
        }
        return false;
    }

    private void useActionPoints(int amount)
    {
        actionPoints -= amount;
        OnActionPointsChanged?.Invoke();
    }


    public int getActionPoints()
    {
        return actionPoints;
    }

    public void setActionPoints(int amount)
    {
        actionPoints = amount;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void SetIsEnemy(bool b)
    {
        isEnemy = b;
    }

    public void Damage(int damageAmount)
    {
        unitHealth.Damage(damageAmount);
    }

    public float getHealth()
    {
        return unitHealth.getHealthPrecent();
    }

    public int getCurrentHealth()
    {
        return unitHealth.getCurrentHealth();
    }

    public bool getCanBeHealed()
    {
        return unitHealth.getCanBeHealed();
    }

    public void Heal(int healAmount)
    {
        unitHealth.Heal(healAmount);
    }

    public string getUnitOwner()
    {
        return unitOwner;
    }

    public void setUnitOwner(string _unitOwner)
    {
        unitOwner = _unitOwner;
    }

    #endregion

    #region Events Functions
    private void OnTurnChanged()
    {
        if (IsEnemy() && !SC_TurnSystem.Instance.IsPlayerTurn() || !IsEnemy() && SC_TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = MAX_ACTION_POINTS;

            OnActionPointsChanged?.Invoke();
        }
    }

   

    private void OnDead()
    {
        SC_LevelGrid.Instance.RemoveUnitFromPos(gridPosition,this);
        Destroy(gameObject);
        OnAnyUnitDeath?.Invoke(this );
    }


    #endregion
}
