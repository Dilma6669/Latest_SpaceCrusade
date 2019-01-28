using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour
{
    GameManager _gameManager;
    PlayerManager _playerManager;
    UIManager _uiManager;
    LocationManager _locationManager;

    SyncedVars _syncedVars;

    int _playerID = 0;
    NetworkInstanceId _netID;
    string _playerName = "???";
    int _totalPlayers = -1;
    int _seed = -1;


    public NetworkInstanceId NetID
    {
        get { return _netID; }
        set { _netID = value; }
    }

    public int PlayerID
    {
        get { return _playerID; }
        set { _playerID = value; }
    }

    public string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }

    public int TotalPlayers
    {
        get { return _totalPlayers; }
        set { _totalPlayers = value; }
    }

    public int GlobalSeed
    {
        get { return _seed; }
        set { _seed = value; }
    }

    Text playerIDText;
    Text playerNameText;
    Text totalPlayerText;
    Text seedNumText;


    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _uiManager = _gameManager._uiManager;
        if (_uiManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        playerIDText = _uiManager.transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        playerNameText = _uiManager.transform.FindDeepChild("PlayerName").GetComponent<Text>();
        totalPlayerText = _uiManager.transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
        seedNumText = _uiManager.transform.FindDeepChild("SeedNum").GetComponent<Text>();

    }

    // Need this Start()
    void Start()
    {
    }

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) return;
        Debug.Log("A network Player object has been created");

        _playerManager._playerObject = this.gameObject;

        transform.SetParent(_playerManager.transform);

        GlobalSeed = _syncedVars.GlobalSeed;
        Random.InitState(GlobalSeed);

        NetID = GetComponent<NetworkIdentity>().netId;

        PlayerID = _syncedVars.PlayerCount;
        _playerManager.LoadPlayerDataInToManager(PlayerID);
        GetComponent<NetworkAgent>().CmdAddPlayerToSession(NetID);

        SetUpPlayersGUI();

        GetComponent<CameraAgent>().SetUpCameraAndLayers(PlayerID);

        _playerManager.LoadPlayersShip(this.gameObject.transform.position, this.gameObject.transform.localEulerAngles);

        _gameManager._worldManager.BuildMapForClient();
    }

    // The players personal GUI
    void SetUpPlayersGUI()
    {
        if (!isLocalPlayer) return;

        _uiManager.GetComponent<Canvas>().enabled = true;

        playerIDText.text = PlayerID.ToString();
        playerNameText.text = _playerManager.GetPlayerName();
        seedNumText.text = GlobalSeed.ToString();
    }



    public void UpdatePlayerCount(int count)
    {
        //Debug.Log("fucken UpdatePlayerCount: " + count);
        TotalPlayers = count;
        totalPlayerText.text = TotalPlayers.ToString();
    }


}
