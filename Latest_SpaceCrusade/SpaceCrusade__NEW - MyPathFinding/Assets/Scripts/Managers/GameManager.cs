using UnityEngine;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public PlayerManager    _playerManager;
    [HideInInspector]
    public CameraManager    _cameraManager;
    [HideInInspector]
    public UIManager        _uiManager;
    [HideInInspector]
    public LocationManager _locationManager;
    [HideInInspector]
    public NetWorkManager   _networkManager;
    [HideInInspector]
    public GamePlayManager _gamePlayManager;
    [HideInInspector]
    public UnitsManager     _unitsManager;



    void Awake() {

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

        _gamePlayManager = GetComponentInChildren<GamePlayManager>();
        if (_gamePlayManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _unitsManager = GetComponentInChildren<UnitsManager> ();
		if(_unitsManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

    }

}

