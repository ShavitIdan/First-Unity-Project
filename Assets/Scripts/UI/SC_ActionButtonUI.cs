using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SC_ActionButtonUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;

    private Image buttonImage;
    private SC_BaseAction baseAction;

    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    #endregion

    #region Logic

    public void SetBaseAction(SC_BaseAction baseAction)
    {
        this.baseAction = baseAction;
        text.text = baseAction.GetActionName().ToUpper();
        button.onClick.AddListener(() => {
            SC_UnitAction.Instance.SetSelectedAction(baseAction);
            UpdateSelectedButton();
        });
    }

    public void UpdateSelectedButton()
    {
        SC_BaseAction selectedBaseAction = SC_UnitAction.Instance.getSelecetedAction();
        
    }

    #endregion




}
