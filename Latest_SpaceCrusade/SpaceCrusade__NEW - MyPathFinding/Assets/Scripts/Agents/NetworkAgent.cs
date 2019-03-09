using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    /////////////////////////////////////////////////////

    Dictionary<NetworkInstanceId, GameObject> network_Client_Objects;
    Dictionary<NetworkInstanceId, GameObject> network_Unit_Objects;

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////

    // Use this for initialization
    void Awake()
    {
        network_Client_Objects = new Dictionary<NetworkInstanceId, GameObject>();
        network_Unit_Objects = new Dictionary<NetworkInstanceId, GameObject>();
    }

    // Need this Start()
    void Start()
    {

        if (!isLocalPlayer) return;
        NetWorkManager.NetworkAgent = this;
    }

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////

    [Command] //Commands - which are called from the client and run on the server;
    public void CmdAddPlayerToSession(NetworkInstanceId clientID)
    {
        Start();
        network_Client_Objects.Add(clientID, ClientScene.FindLocalObject(clientID));

        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake

        RpcUpdatePlayerCountOnClient(_syncedVars.PlayerCount + 1);
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcUpdatePlayerCountOnClient(int count)
    {
        GetComponent<PlayerAgent>().UpdatePlayerCount(count);
    }

    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToSpawnPlayerUnit(NetworkInstanceId clientNetID, UnitData unitData, int playerID, Vector3 worldStart)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        unitData.UnitStartingWorldLoc = worldStart;

        GameObject prefab = UnitsManager._unitBuilder.GetUnitModel(unitData.UnitModel);
        GameObject unit = Instantiate(prefab, GameManager._UnitsManager.transform, false);
        NetworkServer.Spawn(unit);
        AssignUnitDataToUnitScript(unit, playerID, unitData);

        if (unit != null)
        {
            network_Unit_Objects.Add(unit.GetComponent<NetworkIdentity>().netId, unit);
            unit.transform.position = unitData.UnitStartingWorldLoc;
            LocationManager.SetUnitOnCube(unit.GetComponent<UnitScript>(), unitData.UnitStartingWorldLoc);
            NetworkInstanceId unitNetID = unit.GetComponent<NetworkIdentity>().netId;
            RpcUpdatePlayerUnitsOnAllClients(unit, unitNetID, playerID, unitData);

            NetworkConnection clientID = network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
            TargetActivateUnitLeader(clientID, unit);
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

    [TargetRpc] //ClientRpc calls - which are called on the server and run on clients 
    void TargetActivateUnitLeader(NetworkConnection clientID, GameObject unit)
    {
        if (isLocalPlayer)
        {
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            unitScript.ActivateUnit(true);
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
        unitScript.UnitCombatStats = unitData.UnitCombatStats;
        unit.transform.SetParent(GameManager._UnitsManager.transform);
    }


    // Server Move Unit 
    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToMoveUnit(NetworkInstanceId clientNetID, NetworkInstanceId unitNetID, Vector3 vectorToMoveTo)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        GameObject unit = network_Unit_Objects[unitNetID];
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        int[] movePath = MovementManager.SetUnitsPath(unit, unitScript.CubeUnitIsOn.CubeLocVector, vectorToMoveTo);
        NetworkConnection clientID = network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
        int unitID = (int)network_Unit_Objects[unitNetID].GetComponent<NetworkIdentity>().netId.Value;
        TargetSendPathVectorsToClient(clientID, network_Unit_Objects[unitNetID], unitID, movePath);
    }

    [TargetRpc] //ClientRpc calls - which are called on the server and run on clients 
    void TargetSendPathVectorsToClient(NetworkConnection clientID, GameObject unit, int unitID, int[] pathVects)
    {
        //Debug.Log("Creating pathFinding NOdes on client: " + clientID);
        MovementManager.CreatePathFindingNodes(unit, unitID, pathVects);
    }


    // Server assign cube Neighbours
    /*[Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToAssignCubeNeighbours(Vector3 cubeVec)
    {
        if (!isServer) return;

        CubeLocationScript cube =  .LocationManager.GetLocationScript(cubeVec);
        cube.AssignCubeNeighbours();
    }
    */
}
