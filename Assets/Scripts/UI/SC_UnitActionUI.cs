using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SC_UnitActionUI : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<SC_ActionButtonUI> actionButtonList;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        actionButtonList = new List<SC_ActionButtonUI>();
    }

    private void Start()
    {
        //CreateUnitActionButtons();
        //UpdateSelectedActionVisual();
        //UpdateActionPoints();

        SC_TurnSystem.Instance.OnTurnChanged += OnTurnChanged;

    }

    private void OnEnable()
    {
        SC_UnitAction.Instance.OnChangeSelectedUnit += OnChangeSelectedUnit;
        SC_UnitAction.Instance.OnChangeSelectedAction += OnChangeSelectedAction;
        SC_UnitAction.Instance.onActionStarted += onActionStarted;
        SC_Unit.OnActionPointsChanged += OnActionPointsChanged;


    }

    private void OnDisable()
    {
        SC_UnitAction.Instance.OnChangeSelectedUnit -= OnChangeSelectedUnit;
        SC_UnitAction.Instance.OnChangeSelectedAction -= OnChangeSelectedAction;
        SC_UnitAction.Instance.onActionStarted -= onActionStarted;
        SC_TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        SC_Unit.OnActionPointsChanged -= OnActionPointsChanged;


    }

    #endregion

    #region Logic
    public void CreateUnitActionButtons()
    {
        foreach(Transform t in actionButtonContainer)
        {
            Destroy(t.gameObject);
        }

        actionButtonList.Clear();

        SC_Unit unit = SC_UnitAction.Instance.GetSelectedUnit();

        foreach(SC_BaseAction baseAction in unit.getBaseActions())
        {
            Transform actionButton = Instantiate(actionButtonPrefab, actionButtonContainer);
            SC_ActionButtonUI actionButtonUI = actionButton.GetComponent<SC_ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            actionButtonList.Add(actionButtonUI);


        }
    }
    private void UpdateSelectedActionVisual()
    {
        foreach (SC_ActionButtonUI actionButtonUI in actionButtonList)
        {
            actionButtonUI.UpdateSelectedButton();
        }
    }
    private void UpdateActionPoints()
    {
        int points = SC_UnitAction.Instance.GetSelectedUnit().getActionPoints();
        actionPointsText.text = "Action Points: " + points;
    }

    #endregion


    #region Event Functions
    private void OnChangeSelectedUnit(object sender)
    {
        CreateUnitActionButtons();
        UpdateSelectedActionVisual();
        UpdateActionPoints();

    }

    private void OnChangeSelectedAction(object sender)
    {
        UpdateSelectedActionVisual();
    }

   

    private void onActionStarted() { 
        
        UpdateActionPoints();
            
    }

    private void OnActionPointsChanged()
    {
        UpdateActionPoints();
    }

    


    private void OnTurnChanged()
    {
        UpdateActionPoints();
    }
    #endregion
}
