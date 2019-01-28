using UnityEngine;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public WorldManager _worldManager;
    [HideInInspector]
    public PlayerManager _playerManager;
    [HideInInspector]
    public CameraManager _cameraManager;
    [HideInInspector]
    public UIManager _uiManager;
    [HideInInspector]
    public LocationManager _locationManager;
    [HideInInspector]
    public NetWorkManager _networkManager;
    [HideInInspector]
    public UnitsManager _unitsManager;



    void Awake() {

        _worldManager = GetComponentInChildren<WorldManager>();
        if (_worldManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = GetComponentInChildren<PlayerManager>();
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cameraManager = GetComponentInChildren<CameraManager>();
        if (_cameraManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _uiManager = GetComponentInChildren<UIManager>();
        if (_uiManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _locationManager = GetComponentInChildren<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _networkManager = GetComponentInChildren<NetWorkManager>();
        if (_networkManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _unitsManager = GetComponentInChildren<UnitsManager> ();
		if(_unitsManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

    }

    public void StartGame(Vector3 worldNodeLoc)
    {
        _unitsManager.LoadPlayersUnits(worldNodeLoc);
    }

}

