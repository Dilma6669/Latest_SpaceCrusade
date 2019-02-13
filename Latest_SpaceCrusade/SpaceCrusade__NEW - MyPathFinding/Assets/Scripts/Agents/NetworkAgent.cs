using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    GameManager _gameManager;

    SyncedVars _syncedvars;

    Dictionary<NetworkInstanceId, GameObject> network_Client_Objects;
    Dictionary<NetworkInstanceId, GameObject> network_Unit_Objects;


    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _syncedvars = _gameManager._networkManager._syncedVars;
        if (_syncedvars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        network_Client_Objects = new Dictionary<NetworkInstanceId, GameObject>();
        network_Unit_Objects = new Dictionary<NetworkInstanceId, GameObject>();
    }


    // Need this Start()
    void Start()
    {
    }


    [Command] //Commands - which are called from the client and run on the server;
    public void CmdAddPlayerToSession(NetworkInstanceId clientID)
    {
        network_Client_Objects.Add(clientID, ClientScene.FindLocalObject(clientID));
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
        NetworkServer.Spawn(unit);
        AssignUnitDataToUnitScript(unit, playerID, unitData);

        if (unit != null)
        {
            network_Unit_Objects.Add(unit.GetComponent<NetworkIdentity>().netId, unit);
            unit.transform.position = unitData.UnitStartingWorldLoc;
            _gameManager._locationManager.SetUnitOnCube(unit.GetComponent<UnitScript>(), unitData.UnitStartingWorldLoc);
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
        AssignUnitDataToUnitScript(unit, playerID, unitData);
        if (unit != null)
        {
            Debug.Log("Unit Succesfully created on CLIENT");
        }
        else
        {
            Debug.Log("Unit cannot be created on CLIENT");
        }
    }


    void AssignUnitDataToUnitScript(GameObject unit, int playerID, UnitData unitData)
    {
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.UnitData = unitData;
        unitScript.NetID = unit.GetComponent<NetworkIdentity>().netId;
        unitScript.PlayerControllerID = playerID;
        unitScript.UnitModel = unitData.UnitModel;
        unitScript.UnitCanClimbWalls = unitData.UnitCanClimbWalls;
        unitScript.UnitStartingWorldLoc = unitData.UnitStartingWorldLoc;
        unitScript._unitCombatStats = unitData.UnitCombatStats;
        unit.transform.SetParent(_gameManager._unitsManager.gameObject.transform);
    }


    // Server Move Unit 
    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToMoveUnit(NetworkInstanceId clientNetID, NetworkInstanceId unitNetID, Vector3 vectorToMoveTo)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        GameObject unit = network_Unit_Objects[unitNetID];
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        int[] movePath = _gameManager._movementManager.SetUnitsPath(unit, unitScript.CubeUnitIsOn.CubeLocVector, vectorToMoveTo);
        NetworkConnection clientID = network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
        int unitID = (int)network_Unit_Objects[unitNetID].GetComponent<NetworkIdentity>().netId.Value;
        TargetSendPathVectorsToClient(clientID, network_Unit_Objects[unitNetID], unitID, movePath);
    }

    [TargetRpc] //ClientRpc calls - which are called on the server and run on clients 
    void TargetSendPathVectorsToClient(NetworkConnection clientID, GameObject unit, int unitID, int[] pathVects)
    {
        //Debug.Log("Creating pathFinding NOdes on client: " + clientID);
       _gameManager._movementManager.CreatePathFindingNodes(unit, unitID, pathVects);
    }


    // Server assign cube Neighbours
    /*[Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToAssignCubeNeighbours(Vector3 cubeVec)
    {
        if (!isServer) return;

        CubeLocationScript cube = _gameManager._locationManager.GetLocationScript(cubeVec);
        cube.AssignCubeNeighbours();
    }
    */
}
