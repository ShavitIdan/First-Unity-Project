using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SelectedUnit : MonoBehaviour
{
    #region Variables
    [SerializeField] private SC_Unit unit;

    private MeshRenderer meshRenderer;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

   

    private void Start()
    {
        SC_UnitAction.Instance.OnChangeSelectedUnit += OnChangeSelectedUnit;
        updateSelected();
    }

    private void OnDestroy()
    {
        SC_UnitAction.Instance.OnChangeSelectedUnit -= OnChangeSelectedUnit;

    }
    #endregion

    #region eventFuncitons
    private void OnChangeSelectedUnit(object sender)
    {
        updateSelected();
    }
    #endregion

    #region Logic
    private void updateSelected()
    {
        if (SC_UnitAction.Instance.GetSelectedUnit() == unit)
            meshRenderer.enabled = true;
        else
            meshRenderer.enabled = false;
    }
    #endregion

}
