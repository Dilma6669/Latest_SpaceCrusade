﻿using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    GameManager _gameManager;

    SyncedVars _syncedvars;


    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _syncedvars = _gameManager._networkManager._syncedVars;
        if (_syncedvars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

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



    public void TellServerToSpawnPlayerUnit(int unitModel, bool unitCanClimbwalls, Vector3 startLocation, int[] unitCombatStats)
    {
        if (!isLocalPlayer) return;

        Debug.Log("BEFORE <<<<<<<<<<<<<<<<<, ");
        CmdTellServerToSpawnPlayerUnit(unitModel, unitCanClimbwalls, startLocation, unitCombatStats);
    }


    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToSpawnPlayerUnit(int unitModel, bool unitCanClimbwalls, Vector3 startLocation, int[] unitCombatStats)
    {
        if (!isServer) return;

        Debug.Log("fucken unitScript 1 server: ");

        GameObject prefab = _gameManager._unitsManager._unitBuilder.GetUnitModel(unitModel);
        GameObject unit = Instantiate(prefab, _gameManager._unitsManager.gameObject.transform, false);
        NetworkServer.Spawn(unit);
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.UnitScriptConstructor(unitModel, unitCanClimbwalls, unitCombatStats, startLocation);
        unit.transform.SetParent(_gameManager._unitsManager.gameObject.transform);
        unit.transform.position = startLocation;

        GetComponent<UnitsAgent>().SetUpUnitForPlayer(unit);

        RpcSpawnPlayerUnitsOnClient(unitModel, unitCanClimbwalls, startLocation, unitCombatStats);
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcSpawnPlayerUnitsOnClient(int unitModel, bool unitCanClimbwalls, Vector3 startLocation, int[] unitCombatStats)
    {
        Debug.Log("AFTER <<<<<<<<<<<<<<<<<<<<<< ");
    }

}
