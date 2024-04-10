
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class SC_MenuController : MonoBehaviour
{
    #region Singleton
    public static SC_MenuController Instance { get; private set; }

    #endregion

    #region Variables
    public enum MenuScreen { MainMenu, Options, Multiplayer, StudentInfo, Loading }
    private Stack<MenuScreen> navigationStack;
    [System.Serializable]
        public struct ScreenMapping
        {
            public MenuScreen screen;
            public GameObject screenObject;
        }

    public ScreenMapping[] screenMappings;

    #endregion

    #region MonoBehaviour
    void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            navigationStack = new Stack<MenuScreen>();
            navigationStack.Push(MenuScreen.MainMenu); 

        }
        else
            Destroy(gameObject); 
        
    }
    #endregion

    #region Logic
    public void GoToScreen(int screenIndex)
    {
        if (screenIndex < 0 || screenIndex >= screenMappings.Length) return;

        if (navigationStack.Count > 0)
            DeactivateCurrentScreen();

        if (screenIndex == (int)MenuScreen.Loading && (navigationStack.Peek() == MenuScreen.MainMenu))
        {
            SC_GameManager.Instance.StartGame();
        }

        navigationStack.Push(screenMappings[screenIndex].screen);
        ActivateCurrentScreen();

        


    }
    public void GoBack()
    {
        if (navigationStack.Count > 1)
        {
            DeactivateCurrentScreen();
            navigationStack.Pop();
            ActivateCurrentScreen();
        }
    }

    private void DeactivateCurrentScreen()
    {
        MenuScreen currentScreen = navigationStack.Peek();
        GetScreenObject(currentScreen).SetActive(false);
    }

    private void ActivateCurrentScreen()
    {
        MenuScreen currentScreen = navigationStack.Peek();
        GetScreenObject(currentScreen).SetActive(true);
    }

    private GameObject GetScreenObject(MenuScreen screen)
    {
        foreach (var mapping in screenMappings)
        {
            if (mapping.screen == screen)
                return mapping.screenObject;
        }
        return null;
    }

    #endregion


}
