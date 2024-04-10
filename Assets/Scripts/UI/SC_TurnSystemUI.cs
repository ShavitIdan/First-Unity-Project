using AssemblyCSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_TurnSystemUI : MonoBehaviour
{
    #region Variables

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject enemyTurnVisualObject;
    [SerializeField] private GameObject enemyTurnObject;
    [SerializeField] private TextMeshProUGUI multiplayerTimer;

    private float timer;
    private bool isTimerActive;
    #endregion

    #region MonoBehaviour
  
    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            SC_TurnSystem.Instance.NextTurn();
        });

        isTimerActive = SC_GameManager.Instance.getIsMultiplayer();
        if (isTimerActive)
        {
            multiplayerTimer.gameObject.SetActive(true);
        }
        else
            multiplayerTimer.gameObject.SetActive(false);




        SC_TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
        SC_UnitAction.Instance.OnEnemyTakingAction += OnEnemyTakingAction;
        Listener.OnGameStarted += OnGameStarted;

        updateTurnText();
        updateEnemyTurnVisual();
        updateEndTurnButton();
    }


    private void OnDisable()
    {
        SC_TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        Listener.OnGameStarted -= OnGameStarted;
        SC_UnitAction.Instance.OnEnemyTakingAction -= OnEnemyTakingAction;

    }

   

    private void Update()
    {
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                SC_TurnSystem.Instance.NextTurn();
            }
            UpdateTimerText();
        }
    }


    #endregion

    #region Event Functions

  
    private void OnTurnChanged()
    {
        if (isTimerActive)
        {
            timer = SC_GlobalVariables.Instance.getTurnTime();
            UpdateTimerText();
        }

        updateTurnText();
        updateEnemyTurnVisual();
        updateEndTurnButton();
    }

    private void OnEnemyTakingAction(bool obj)
    {
        enemyTurnObject.SetActive(obj);
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        timer = SC_GlobalVariables.Instance.getTurnTime();
    }
    #endregion

    #region Logic
    private void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(timer);
        multiplayerTimer.text = $"{seconds}";
    }
    private void updateTurnText()
    {
        turnText.text = "TURN " + SC_TurnSystem.Instance.getTurnNumber();
    }

    private void updateEnemyTurnVisual()
    {
        enemyTurnVisualObject.SetActive(!SC_TurnSystem.Instance.IsPlayerTurn());
    }

    private void updateEndTurnButton()
    {
        endTurnButton.gameObject.SetActive(SC_TurnSystem.Instance.IsPlayerTurn());
    }

    #endregion

}