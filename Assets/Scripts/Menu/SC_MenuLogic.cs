using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_MenuLogic : MonoBehaviour
{
    #region AppWarpKeys
    private string apiKey = "96b287484e258da542f70b57dae04c2aa50c40e3b93828596650ac1561ea8b7d";
    private string pvtKey = "ea65f2ffc2c7cf1d959118354e5cd0924d48500be460cb74ba1154e2c37de89d";
    #endregion

    #region Variables
    [SerializeField] private TextMeshProUGUI MutiplayerText;

    private Dictionary<string, object> passedParams;
    private Dictionary<string, GameObject> unityObjects;
    private List<string> roomIds;
    private static int roomNumber = 0;
    private int maxRoomUsers = 2;
    private string roomName = "TestRoom";
    private string roomId;
    private int roomIndex = 0;
    private bool isConnected = false;
    private bool debugStatus = true;


    #endregion

    #region Listeners
    private Listener listner;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        InitAwake();
    }

    private void Start()
    {
        InitStart();
    }

   
    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;


    }

    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;


    }

    



    #endregion

    #region Logic
    private void InitAwake()
    {
        updateUnityObjects();

        passedParams = new Dictionary<string, object>()
        {
            { "Password","Shenkar2023" }
        };

        if (listner == null)
            listner = new Listener();



        WarpClient.initialize(apiKey, pvtKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listner);
        WarpClient.GetInstance().AddChatRequestListener(listner);
        WarpClient.GetInstance().AddUpdateRequestListener(listner);
        WarpClient.GetInstance().AddLobbyRequestListener(listner);
        WarpClient.GetInstance().AddNotificationListener(listner);
        WarpClient.GetInstance().AddRoomRequestListener(listner);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listner);
        WarpClient.GetInstance().AddZoneRequestListener(listner);


        string baseName = "User";
        string uniqueIdentifier = System.Guid.NewGuid().ToString().Substring(0, 8); // Get the first 8 characters of the GUID
        SC_GlobalVariables.Instance.setUserId($"{baseName}_{uniqueIdentifier}") ;
       
    }

    private void InitStart()
    {
        unityObjects["Screen_MainMenu_User_Text"].GetComponent<TextMeshProUGUI>().text = SC_GlobalVariables.Instance.getUserId();

        WarpClient.GetInstance().Connect(SC_GlobalVariables.Instance.getUserId());
    }


    private void UpdateStatus(string status)
    {
        if (debugStatus)
            Debug.Log(status);
    }

    private void updateUnityObjects()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _obj = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _obj)
            unityObjects.Add(g.name, g);
    }

    private void DoRoomSearchLogic()
    {
        if (roomIndex < roomIds.Count)
        {
            UpdateStatus("Bring room info" + roomIds[roomIndex]);
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIndex]);
        }
        else
        {
            UpdateStatus("Creating room");
            WarpClient.GetInstance().CreateTurnRoom(roomName + roomNumber, SC_GlobalVariables.Instance.getUserId(), maxRoomUsers, passedParams, SC_GlobalVariables.Instance.getTurnTime());
            roomNumber++;
        }
    }
     

    #endregion

    #region Server Callsbacks
    private void OnConnect(bool _IsSuccess)
    {
       UpdateStatus("OnConnect " + _IsSuccess);
        if (_IsSuccess)
            isConnected = true;
        else
            isConnected = false;
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log(_IsSuccess);
        if(_IsSuccess )
        {
            UpdateStatus("Parsing Rooms");
            roomIds = new List<string>();
            foreach(var RoomData in  eventObj.getRoomsData())
            {
                roomIds.Add(RoomData.getId());
            }

            roomIndex = 0;
            DoRoomSearchLogic();
        }
    }

    private void OnCreateRoom (bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess )
        {
            roomId = _RoomId;
            UpdateStatus("Room: " + _RoomId + " Created");
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else
        {
            WarpClient.GetInstance().GetRoomsInRange(1, 2);
        }
    }

    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
        {
            UpdateStatus("Joined Room:" + _RoomId);
            updateUnityObjects();
            unityObjects["Screen_Loading_RoomId_Txt"].GetComponent<TextMeshProUGUI>().text = "Room: " + _RoomId;
        }
        else
        {
            UpdateStatus("Failed to join Room: " + _RoomId);
            WarpClient.GetInstance().GetRoomsInRange(1, 2);

        }
    }


    private void OnUserJoinRoom(RoomData eventObj, string _UserName)
    {
        UpdateStatus("User Joined Room " + _UserName);
        if (eventObj.getRoomOwner() == SC_GlobalVariables.Instance.getUserId() && SC_GlobalVariables.Instance.getUserId() != _UserName)
        {
            UpdateStatus("Starting Game");
            WarpClient.GetInstance().startGame();
        }
    }


    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        if (eventObj != null &&  eventObj.getProperties() != null)
        {
            Dictionary<string,object> _properties = eventObj.getProperties();
            if( _properties.ContainsKey("Password") && (passedParams["Password"].ToString() == _properties["Password"].ToString()))
            {
                roomId = eventObj.getData().getId();
                UpdateStatus("Recived Room Info, joining room " + roomId);
                WarpClient.GetInstance().JoinRoom(roomId);
                WarpClient.GetInstance().SubscribeRoom(roomId);
            }
            else
            {
                roomIndex++;
                DoRoomSearchLogic();
            }
        }
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        UpdateStatus("Game Started, Next Turn: " + _NextTurn);
        SC_GameManager.Instance.setMultiplayer(true);
        SC_GameManager.Instance.setNextTurnPlayer(_NextTurn);

        SC_GameManager.Instance.StartGame();

        if (_NextTurn == SC_GlobalVariables.Instance.getUserId())
        {
            //SC_GameManager.Instance.SetupUnitsAsAlly();
        }
        else
        {
            //SC_GameManager.Instance.SetupUnitsAsEnemy();
        }
    }




    #endregion

    #region Controller
    public void Btn_PlayLogic()
    {
        if (isConnected)
        {
            WarpClient.GetInstance().GetRoomsInRange(1, 2);
            MutiplayerText.text = "Waiting for players";
            UpdateStatus("Searching room");
        }
        else
        {
            MutiplayerText.text = "Connection Failed";
            UpdateStatus("Could not connect to server");
        }
    }

    #endregion

}
