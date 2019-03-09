using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static MovementManager _instance;

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

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // this is now being done on sevrer and return a list of vector3 to make node visual display for path for client 
    public static int[] SetUnitsPath(GameObject objToMove, Vector3 start, Vector3 end)
    {
        UnitScript unitScript = objToMove.GetComponent<UnitScript>();

        List<CubeLocationScript> path = PathFinding.FindPath(unitScript, start, end);

        if (path == null) { return null; }

        objToMove.GetComponent<MovementScript>().MoveUnit(path);
        List<Vector3> vects = DataManipulation.GetLocVectorsFromCubeScript(path);
        int[] movePath = DataManipulation.ConvertVectorsIntoIntArray(vects);
        return movePath;
    }


    public static void StopUnits() {


	}

    public static void CreatePathFindingNodes(GameObject unit, int unitNetID, int[] path)
    {
        unit.GetComponent<UnitScript>().ClearPathFindingNodes();

        List<Vector3> vects = DataManipulation.ConvertIntArrayIntoVectors(path);

        List<CubeLocationScript> scriptList = new List<CubeLocationScript>();

        foreach(Vector3 vect in vects)
        {
            CubeLocationScript script = LocationManager.GetLocationScript(vect);
            script.CreatePathFindingNode(unitNetID);
            scriptList.Add(script);
            //Debug.Log("pathfinding VISUAL node set at vect: " + vect);
        }

        unit.GetComponent<UnitScript>().AssignPathFindingNodes(scriptList);
    }
}
