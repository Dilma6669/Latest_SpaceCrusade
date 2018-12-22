using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    GameManager _gameManager;

    SyncedVars _syncedvars;

    Dictionary<NetworkInstanceId, GameObject> network_Unit_Objects;


    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _syncedvars = _gameManager._networkManager._syncedVars;
        if (_syncedvars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        network_Unit_Objects = new Dictionary<NetworkInstanceId, GameObject>();
    }


    // Need this Start()
    void Start()
    {
    }


    [Command] //Commands - which are called from the client and run on the server;
    public void CmdTellServerToUpdatePlayerCount()
    {
        RpcUpdatePlayerCountOnClient(_syncedvars.PlayerCount + 1);
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcUpdatePlayerCountOnClient(int count)
    {
        GetComponent<PlayerAgent>().UpdatePlayerCount(count);
    }


    /////////////////////////////////////////////////////

    /////////////////////////////////////////////////////

    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToSpawnPlayerUnit(UnitData unitData, int playerID, Vector3 worldStart)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        unitData.UnitStartingWorldLoc = worldStart;

        GameObject prefab = _gameManager._unitsManager._unitBuilder.GetUnitModel(unitData.UnitModel);
        GameObject unit = Instantiate(prefab, _gameManager._unitsManager.gameObject.transform, false);
        GameObject fullUnit = AssignUnitDataToUnitScript(unit, playerID, unitData);

        NetworkServer.Spawn(fullUnit);

        if (fullUnit != null)
        {
            network_Unit_Objects.Add(unit.GetComponent<NetworkIdentity>().netId, fullUnit);
            fullUnit.transform.position = unitData.UnitStartingWorldLoc;
            NetworkInstanceId unitNetID = unit.GetComponent<NetworkIdentity>().netId;
            RpcUpdatePlayerUnitsOnAllClients(unit, unitNetID, playerID, unitData);
        }
        else
        {
            Debug.LogError("Unit cannot be created on SERVER");
        }
    }


    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcUpdatePlayerUnitsOnAllClients(GameObject unit, NetworkInstanceId netID, int playerID, UnitData unitData)
    {
        GameObject fullUnit = AssignUnitDataToUnitScript(unit, playerID, unitData);

        if (unit != null)
        {
            Debug.Log("Unit SUCCESSFULLY created on CLIENT netID: " + netID);
        }
        else
        {
            Debug.LogError("Unit cannot be created on CLIENT");
        }
    }


    GameObject AssignUnitDataToUnitScript(GameObject unit, int playerID, UnitData unitData)
    {
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.UnitData = unitData;
        unitScript.NetID = unit.GetComponent<NetworkIdentity>().netId;
        unitScript.PlayerControllerID = playerID;
        unitScript.UnitModel = unitData.UnitModel;
        unitScript.UnitCanClimbWalls = unitData.UnitCanClimbWalls;
        Debug.Log("AFTER unitData[1]: " + unitData.UnitCanClimbWalls);
        unitScript.UnitStartingWorldLoc = unitData.UnitStartingWorldLoc;
        unitScript.UnitCombatStats = unitData.UnitCombatStats;
        unitScript.CubeUnitIsOn = _gameManager._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
        unit.transform.SetParent(_gameManager._unitsManager.gameObject.transform);

        return unit;
    }


    // Server Move Unit
    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToMoveUnit(NetworkInstanceId unitNetworkID, Vector3 vectorToMoveTo, Vector3 offsetPosToMoveTo)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        GameObject unit = network_Unit_Objects[unitNetworkID];
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        _gameManager._gamePlayManager._movementManager.SetUnitsPath(unit, unitScript.UnitCanClimbWalls, unitScript.CubeUnitIsOn.cubeLoc, vectorToMoveTo, offsetPosToMoveTo);
    }

}
