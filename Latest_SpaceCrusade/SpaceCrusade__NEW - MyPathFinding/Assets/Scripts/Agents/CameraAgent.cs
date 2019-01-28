using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CameraAgent : NetworkBehaviour {

    GameManager _gameManager;

	Camera _camera;

	private float mouseRotationSpeed = 10f;                         // Horizontal turn speed.
	private float keysMovementSpeed = 1.0f;                           // Vertical turn speed.

	private float zoomMinFOV = 15f;
	private float zoomMaxFOV = 90f;
	private float zoomSensitivity = 10f;

	private float smooth = 10f;                                         // Speed of camera responsiveness.

	private float maxVerticalAngle = 0f;                               // Camera max clamp angle. 
	private float minVerticalAngle = 0f;  								 // Camera min clamp angle.

	private float h;                                
	private float v;    
	private float x;                                
	private float y;  

	private float angleH = 180;                                          // Float to store camera horizontal angle related to mouse movement.
	private float angleV = -26;                                          // Float to store camera vertical angle related to mouse movement.

	private float targetFOV;                                           // Target camera FIeld of View.
	private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle. 

	// Layer INfo
	public int _minLayer;
    public int _maxLayer;
    public int _currLayer;

	void Awake()
	{
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _camera = GetComponent<Camera>();
        _camera.enabled = false;

        // change layer callback event
       // UIManager.OnChangeLayerClick += ChangeCameraLayer;
    
    }


	// Mouse rotation movement
	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}

		if (_camera) {
			
			// Mouse rotation
			if (Input.GetMouseButton (2)) {
             
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    keysMovementSpeed = 5.0f;
                }
                else
                {
                    keysMovementSpeed = 1.0f;
                }

				// mouse look around
				x = Input.GetAxis ("Mouse X");
				y = Input.GetAxis ("Mouse Y");

				// Basic Movement Player //
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");

				// Get mouse movement to orbit the camera.
				angleH += Mathf.Clamp (x, -1, 1) * mouseRotationSpeed;
				angleV += Mathf.Clamp (y, -1, 1) * mouseRotationSpeed;

				//Sets x and y basic movement
				transform.Translate (new Vector3 (keysMovementSpeed * h, 0, 0));
				transform.Translate (new Vector3 (0, 0, keysMovementSpeed * v));

				// Set camera orientation..
				Quaternion aimRotation = Quaternion.Euler (-angleV, angleH, 0);
				transform.rotation = aimRotation;
			}
		}
	}



	public void SetUpCameraAndLayers(int playerID) {

		_camera.enabled = true;
	
		KeyValuePair<Vector3, Vector3> camStartPos = _gameManager._cameraManager.GetCameraStartPosition (playerID);

		Vector3 camPos = camStartPos.Key;
		Quaternion camRot = Quaternion.Euler (camStartPos.Value);

        /*
		_currLayer = _gameManager._cameraManager.LayerStart;
        _maxLayer = _gameManager._cameraManager.LayerMax;

		// reveal layers up to current
		for (int i = 0; i <= _currLayer; i++) 
		{
			_camera.cullingMask |= 1 << LayerMask.NameToLayer("Floor" + i.ToString ());
		}
        */

		gameObject.transform.position = camPos;
		gameObject.transform.rotation = camRot;

		angleH = camRot.eulerAngles.y;
		angleV = -camRot.eulerAngles.x; 

		// units have already been put into correct layer now need to make camera see layer
		//string layerStr = "Player0" + playerID.ToString () + "Units";
		//_camera.cullingMask |= 1 << LayerMask.NameToLayer (layerStr);
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
