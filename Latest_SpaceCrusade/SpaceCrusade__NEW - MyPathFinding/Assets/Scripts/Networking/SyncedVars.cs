using UnityEngine;
using UnityEngine.Networking;

public class SyncedVars : NetworkBehaviour {

    GameManager _gameManager;

    [SyncVar]
	public int globalSeed = -1;

	[SyncVar]
	public int playerCount = -1;


	public int GlobalSeed
	{
		get { return globalSeed; }
		set { globalSeed = value; }
	}

	public int PlayerCount
	{
		get { return playerCount; }
		set { playerCount = playerCount + value; }
	}

    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }



    [Command] //Commands - which are called from the client and run on the server;
	public void CmdTellServerToUpdatePlayerCount() {
		RpcUpdatePlayerCountOnClient ();
	}
	[ClientRpc] //ClientRpc calls - which are called on the server and run on clients
	void RpcUpdatePlayerCountOnClient() {
		PlayerAgent _playerAgent = FindObjectOfType<PlayerAgent> ();
        _playerAgent.UpdatePlayerCount (PlayerCount + 1);
    }

    [Command] //ClientRpc calls - which are called on the server and run on clients
    public void CmdTellServerToSpawnPlayerUnits(GameObject _unitPrefab, Vector3 startingLoc)
    {
        var unit = Instantiate(_unitPrefab, _gameManager._unitsManager.gameObject.transform, false);
        NetworkServer.Spawn(unit);
        Debug.Log("fucken unit 1: " + unit);
        unit.transform.SetParent(_gameManager._unitsManager.gameObject.transform);
        UnitScript unitscript = unit.gameObject.GetComponent<UnitScript>();
        unit.transform.position = startingLoc;

        RpcSpawnPlayerUnitsOnClient(unit);
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcSpawnPlayerUnitsOnClient(GameObject unit)
    {
        Debug.Log("fucken unit 2: " + unit);
        _gameManager._playerManager._unitsAgent.SetUpUnitForPlayer(unit);
    }
}
