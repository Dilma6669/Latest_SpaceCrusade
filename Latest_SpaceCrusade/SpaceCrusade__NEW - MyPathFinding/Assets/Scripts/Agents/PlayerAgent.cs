using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour
{
    ////////////////////////////////////////////////

    GameManager _gameManager;
    PlayerManager _playerManager;
    UIManager _uiManager;
    CameraManager _cameraManager;
    
    ////////////////////////////////////////////////

    NetworkInstanceId _netID;
    int _totalPlayers = -1;

    ////////////////////////////////////////////////

    public NetworkInstanceId NetID
    {
        get { return _netID; }
        set { _netID = value; }
    }

    public int TotalPlayers
    {
        get { return _totalPlayers; }
        set { _totalPlayers = value; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // Use this for initialization
    void Awake()
    {
    }

    // Need this Start()
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cameraManager = _gameManager._cameraManager;
        if (_cameraManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _uiManager = _gameManager._uiManager;
        if (_uiManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (!isLocalPlayer) return;
        _playerManager.PlayerAgent = this;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public override void OnStartLocalPlayer()
    {
        Start();
        if (!isLocalPlayer) return;
        Debug.Log("A network Player object has been created");

        transform.SetParent(_playerManager.transform);
        _playerManager.SetUpPlayer();

        NetID = GetComponent<NetworkIdentity>().netId;
        GetComponent<NetworkAgent>().CmdAddPlayerToSession(NetID);

        _uiManager.SetUpPlayersGUI(_playerManager.PlayerID);
        _cameraManager.SetUpCameraAndLayers(_playerManager.PlayerID, GetComponent<CameraAgent>());

        _playerManager.LoadPlayersShip(transform.position, transform.localEulerAngles);

        _gameManager._worldManager.BuildMapForClient();
    }


    public void UpdatePlayerCount(int count)
    {
        TotalPlayers = count;
        _uiManager.UpdateTotalPlayersGUI(count);
    }

    public void SetUpPlayerStartPosition(Vector3 camPos, Quaternion camRot)
    {
        transform.position = camPos;
        transform.rotation = camRot;
    }


}
