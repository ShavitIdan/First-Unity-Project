using AssemblyCSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_UnitManager : MonoBehaviour
{
    #region Singleton
    public static SC_UnitManager Instance { get; private set; }

    #endregion

    #region Variables

    [SerializeField] private Transform friendlyUnitPrefab;
    [SerializeField] private Transform enemyUnitPrefab;
    [SerializeField] private Transform enemyAIUnitPrefab;




    private List<SC_Unit> unitList;
    private List<SC_Unit> friendlyUnitList;
    private List<SC_Unit> enemyUnitList;
    private int UnitCounter = 0;

    private List<Vector3> friendlyPositions = new List<Vector3>
    {
        new Vector3 (2,0,2),
        new Vector3 (4,0,4),
        new Vector3 (6,0,2)
    };

    private List<Vector3> enemyAIPositions = new List<Vector3>
    {
        new Vector3(44,0,52),
        new Vector3 (52,0,26),
        new Vector3 (14,0,54)
    };

    private List<Vector3> enemyPositions = new List<Vector3>
    {
        new Vector3(36,0,56),
        new Vector3 (32,0,56),
        new Vector3 (34,0,54)
    };
    #endregion

    #region Events
    public static event Action OnDefeat;
    public static event Action OnWin;
    #endregion

    #region MonoBehaviour

    private void Start()
    {

        SC_Unit.OnAnyUnitSpawn += OnAnyUnitSpawn;
        SC_Unit.OnAnyUnitDeath += OnAnyUnitDeath;
        
        InstantiateAllyUnit();
        if (SC_GameManager.Instance.getIsMultiplayer())
        {
            InstantiateEnemyUnit();
        }
        else
        {     
            InstantiateEnemyAIUnit();
        }

    }

    private void OnEnable()
    {
        SC_GameManager.Instance.OnRestart += OnRestart;

    }

    private void OnDisable()
    {
        SC_Unit.OnAnyUnitSpawn -= OnAnyUnitSpawn;
        SC_Unit.OnAnyUnitDeath -= OnAnyUnitDeath;
        SC_GameManager.Instance.OnRestart -= OnRestart;


    }



    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }


        unitList = new List<SC_Unit>();
        friendlyUnitList = new List<SC_Unit>();
        enemyUnitList = new List<SC_Unit>();
    }

#endregion

    private void OnAnyUnitSpawn(object sender)
    {

        SC_Unit unit = sender as SC_Unit;

        unitList.Add(unit);

        if (unit.getUnitOwner() != SC_GlobalVariables.Instance.getUserId())
            enemyUnitList.Add(unit);
        else { 
            friendlyUnitList.Add(unit);
            SC_FogManager.Instance.addRevealer(unit);
        }

        if (unitList.Count >= 1 && friendlyUnitList.Count > 0)
            SC_UnitAction.Instance.SelectUnit(friendlyUnitList[0]);

    }

    private void OnAnyUnitDeath(object sender)
    {
        SC_Unit unit = sender as SC_Unit;

        unitList.Remove(unit);
        

        if (unit.getUnitOwner() != SC_GlobalVariables.Instance.getUserId())
        {
            enemyUnitList.Remove(unit);
            if (enemyUnitList.Count <= 0)
            {
                if(SC_GameManager.Instance.getIsMultiplayer())
                    SC_UnitAction.Instance.SendActions();
                OnWin?.Invoke();

            }
        }
        else
        {
            SC_FogManager.Instance.removeRevealer(unit);
            friendlyUnitList.Remove(unit);
            if (friendlyUnitList.Count <= 0) {
                if (SC_GameManager.Instance.getIsMultiplayer())
                    SC_UnitAction.Instance.SendActions();
                OnDefeat?.Invoke();
            }
            else if (unit == SC_UnitAction.Instance.GetSelectedUnit())
                SC_UnitAction.Instance.SelectUnit(friendlyUnitList[0]);
        }
    }

    private void OnRestart()
    {
        clearUnitsFromBoard();
        clearRagdollsFromBoard();

        InstantiateAllyUnit();
        InstantiateEnemyAIUnit();
    }

    private void InstantiateAllyUnit()
    {
        foreach (Vector3 position in friendlyPositions)
        {
            Transform newUnit = Instantiate(friendlyUnitPrefab, position, Quaternion.identity);
            
            newUnit.gameObject.name = "AllyUnit" + UnitCounter;
            UnitCounter++;
            SC_Unit unit = newUnit.gameObject.GetComponent<SC_Unit>();

            if (SC_GameManager.Instance.getIsMultiplayer() && SC_GameManager.Instance.getNextTurnPlayer() != SC_GlobalVariables.Instance.getUserId())
            {
                    unit.setUnitOwner("Enemy");
            }
            else
                unit.setUnitOwner(SC_GlobalVariables.Instance.getUserId());

            GridPosition gridPosition = SC_LevelGrid.Instance.getGridPosition(position);
            SC_LevelGrid.Instance.addUnitAtPos(gridPosition, unit);

        }
    }

    private void InstantiateEnemyUnit()
    {
        foreach (Vector3 position in enemyPositions)
        {
            Transform newUnit = Instantiate(enemyUnitPrefab, position, Quaternion.Euler(0, 180, 0));
            newUnit.gameObject.name = "EnemyUnit" + UnitCounter;
            UnitCounter++;
            SC_Unit unit = newUnit.gameObject.GetComponent<SC_Unit>();

            if (SC_GameManager.Instance.getIsMultiplayer() && SC_GameManager.Instance.getNextTurnPlayer() != SC_GlobalVariables.Instance.getUserId())
            {
                unit.setUnitOwner(SC_GlobalVariables.Instance.getUserId());
            }
            else
                unit.setUnitOwner("Enemy");


            GridPosition gridPosition = SC_LevelGrid.Instance.getGridPosition(position);
            SC_LevelGrid.Instance.addUnitAtPos(gridPosition, unit);
        }
    }

    private void InstantiateEnemyAIUnit()
    {
        foreach (Vector3 position in enemyAIPositions)
        {
            Transform newUnit = Instantiate(enemyAIUnitPrefab, position, Quaternion.identity);
            newUnit.gameObject.name = "EnemyAIUnit" + UnitCounter;
            UnitCounter++;
            SC_Unit unit = newUnit.gameObject.GetComponent<SC_Unit>();
            unit.setUnitOwner("EnemyAI");
            GridPosition gridPosition = SC_LevelGrid.Instance.getGridPosition(position);
            SC_LevelGrid.Instance.addUnitAtPos(gridPosition, unit);
        }
    }


    private void clearUnitsFromBoard()
    {

        for (int i = unitList.Count - 1; i >= 0; i--)
        {
            unitList[i].Damage(unitList[i].getCurrentHealth());
        }
    }

    private void clearRagdollsFromBoard()
    {
        for (int i = SC_UnitRagdoll.ragdollList.Count - 1; i >= 0; i--)
        {
            Destroy(SC_UnitRagdoll.ragdollList[i].gameObject);
            SC_UnitRagdoll.ragdollList.RemoveAt(i);
        }
    }

    public List<SC_Unit> getUnitList()
    {
        return unitList;
    }
    public List<SC_Unit> getEnemyUnitList()
    {
        return enemyUnitList;
    }
    public List<SC_Unit> getFriendlyUnitList()
    {
        return friendlyUnitList;
    }




}
