using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static PlayerManager _instance;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    SyncedVars _syncedVars;

    ////////////////////////////////////////////////

    private PlayerAgent _playerAgent;
    private BasePlayerData _playerData;

    ////////////////////////////////////////////////

    public GameObject _playerMotherShip;

    ////////////////////////////////////////////////

    int _playerID = 0;
    string _playerName = "???";
    int _totalPlayers = -1;
    int _seed = -1;

    ////////////////////////////////////////////////

    public PlayerAgent PlayerAgent
    {
        get { return _playerAgent; }
        set { _playerAgent = value; }
    }

    public BasePlayerData PlayerData
    {
        get { return _playerData; }
        set { _playerData = value; }
    }

    public int PlayerID
    {
        get { return _playerID; }
        set { _playerID = value; }
    }

    public int TotalPlayers
    {
        get { return _totalPlayers; }
        set { _totalPlayers = value; }
    }

    public string PlayerName
    {
        get { return _playerData.name; }
    }

    public List<int[,]> PlayerShipSmallFloorDataPART1
    {
        get { return _playerData.smallShipFloorsPART1; }
    }

    public List<int[,]> PlayerShipSmallFloorDataPART2
    {
        get { return _playerData.smallShipFloorsPART2; }
    }

    public List<int[,]> PlayerShipSmallVentDataPART1
    {
        get { return _playerData.smallShipVentsPART1; }
    }

    public List<int[,]> PlayerShipSmallVentDataPART2
    {
        get { return _playerData.smallShipVentsPART2; }
    }

    public List<UnitData> PlayerUnitData
    {
        get { return _playerData.allUnitData; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        if (_playerMotherShip == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _syncedVars = _gameManager._networkManager._syncedVars;
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void SetUpPlayer()
    {
        PlayerID = _syncedVars.PlayerCount;
        LoadPlayerDataInToManager(PlayerID);
    }


    public void LoadPlayerDataInToManager(int playerID)
    {
        BasePlayerData data = null;

        switch (playerID)
        {
            case 0:
                data = new PlayerData_00();
                break;
            case 1:
                data = new PlayerData_01();
                break;
            case 2:
                data = new PlayerData_02();
                break;
            case 3:
                data = new PlayerData_03();
                break;
            default:
                Debug.Log("SOMETHING WENT WRONG HERE: playerID: " + playerID);
                break;
        }
        PlayerData = data;
    }

    public void LoadPlayersShip(Vector3 loc, Vector3 rot) // Dont like this here
    {
        GameObject ship = Instantiate(_playerMotherShip, this.gameObject.transform, false);
        ship.transform.position = loc;
        ship.transform.localEulerAngles = rot;
        ship.transform.SetParent(this.gameObject.transform);
    }
}
