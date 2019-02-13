using UnityEngine;

public class GameManager : MonoBehaviour {

    ////////////////////////////////////////////////

    private static GameManager _instance;

    ////////////////////////////////////////////////

    public WorldManager     _worldManager;
    public PlayerManager    _playerManager;
    public LocationManager  _locationManager;
    public MovementManager  _movementManager;
    public CombatManager    _combatManager;
    public CameraManager    _cameraManager;
    public UIManager        _uiManager;
    public NetWorkManager   _networkManager;
    public UnitsManager     _unitsManager;

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

        if (_worldManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_movementManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_combatManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_cameraManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_uiManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_networkManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_unitsManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void StartGame(Vector3 worldNodeLoc)
    {
        _unitsManager.LoadPlayersUnits(worldNodeLoc);
    }

}

