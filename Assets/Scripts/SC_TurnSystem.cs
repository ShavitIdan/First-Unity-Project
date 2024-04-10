using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TurnSystem : MonoBehaviour
{
    #region singleton
    public static SC_TurnSystem Instance { get; private set; }
    #endregion

    #region variables
    private int turnNumber = 1;
    private bool isPlayerTurn = true;

    #endregion

    #region Events
    public event Action OnTurnChanged;
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

    private void OnEnable()
    {
        SC_GameManager.Instance.OnRestart += OnRestart;
        SC_GameManager.Instance.OnMultiplayerGameStart += OnMultiplayerGameStart;
        Listener.OnMoveCompleted += OnMoveCompleted;


    }

    private void OnDisable()
    {
        SC_GameManager.Instance.OnRestart -= OnRestart;
        SC_GameManager.Instance.OnMultiplayerGameStart -= OnMultiplayerGameStart;

        Listener.OnMoveCompleted -= OnMoveCompleted;

    }




    #endregion

    #region EventFunctions
    private void OnRestart()
    {
        turnNumber = 1;
        isPlayerTurn = true;
        OnTurnChanged?.Invoke();

    }

    private void OnMoveCompleted(MoveEvent _Move)
    {

        if (_Move.getNextTurn() == SC_GlobalVariables.Instance.getUserId())
            isPlayerTurn = true;
        else
            isPlayerTurn = false;
        OnTurnChanged?.Invoke();

    }


    private void OnMultiplayerGameStart(string _NextTurn)
    {
        if (SC_GlobalVariables.Instance.getUserId() == _NextTurn)
            isPlayerTurn = true;
        else
            isPlayerTurn = false;
        OnTurnChanged?.Invoke();
    }
    #endregion


    #region Logic

    public void NextTurn()
    {
        if (SC_GameManager.Instance.getIsMultiplayer())
        {
            SC_UnitAction.Instance.SendActions();
        }
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;

        OnTurnChanged?.Invoke();
    }

    public int getTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    #endregion

}
