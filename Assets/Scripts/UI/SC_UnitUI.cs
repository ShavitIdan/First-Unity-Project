using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SC_UnitUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private SC_Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private SC_UnitHealth unitHealth;
    #endregion


    #region MonoBehaviour
    private void Start()
    {
        updateActionPoints();
        updateHealthBar();
    }

    private void OnEnable()
    {
        SC_Unit.OnActionPointsChanged += OnActionPointsChanged;
        unitHealth.OnDamaged += OnDamaged;
    }

    private void OnDisable()
    {
        SC_Unit.OnActionPointsChanged -= OnActionPointsChanged;
        unitHealth.OnDamaged -= OnDamaged;


    }
    #endregion


    #region Logic
    private void updateActionPoints()
    {
        actionPointsText.text = unit.getActionPoints().ToString();
    }
     private void updateHealthBar()
     {
         if (healthBarImage != null)
         {
             healthBarImage.fillAmount = unitHealth.getHealthPrecent();
         }
     }
    #endregion

    #region Event Function
    private void OnActionPointsChanged()
    {
        updateActionPoints();
    }

   

    private void OnDamaged()
    {
        updateHealthBar();
    }

    #endregion
}
