using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GlobalVariables : MonoBehaviour 
{
    #region Singleton
    public static SC_GlobalVariables Instance { get; private set; }


    #endregion


    #region Variables
    private string userId = string.Empty;
    [SerializeField] private int turnTime;


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
    }

    #endregion

    #region GetSet

    public string getUserId()
    {
        return userId;
    }

    public void setUserId(string s)
    {
        userId = s;
    }

    public int getTurnTime()
    {
        return turnTime;
    }

    #endregion
}
