using com.shephertz.app42.gaming.multiplayer.client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_GameManager : MonoBehaviour
{
    #region Singleton

    public static SC_GameManager Instance { get; private set; }


    #endregion

    #region Variables

    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private GameObject gameEndWindow;
    [SerializeField] private GameObject gameWindow;
    [SerializeField] private GameObject menuWindow;
    [SerializeField] private Button restartButton;

    private bool isMultiplayer = false;
    private bool isGameEnd = false;
    private string NextTurnPlayer;

    #endregion

    #region Events
    public event Action OnRestart;
    public event Action<string> OnMultiplayerGameStart;


    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    private void Start()
    {
        gameEndWindow.SetActive(false);
        gameWindow.SetActive(false);
        menuWindow.SetActive(true);
    }

    private void OnEnable()
    {
        SC_UnitManager.OnDefeat += OnDefeat;
        SC_UnitManager.OnWin += OnWin;

    }

    private void OnDisable()
    {
        SC_UnitManager.OnDefeat -= OnDefeat;
        SC_UnitManager.OnWin -= OnWin;
    }

    #endregion

    #region Event Func
    private void OnWin()
    {
        announcementText.text = "YOU WON!";
        endGame();
    }

    private void OnDefeat()
    {
        announcementText.text = "YOU LOST!";
        endGame();
    }

    #endregion

    #region Logic
    private void endGame()
    {
        isGameEnd = true;
        gameEndWindow.SetActive(true);
        if(isMultiplayer)
        {
              restartButton.gameObject.SetActive(false);
        }
        else
        {
            restartButton.gameObject.SetActive(true);
        }

    }

    public void ResetTheGame()
    {
        OnRestart?.Invoke();
        gameEndWindow.SetActive(false);

    }

    public void ResetTheScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    
    public void StartGame()
    {
        menuWindow.SetActive(false);
        gameWindow.SetActive(true );
        if (isMultiplayer)
            OnMultiplayerGameStart?.Invoke(NextTurnPlayer);
    }

    public void setMultiplayer(bool b)
    {
        isMultiplayer = b;
    }

    public bool getIsMultiplayer()
    {
        return isMultiplayer;
    }

    public string getNextTurnPlayer()
    {
        return NextTurnPlayer;
    }

    public void setNextTurnPlayer(string n)
    {
        NextTurnPlayer = n;
    }
    
    public bool IsGameEnd()
    {
        return isGameEnd;
    }
    #endregion

}
