using UnityEngine;
using UnityEngine.Networking;

public class SyncedVars : NetworkBehaviour {

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



	[Command] //Commands - which are called from the client and run on the server;
	public void CmdTellServerToUpdatePlayerCount() {
		RpcUpdatePlayerCountOnClient ();
	}
	[ClientRpc] //ClientRpc calls - which are called on the server and run on clients
	void RpcUpdatePlayerCountOnClient() {
		PlayerAgent _playerAgent = FindObjectOfType<PlayerAgent> ();
        _playerAgent.UpdatePlayerCount (PlayerCount + 1);
    }
}
