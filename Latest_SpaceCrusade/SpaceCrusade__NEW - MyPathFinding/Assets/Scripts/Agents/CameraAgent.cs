using UnityEngine;
using UnityEngine.Networking;


public class CameraAgent : NetworkBehaviour
{
    ////////////////////////////////////////////////

    GameManager _gameManager;
    CameraManager _cameraManager;

    ////////////////////////////////////////////////

    [HideInInspector]
	public Camera _camera;

    ////////////////////////////////////////////////

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

	public float angleH = 180;                                          // Float to store camera horizontal angle related to mouse movement.
    public float angleV = -26;                                          // Float to store camera vertical angle related to mouse movement.

	private float targetFOV;                                           // Target camera FIeld of View.
	private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle. 

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.enabled = false;

        // change layer callback event
        // UIManager.OnChangeLayerClick += ChangeCameraLayer;
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _cameraManager = _gameManager._cameraManager;
        if (_cameraManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (!isLocalPlayer) return;
        _cameraManager.Camera_Agent = this;
        _camera.enabled = true;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////


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
}
