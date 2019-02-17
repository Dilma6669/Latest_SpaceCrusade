using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UnitsManager _instance;

    private UnitsAgent _unitsAgent;

    ////////////////////////////////////////////////

    public UnitBuilder _unitBuilder;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    NetWorkManager _networkManager;
    PlayerManager _playerManager;

    ////////////////////////////////////////////////

    public UnitsAgent UnitsAgent
    {
        get { return _unitsAgent; }
        set { _unitsAgent = value; }
    }

    ////////////////////////////////////////////////

    UnitScript _activeUnit = null;

    public List<GameObject> unitObjects = new List<GameObject>();
    public List<UnitScript> unitScripts = new List<UnitScript>();

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

        if (_unitBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _networkManager = _gameManager._networkManager;
        if (_networkManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void LoadPlayersUnits(Vector3 worldNodeLoc)
    {
        List<UnitData> units = _gameManager._playerManager.PlayerUnitData;

        foreach (UnitData unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldNodeLoc.x, localStart.y + worldNodeLoc.y, localStart.z + worldNodeLoc.z);

            CreateUnitOnNetwork(unit, worldStart);
        }
    }


    private void CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart)
    {
        int playerID = _gameManager._playerManager.PlayerID;
        _networkManager.NetworkAgent.CmdTellServerToSpawnPlayerUnit(unitData, playerID, worldStart);
    }


    public void SetUnitActive(bool onOff, UnitScript unit = null)
    {
        if (onOff)
        {
            if (_activeUnit)
            {
                _activeUnit.ActivateUnit(false);
            }
            //_gameManager._cubeManager.SetCubeActive (false);
            _activeUnit = unit;
            //_gameManager._locationManager.DebugTestPathFindingNodes(_activeUnit);
        }
        else
        {
            _activeUnit = null;
        }
    }

    public void MakeActiveUnitMove(Vector3 vectorToMoveTo)
    {
        if (_activeUnit)
        {
            _networkManager.NetworkAgent.CmdTellServerToMoveUnit(_playerManager.PlayerAgent.NetID, _activeUnit.NetID, vectorToMoveTo);
        }
    }

    public void MakeUnitRecalculateMove(UnitScript unit, Vector3 vectorToMoveTo)
    {
        Debug.Log("recalulating from unitsAgent");
        GetComponent<NetworkAgent>().CmdTellServerToMoveUnit(_playerManager.PlayerAgent.NetID, unit.NetID, vectorToMoveTo);
    }

    /*
    public void SetUpUnitForPlayer(GameObject unit)
    {
        Debug.Log("fucken unit 3: " + unit);
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.CubeUnitIsOn = _gameManager._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
        unitScript.PlayerControllerID = _playerManager.PlayerID;
        Debug.Log("fucken unitScript.PlayerControllerID 1: " + unitScript.PlayerControllerID);
    }
    */


    /*
    public void AssignUniqueLayerToUnits()
    {
        string layerStr = "Player0" + _gameManager._playerManager._playerAgent.PlayerUniqueID.ToString() + "Units";
        gameObject.layer = LayerMask.NameToLayer(layerStr);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerStr);
        }
    }
    */


}
