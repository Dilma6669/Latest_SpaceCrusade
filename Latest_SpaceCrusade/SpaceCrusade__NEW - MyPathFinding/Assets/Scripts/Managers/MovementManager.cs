using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementManager : NetworkBehaviour
{

    LocationManager _locationManager;
	PathFinding _pathFinding;
    DataManipulation _dataManipulation;

	private List<GameObject> unitsToMove = new List<GameObject>();


	void Awake() {

        _locationManager = transform.parent.GetComponentInChildren<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _pathFinding = GetComponent<PathFinding> ();
		if(_pathFinding == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _dataManipulation = FindObjectOfType<DataManipulation>();
        if (_dataManipulation == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    // this is now being done on sevrer and return a list of vector3 to make node visual display for path for client 
    public int[] SetUnitsPath(GameObject objToMove, Vector3 start, Vector3 end) {

        unitsToMove.Add (objToMove);

		UnitScript unitScript = objToMove.GetComponent<UnitScript> ();

        List<CubeLocationScript> path = _pathFinding.FindPath(unitScript, start, end);
        objToMove.GetComponent<MovementScript>().MoveUnit(path);
        List<Vector3> vects = _dataManipulation.GetLocVectorsFromCubeScript(path);
        int[] movePath = _dataManipulation.ConvertVectorsIntoIntArray(vects);
        return movePath;
    }


    public void StopUnits() {


	}

    public void CreatePathFindingNodes(GameObject unit, int unitNetID, int[] path)
    {
        unit.GetComponent<UnitScript>().ClearPathFindingNodes();

        List<Vector3> vects = _dataManipulation.ConvertIntArrayIntoVectors(path);

        List<CubeLocationScript> scriptList = new List<CubeLocationScript>();

        foreach(Vector3 vect in vects)
        {
            CubeLocationScript script = _locationManager.GetLocationScript(vect);
            script.CreatePathFindingNode(unitNetID);
            scriptList.Add(script);
            //Debug.Log("pathfinding VISUAL node set at vect: " + vect);
        }

        unit.GetComponent<UnitScript>().AssignPathFindingNodes(scriptList);
    }
}
