using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

   // [HideInInspector]
    public PlayerManager    _playerManager;
   // [HideInInspector]
    public CameraManager    _cameraManager;
   // [HideInInspector]
	public LocationManager  _locationManager;
    //[HideInInspector]
    public UIManager        _uiManager;
    //[HideInInspector]
    public NetWorkManager   _networkManager;
   // [HideInInspector]
	public UnitsManager     _unitsManager;
   // [HideInInspector]
    public GamePlayManager  _gamePlayManager;



    void Awake() {

        _playerManager = GetComponentInChildren<PlayerManager>();
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cameraManager = GetComponentInChildren<CameraManager>();
        if (_cameraManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _locationManager = GetComponentInChildren<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _networkManager = GetComponentInChildren<NetWorkManager>();
        if (_networkManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _uiManager = GetComponentInChildren<UIManager>();
        if (_uiManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _unitsManager = GetComponentInChildren<UnitsManager> ();
		if(_unitsManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _gamePlayManager = GetComponentInChildren<GamePlayManager>();
        if (_gamePlayManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


	void Start() {

        //	_locationManager.BuildMap ();

        //_unitsManager.LoadPlayersUnitAgents();
        //_cameraManager.LoadPlayersCameraAgents();

    }

	// this is because maps are built in an IEnumerator now
	public void MapsFinishedLoading() {

//		_cubeManager.SetUpPanelCubeScripts ();
//
//		_cubeManager.SetCubeNeighbours ();
//
//		_unitManager.LoadPlayers ();

		//	_cubeManager.GetCubeConnections ();

		//		_uiManager.PutFloorsIntoLists ();
		//		_uiManager.PutCeilingsIntoLists ();

	}

}

