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
}
