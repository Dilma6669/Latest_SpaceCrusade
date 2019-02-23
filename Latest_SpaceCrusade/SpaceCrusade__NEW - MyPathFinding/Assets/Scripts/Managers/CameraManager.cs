using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static CameraManager _instance;

    private CameraAgent _cameraAgent;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    PlayerManager _playerManager;

    ////////////////////////////////////////////////

    public CameraAgent Camera_Agent
    {
        get { return _cameraAgent; }
        set { _cameraAgent = value; }
    }

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

    // Need this Start()
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void SetUpCameraAndLayers(int playerID, CameraAgent cameraAgent)
    {
        Start();
        Camera_Agent = cameraAgent;


        KeyValuePair<Vector3, Vector3> camStartPos = GetCameraStartPosition(playerID);

        Vector3 camPos = camStartPos.Key;
        Quaternion camRot = Quaternion.Euler(camStartPos.Value);

        _gameManager._playerManager.PlayerAgent.SetUpPlayerStartPosition(camPos, camRot);

        Camera_Agent.angleH = camRot.eulerAngles.y;
        Camera_Agent.angleV = -camRot.eulerAngles.x;

        /*

        // reveal layers up to current
        for (int i = 0; i <= _currLayer; i++) 
        {
            _camera.cullingMask |= 1 << LayerMask.NameToLayer("Floor" + i.ToString ());
        }
        */

        // units have already been put into correct layer now need to make camera see layer
        //string layerStr = "Player0" + playerID.ToString () + "Units";
        //_camera.cullingMask |= 1 << LayerMask.NameToLayer (layerStr);
    }

    public KeyValuePair<Vector3, Vector3> GetCameraStartPosition(int playerID = -1)
    {
        List<KeyValuePair<Vector3, Vector3>> cameraPositions = new List<KeyValuePair<Vector3, Vector3>>();

        KeyValuePair<Vector3, Vector3> cam0 = new KeyValuePair<Vector3, Vector3>(new Vector3(-124.3f, 475, 895.9f), new Vector3(0, 90, 0));
        KeyValuePair<Vector3, Vector3> cam1 = new KeyValuePair<Vector3, Vector3>(new Vector3(11, 572, -879), new Vector3(0, 90, 0));
        KeyValuePair<Vector3, Vector3> cam2 = new KeyValuePair<Vector3, Vector3>(new Vector3(-955, 489.4f, -71), new Vector3(0, 0, 0));
        KeyValuePair<Vector3, Vector3> cam3 = new KeyValuePair<Vector3, Vector3>(new Vector3(738, 344, -210), new Vector3(0, 0, 0));

        cameraPositions.Add(cam0);
        cameraPositions.Add(cam1);
        cameraPositions.Add(cam2);
        cameraPositions.Add(cam3);

        return cameraPositions[playerID];
    }

    public void SetCamToOrbitUnit(Transform unit)
    {
        Camera_Agent.SetCamToOrbitUnit(unit);
    }


    /*
	public void ChangeCameraLayer(int change) {

		if (change == 1) {
			if (_currLayer >= _maxLayer) {
				return;
			}
			_currLayer += change;
			if (_currLayer >= _maxLayer) {
				_currLayer = _maxLayer;
			}
			_camera.cullingMask |= 1 << LayerMask.NameToLayer("Floor" + _currLayer.ToString ());
		} else if (change == -1) {
			if (_currLayer <= _minLayer) {
				return;
			}
			_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Floor" + _currLayer.ToString ()));
			_currLayer += change;
			if (_currLayer <= _minLayer) {
				_currLayer = _minLayer;
			}
		}
	}
    */
}
