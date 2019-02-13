using UnityEngine;

public class CubeBuilder : MonoBehaviour {

    ////////////////////////////////////////////////

    private static CubeBuilder _instance;

    ////////////////////////////////////////////////

    public PanelBuilder _panelBuilder;
    public ObjectBuilder _objectBuilder;

    public GameObject _defaultCubePrefab; // Debugging purposes

    ////////////////////////////////////////////////

    GameManager _gameManager;

    ////////////////////////////////////////////////

	private int rotationY = 0;

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

        if (_panelBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_objectBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_defaultCubePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public CubeLocationScript CreateCubeObject(Vector3 gridLoc, int cubeType, int rotations, int layerCount, Transform parent)
    {

        rotationY = (rotations * -90) % 360;

        GameObject cubeObject = Instantiate(_defaultCubePrefab, transform, false); // empty cube
        cubeObject.transform.SetParent(parent);
        cubeObject.transform.position = gridLoc;
        CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript>();
        _gameManager._worldManager._gridBuilder.SetCubeScriptToGridLocation(gridLoc, cubeScript);
        cubeScript._locationManager = _gameManager._locationManager;

        cubeScript.CubeLocVector = gridLoc;
        cubeObject.transform.eulerAngles = new Vector3(0, rotationY, 0);
        //cubeObject.gameObject.layer = LayerMask.NameToLayer ("Floor" + layerCount.ToString ());

        cubeScript.CubeAngle = (int)rotationY;

        switch (cubeType)
        {
            case 00:
                return null;
            case 01:
                _panelBuilder.CreatePanelForCube("Floor", cubeObject.transform, layerCount, 0, rotations);
                break;
            case 02:
                _panelBuilder.CreatePanelForCube("Wall", cubeObject.transform, layerCount, 90, rotations); // Down
                break;
            case 03:
                _panelBuilder.CreatePanelForCube("Wall", cubeObject.transform, layerCount, 0, rotations); // across
                break;
            case 04:
                _panelBuilder.CreatePanelForCube("FloorAngle", cubeObject.transform, layerCount, 90, rotations);
                break;
            case 05:
                _panelBuilder.CreatePanelForCube("FloorAngle", cubeObject.transform, layerCount, 270, rotations);
                break;
            case 06:
                _panelBuilder.CreatePanelForCube("FloorAngle", cubeObject.transform, layerCount, 180, rotations);
                break;
            case 07:
                _panelBuilder.CreatePanelForCube("FloorAngle", cubeObject.transform, layerCount, 0, rotations);
                break;
            case 08:
                _panelBuilder.CreatePanelForCube("CeilingAngle", cubeObject.transform, layerCount, 90, rotations);
                break;
            case 09:
                _panelBuilder.CreatePanelForCube("CeilingAngle", cubeObject.transform, layerCount, 270, rotations);
                break;
            case 10:
                _panelBuilder.CreatePanelForCube("CeilingAngle", cubeObject.transform, layerCount, 180, rotations);
                break;
            case 11:
                _panelBuilder.CreatePanelForCube("CeilingAngle", cubeObject.transform, layerCount, 0, rotations);
                break;
        }
        return cubeScript;
    }

}
