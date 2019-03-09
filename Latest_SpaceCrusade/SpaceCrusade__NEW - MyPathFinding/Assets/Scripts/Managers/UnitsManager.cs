using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UnitsManager _instance;

    private static UnitsAgent _unitsAgent;

    ////////////////////////////////////////////////

    public static UnitBuilder _unitBuilder;

    ////////////////////////////////////////////////

    public static UnitsAgent UnitsAgent
    {
        get { return _unitsAgent; }
        set { _unitsAgent = value; }
    }

    ////////////////////////////////////////////////

    private static UnitScript _activeUnit = null;

    public static List<GameObject> unitObjects = new List<GameObject>();
    public static List<UnitScript> unitScripts = new List<UnitScript>();

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
    }

    void Start()
    {
        _unitBuilder = GameObject.Find("UnitBuilder").GetComponent<UnitBuilder>();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void LoadPlayersUnits(Vector3 worldNodeLoc)
    {
        List<UnitData> units = PlayerManager.PlayerUnitData;

        foreach (UnitData unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldNodeLoc.x, localStart.y + worldNodeLoc.y, localStart.z + worldNodeLoc.z);

            CreateUnitOnNetwork(unit, worldStart);
        }
    }


    private static void CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart)
    {
        int playerID = PlayerManager.PlayerID;
        NetWorkManager.NetworkAgent.CmdTellServerToSpawnPlayerUnit(PlayerManager.PlayerAgent.NetID, unitData, playerID, worldStart);
    }


    public static void SetUnitActive(bool onOff, UnitScript unit = null)
    {
        if (onOff)
        {
            if (_activeUnit)
            {
                _activeUnit.ActivateUnit(false);
            }
            // ._cubeManager.SetCubeActive (false);
            CameraManager.SetCamToOrbitUnit(unit.transform);
            LayerManager.ChangeCameraLayer(unit.CubeUnitIsOn);
            _activeUnit = unit;
            // ._locationManager.DebugTestPathFindingNodes(_activeUnit);
        }
        else
        {
            _activeUnit = null;
        }
    }

    public static void MakeActiveUnitMove(Vector3 vectorToMoveTo)
    {
        if (_activeUnit)
        {
            NetWorkManager.NetworkAgent.CmdTellServerToMoveUnit(PlayerManager.PlayerAgent.NetID, _activeUnit.NetID, vectorToMoveTo);
        }
    }

    public static void MakeUnitRecalculateMove(UnitScript unit, Vector3 vectorToMoveTo)
    {
        Debug.Log("recalulating from unitsAgent");
        NetWorkManager.NetworkAgent.CmdTellServerToMoveUnit(PlayerManager.PlayerAgent.NetID, unit.NetID, vectorToMoveTo);
    }

    /*
    public static void SetUpUnitForPlayer(GameObject unit)
    {
        Debug.Log("fucken unit 3: " + unit);
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.CubeUnitIsOn =  ._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
        unitScript.PlayerControllerID = _playerManager.PlayerID;
        Debug.Log("fucken unitScript.PlayerControllerID 1: " + unitScript.PlayerControllerID);
    }
    */


    /*
    public static void AssignUniqueLayerToUnits()
    {
        string layerStr = "Player0" +  ._playerManager._playerAgent.PlayerUniqueID.ToString() + "Units";
        gameObject.layer = LayerMask.NameToLayer(layerStr);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerStr);
        }
    }
    */


}
