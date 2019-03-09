using UnityEngine;
using UnityEngine.Networking;


public class CameraAgent : NetworkBehaviour
{
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
    private float minVerticalAngle = 0f;                                 // Camera min clamp angle.

    private float h;
    private float v;
    private float x;
    private float y;

    public float angleH = 180;                                          // Float to store camera horizontal angle related to mouse movement.
    public float angleV = -26;                                          // Float to store camera vertical angle related to mouse movement.

    private float targetFOV;                                           // Target camera FIeld of View.
    private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle. 

    ////////////////////////////////////////////////

    private bool _unitOrbitCamEnable = false;
    private Transform _unitToOrbit = null;

    private float _turnSpeed = 4.0f;

    private float orbitCamDist = 10;
    private Vector3 _offset;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.enabled = false;

    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CameraManager.Camera_Agent = this;
        _camera.enabled = true;
    }


    void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (_camera)
        {
            if (_unitOrbitCamEnable)
            {
                /*
                var d = Input.GetAxis("Mouse ScrollWheel");
                if (d > 0f)
                {
                   //-orbitCamDist += 1;
                    _offset = new Vector3(_offset.x + 1, _offset.y + 1, _offset.z + 1);
                }
                else if (d < 0f)
                {
                    //orbitCamDist -= 1;
                    _offset = new Vector3(_offset.x - 1, _offset.y - 1, _offset.z - 1);
                }
                */
                int keyNum = 0;
                if (Input.GetKey(KeyCode.Q))
                {
                    keyNum = 1;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    keyNum = -1;
                }
                _offset = Quaternion.AngleAxis(keyNum * _turnSpeed, Vector3.up) * _offset;
                transform.position = _unitToOrbit.position + _offset;
                transform.LookAt(_unitToOrbit.position);
            }
            else
            {
                // Mouse rotation
                if (Input.GetMouseButton(2))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        keysMovementSpeed = 5.0f;
                    }
                    else
                    {
                        keysMovementSpeed = 1.0f;
                    }

                    // mouse look around
                    x = Input.GetAxis("Mouse X");
                    y = Input.GetAxis("Mouse Y");

                    // Basic Movement Player //
                    h = Input.GetAxis("Horizontal");
                    v = Input.GetAxis("Vertical");

                    // Get mouse movement to orbit the camera.
                    angleH += Mathf.Clamp(x, -1, 1) * mouseRotationSpeed;
                    angleV += Mathf.Clamp(y, -1, 1) * mouseRotationSpeed;

                    //Sets x and y basic movement
                    transform.Translate(new Vector3(keysMovementSpeed * h, 0, 0));
                    transform.Translate(new Vector3(0, 0, keysMovementSpeed * v));

                    // Set camera orientation..
                    Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
                    transform.rotation = aimRotation;
                }
            }
        }
    }

    public void SetCamToOrbitUnit(Transform unit)
    {
        _unitToOrbit = unit;
        _offset = new Vector3(orbitCamDist, orbitCamDist, orbitCamDist);
        _unitOrbitCamEnable = true;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
}