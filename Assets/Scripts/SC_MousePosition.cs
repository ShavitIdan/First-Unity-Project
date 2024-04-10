using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MousePosition : MonoBehaviour
{
    #region Singleton
    private static SC_MousePosition Instance;
    #endregion


    #region Variables
    [SerializeField] private LayerMask mouseMask;
    #endregion

    #region Monobehaviour
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
    }
    #endregion

    #region Logic
    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray,out RaycastHit hitInfo, float.MaxValue, Instance.mouseMask);
        return hitInfo.point;
    }

    #endregion
}
