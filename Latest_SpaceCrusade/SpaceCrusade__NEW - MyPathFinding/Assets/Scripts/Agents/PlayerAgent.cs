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

    int _playerUniqueID = 0;
    public string _playerName = "???";
    public int _totalPlayers = -1;
    public int _seed = -1;

    public int PlayerUniqueID
    {
        get { return _playerUniqueID; }
        set { _playerUniqueID = value; }
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

        playerIDText = _uiManager.transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        playerNameText = _uiManager.transform.FindDeepChild("PlayerName").GetComponent<Text>();
        totalPlayerText = _uiManager.transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
        seedNumText = _uiManager.transform.FindDeepChild("SeedNum").GetComponent<Text>();

    }

    // Need this Start()
    void Start()
    {
        Debug.Log("A network Player object has been created");
        _gameManager._playerManager._playerAgent = this;
        CreatePlayerAgent();
    }

    // DONT FUCKING TOUCH THIS FUNCTION
    void CreatePlayerAgent()
    {
        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        transform.SetParent(_playerManager.transform);
        _uiManager.GetComponent<Canvas>().enabled = true;

        _seed = _syncedVars.GlobalSeed;
        seedNumText.text = _seed.ToString();
        Random.InitState(_seed);

        GetComponent<NetworkAgent>().CmdTellServerToUpdatePlayerCount();

        if (isLocalPlayer)
        {
            PlayerUniqueID = _syncedVars.PlayerCount;
            playerIDText.text = PlayerUniqueID.ToString();
            _playerManager.LoadPlayerDataInToManager(PlayerUniqueID);
            playerNameText.text = _playerManager.GetPlayerName();
            ContinuePlayerSetUp();
        }
    }

    public void UpdatePlayerCount(int count)
    {
        _totalPlayers = count;
        totalPlayerText.text = _totalPlayers.ToString();
    }



    void ContinuePlayerSetUp()
    {
        _playerManager._cameraAgent.SetUpCameraAndLayers(PlayerUniqueID);

        _gameManager._locationManager.BuildMapForClient();

    }

}
