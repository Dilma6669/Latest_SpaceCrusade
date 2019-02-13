using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : NetworkManager
{
    ////////////////////////////////////////////////

    private static NetWorkManager _instance;

    ////////////////////////////////////////////////

    public SyncedVars _syncedVars;

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

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

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
