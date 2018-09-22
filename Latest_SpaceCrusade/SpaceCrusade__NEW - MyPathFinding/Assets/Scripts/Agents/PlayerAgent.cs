using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour {


    GameManager _gameManager;

    PlayerManager _playerManager;
    UIManager _uiManager;
    LocationManager _locationManager;

    SyncedVars _syncedVars;

	CameraAgent _cameraAgent;
    //
    //	[HideInInspector]
    //	public UnitsAgent _unitsAgent;

    BasePlayerData _playerData;

    public int _playerUniqueID = 0;
    public string _playerName = "???";
    public int _totalPlayers = -1;
    public int _seed = -1;

    Text playerIDText;
    Text playerNameText;
    Text totalPlayerText;
    Text seedNumText;


    // Use this for initialization
    void Awake () {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _playerManager = _gameManager._playerManager;
		if(_playerManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _uiManager = _gameManager._uiManager;
        if (_uiManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _locationManager = _gameManager._locationManager;
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _cameraAgent = GetComponent<CameraAgent> ();
		if(_cameraAgent == null){Debug.LogError ("OOPSALA we have an ERROR!");}



        //unitsAgent = _gameManager._playerManager;
        //if(_unitsAgent == null){Debug.LogError ("OOPSALA we have an ERROR!");}



        playerIDText = _uiManager.transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        playerNameText = _uiManager.transform.FindDeepChild("PlayerName").GetComponent<Text>();
        totalPlayerText = _uiManager.transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
        seedNumText = _uiManager.transform.FindDeepChild("SeedNum").GetComponent<Text>();

    }

    // Need this Start()
    void Start()
    {
        Debug.Log("A network Player object has been created");
        CreatePlayerAgent();
    }

    // DONT FUCKING TOUCH THIS FUNCTION
    void CreatePlayerAgent() {

        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        transform.SetParent (_playerManager.transform);
		_uiManager.GetComponent<Canvas>().enabled = true;

        _seed = _syncedVars.GlobalSeed;
        seedNumText.text = _seed.ToString();
        Random.InitState(_seed);

        _syncedVars.CmdTellServerToUpdatePlayerCount();

        if (isLocalPlayer)
        {
            _playerUniqueID = _syncedVars.PlayerCount;
            playerIDText.text = _playerUniqueID.ToString();
            _playerData = _playerManager.GetPlayerData(_playerUniqueID);
            playerNameText.text = _playerData.name;
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
	    _cameraAgent.SetUpCameraAndLayers (_playerUniqueID);

        _locationManager.BuildMapForClient();

    }
	
}
