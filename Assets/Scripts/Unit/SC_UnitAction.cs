using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_UnitAction : MonoBehaviour
{

    #region Singleton

    public static SC_UnitAction Instance { get; private set; }

    #endregion

    #region Variables

    [SerializeField] private SC_Unit selectedUnit;
    [SerializeField] private LayerMask unitsMask;

    private SC_BaseAction selectedAction;
    private bool isOnAction;
    private Dictionary<string,object> actionsDictionary;
    private int ActionNumber = 0;
    #endregion

    #region Events
    public event Action<object> OnChangeSelectedUnit;
    public event Action<object> OnChangeSelectedAction;
    public event Action<bool> OnTakingAction;
    public event Action onActionStarted;
    public event Action<bool> OnEnemyTakingAction;
    

    #endregion

    #region MonoBehaviour

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

        actionsDictionary = new Dictionary<string, object>();

    }

    


    private void Update()
    {
        if (isOnAction)
            return;

        if (!SC_TurnSystem.Instance.IsPlayerTurn())
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (handleUnitSelection())
                return;
        
        HandleSelectedAction();
    }

    private void OnEnable()
    {
        Listener.OnMoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        Listener.OnMoveCompleted -= OnMoveCompleted;

    }

    



    #endregion

    #region Logic
    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mousePosition = SC_LevelGrid.Instance.getGridPosition(SC_MousePosition.GetPosition());
            if (selectedAction.isValidPosition(mousePosition))
            {
                if (selectedUnit.TryTakeAction(selectedAction))
                {
                    SetOnAction();
                    selectedAction.TakeAction(mousePosition, finishAction);

                    if (SC_GameManager.Instance.getIsMultiplayer())
                    {
                        if (actionsDictionary == null)
                            actionsDictionary = new Dictionary<string, object>();
                        ActionData action = new ActionData(selectedUnit.GetGridPosition(), mousePosition);
                        actionsDictionary.Add(ActionNumber.ToString(), action);
                        ActionNumber++;
                    }
                    onActionStarted?.Invoke();
                }

                
            }
        }
    }

    public void SendActions()
    {
        
        string toJason = MiniJSON.Json.Serialize(actionsDictionary);  
        WarpClient.GetInstance().sendMove(toJason);
       
    }

    
   
    private IEnumerator PerformActionsWithDelay(Dictionary<string, object> actions, float delayInSeconds)
    {
        if (actions == null || actions.Count == 0)
        {
            yield break;
        }
        OnEnemyTakingAction?.Invoke(true);
        foreach (var v in actions)
        {
            string s = v.Value as string;
            Debug.Log("Action: " + s);
            var data = MiniJSON.Json.Deserialize(s) as Dictionary<string, object>;

            if (data == null)
            {
                Debug.Log("continue action == null");
                continue;
            }

            ActionData action = new ActionData(new GridPosition(data["unitPosition"] as string), new GridPosition(data["targetPosition"] as string));
            Debug.Log("Performing action for unit at position: " + action.unitPosition + " to target location: " + action.targetPosition);
            if (action == null)
                continue;
            SetOnAction();
            getActionTypeFromData(action).TakeAction(action.targetPosition, finishAction);
            onActionStarted?.Invoke();

            yield return new WaitForSeconds(delayInSeconds);
        }
        OnEnemyTakingAction?.Invoke(false);
        actionsDictionary = new Dictionary<string, object>();
        ActionNumber = 0;
    }


    private SC_BaseAction getActionTypeFromData(ActionData action)
    {

        if (action == null)
        {
            Debug.Log("Action cannot be found (null)");
            return null;

        }

        SC_Unit unit = SC_LevelGrid.Instance.getUnitAtGridPosition(action.unitPosition);
        SC_Unit target = SC_LevelGrid.Instance.getUnitAtGridPosition(action.targetPosition);

        if (!target)
        {
            Debug.Log("Move_Action");
            return unit.gameObject.GetComponent<SC_MoveAction>();
        }
        else
        {
            if(target.getUnitOwner() == unit.getUnitOwner())
            {
                Debug.Log("Heal_Action");
                return unit.gameObject.GetComponent<SC_HealAction>();

            }
            else
            {
                Debug.Log("Shoot_Action");
                return unit.gameObject.GetComponent<SC_ShootAction>();

            }
        }
    }



    private void SetOnAction()
    {
        isOnAction = true;
        OnTakingAction?.Invoke(isOnAction);
    }

    private void finishAction()
    {
        isOnAction = false;
        OnTakingAction?.Invoke(isOnAction);

    }

    private bool handleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0)) { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitsMask))
            {
                SC_Unit unit =  hitInfo.transform.GetComponent<SC_Unit>();
                if(unit != null )
                {
                    if (unit == selectedUnit)
                        return false;
                    
                    if(unit.IsEnemy() || unit.getUnitOwner() != SC_GlobalVariables.Instance.getUserId() )
                        return false;

                    SelectUnit(unit);
                    return true;
                }
            }
        }
        return false;
        
    }

    public void SelectUnit(SC_Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.getAction<SC_MoveAction>());
        OnChangeSelectedUnit?.Invoke(this);

    }

    public void SetSelectedAction(SC_BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnChangeSelectedAction?.Invoke(this);

    }

    public SC_Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public SC_BaseAction getSelecetedAction()
    {
        return selectedAction;
    }

    #endregion

    #region EventFunctions

    private void OnMoveCompleted(MoveEvent _Move)
    {

        if (_Move.getSender() != SC_GlobalVariables.Instance.getUserId() && _Move.getMoveData() != null)
        {
            Debug.Log($"Move sender: {_Move.getSender()}, Local User ID: {SC_GlobalVariables.Instance.getUserId()}, Move data: {_Move.getMoveData()}");
            Debug.Log("Move data received: " + _Move.getMoveData());
            ReceiveActions(_Move.getMoveData());
        }
        //else if (_Move.getMoveData() == null)
          //SC_TurnSystem.Instance.NextTurn();




        if (SC_GameManager.Instance.IsGameEnd())
        {
            WarpClient.GetInstance().stopGame();
        }


    }

    public void ReceiveActions(string actions)
    {
        Debug.Log("ReceiveActions: " + actions);
        actionsDictionary = MiniJSON.Json.Deserialize(actions) as Dictionary<string, object>;
        Debug.Log("Deserialized actions count: " + actionsDictionary?.Count);
        if (actionsDictionary == null)
        {
            Debug.LogError("Failed to deserialize actions dictionary");
        }

        StartCoroutine(PerformActionsWithDelay(actionsDictionary,3f));
        Debug.Log("Actions performed: " + actionsDictionary?.Count);
    }




    #endregion

}
