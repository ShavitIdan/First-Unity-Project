using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TakingActionUI : MonoBehaviour
{
    #region MonoBehaviour
    private void Start()
    {
        SC_UnitAction.Instance.OnTakingAction += OnTakingAction;
        Hide();

    }
    #endregion

    #region Logic

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnTakingAction(bool isOnAction)
    {
        if(isOnAction)
            Show();
        else
            Hide();
    }
    #endregion
}
