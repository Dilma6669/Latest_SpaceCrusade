using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBuilder : MonoBehaviour {

    public GameObject _defaultCubePrefab; // Debugging purposes

    GridBuilder _gridBuilder;

    PanelBuilder _panelBuilder;
	ObjectBuilder _objectBuilder;

	private int rotationY = 0;

	void Awake() {

        _gridBuilder = transform.parent.GetComponentInChildren<GridBuilder>();
        if (_gridBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _panelBuilder = GetComponentInChildren<PanelBuilder> ();
		if(_panelBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_objectBuilder = GetComponentInChildren<ObjectBuilder> ();
		if(_objectBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}
		

	public void CreateCubeObject(Vector3 gridLoc, int cubeType, int rotations, int layerCount, Transform parent){

		rotationY = (rotations * -90) % 360;

        GameObject cubeObject = Instantiate(_defaultCubePrefab, transform, false); // empty cube
        cubeObject.transform.SetParent(parent);
        cubeObject.transform.position = gridLoc;
        CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript>();

        _gridBuilder._GridLocToScriptLookup[gridLoc] = cubeScript;

        cubeScript.cubeLoc = gridLoc;
		cubeObject.transform.eulerAngles = new Vector3 (0, rotationY, 0);
        cubeObject.gameObject.layer = LayerMask.NameToLayer ("Floor" + layerCount.ToString ());

		cubeScript.cubeAngle = (int)rotationY;

		switch (cubeType) {
		case 00:
			break;
		case 01:
			_panelBuilder.CreatePanelForCube ("Floor", cubeObject.transform, layerCount, 0, rotations);
			break;
		case 02:
			_panelBuilder.CreatePanelForCube ("Wall", cubeObject.transform, layerCount, 90, rotations); // Down
			break;
		case 03:
			_panelBuilder.CreatePanelForCube ("Wall", cubeObject.transform, layerCount, 0, rotations); // across
			break;
		case 04:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 90, rotations);
			break;
		case 05:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 270, rotations); 
			break;
		case 06:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 180, rotations);
			break;
		case 07:
			_panelBuilder.CreatePanelForCube ("FloorAngle", cubeObject.transform, layerCount, 0, rotations);
			break;
		case 08:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 90, rotations);
			break;
		case 09:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 270, rotations); 
			break;
		case 10:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 180, rotations);
			break;
		case 11:
			_panelBuilder.CreatePanelForCube ("CeilingAngle", cubeObject.transform, layerCount, 0, rotations);
			break;
		}
	}

}
