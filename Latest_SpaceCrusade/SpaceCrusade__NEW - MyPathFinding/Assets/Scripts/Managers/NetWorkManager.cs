using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : NetworkManager {

    public LocationManager _locationManager;
    public MovementManager _movementManager;
    public CombatManager _combatManager;

    public SyncedVars _syncedVars;

    void Awake() {

        _locationManager = GetComponentInChildren<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _movementManager = GetComponentInChildren<MovementManager>();
        if (_movementManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _combatManager = GetComponentInChildren<CombatManager>();
        if (_combatManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _syncedVars = GetComponentInChildren<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


	// called on the SERVER when a client connects
	public override void OnServerConnect(NetworkConnection Conn)
	{
        Debug.Log("NETWORKMANAGER: Client Connect!! Con: " + Conn.hostId);

        _syncedVars = FindObjectOfType<SyncedVars>();
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (Conn.hostId == -1)
        {
            int globalSeed = Random.Range(0, 100);
            Random.InitState(globalSeed);
            _syncedVars.GlobalSeed = globalSeed;
        }

        _syncedVars.PlayerCount = 1;
	}

}
