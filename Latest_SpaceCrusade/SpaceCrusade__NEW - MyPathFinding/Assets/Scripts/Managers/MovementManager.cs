using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

    GameManager _gameManager;

	PathFinding _pathFinding;

	private List<GameObject> unitsToMove = new List<GameObject>();

	void Awake() {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _pathFinding = GetComponent<PathFinding> ();
		if(_pathFinding == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}



	public void SetUnitsPath(GameObject objToMove, bool canClimbWalls, Vector3 start, Vector3 end, Vector3 posOffset) {

        unitsToMove.Add (objToMove);

		UnitScript unitScript = objToMove.GetComponent<UnitScript> ();

		List<CubeLocationScript> nodes = unitScript.movePath;
		if(unitScript.movePath.Count != 0) {
			foreach (CubeLocationScript node in nodes) {
				if (node.pathFindingNode) {
                    Destroy (node.pathFindingNode);
				}
			}
		}
		unitScript.movePath.Clear ();
        int unitsMovementStat = unitScript.UnitCombatStats[0];
        unitScript.movePath = _pathFinding.FindPath (unitsMovementStat, canClimbWalls, start, end, posOffset);

        int[] pathArray = GetUnitsMovePathInArray(unitScript); // need a struct of int[]'s to go in here;
        _gameManager._playerManager._playerObject.GetComponent<NetworkAgent>().CmdTellServerToMoveUnit(unitScript.NetID, pathArray);
    }


    /*
	public void MoveUnits() { 

		foreach (GameObject unit in unitsToMove) {
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            List<CubeLocationScript> nodes = unit.GetComponent<UnitScript> ().movePath;
			foreach (CubeLocationScript node in nodes) {
				if (node.pathFindingNode) {
					Destroy (node.pathFindingNode);
				}
			}
            List<Vector3> vectorList = GetUnitsMovePathInVectors(unitScript);
            _gameManager._playerManager._playerObject.GetComponent<NetworkAgent>().CmdTellServerToMoveUnit(unitScript.NetID, vectorList);
        }
        unitsToMove.Clear ();
	}
    */


    private int[] GetUnitsMovePathInArray(UnitScript unitScript)
    {
        List<CubeLocationScript> nodes = unitScript.movePath;

        int[] pathArray = new int[nodes.Count*3];

        int index = 0;
        if (unitScript.movePath.Count != 0)
        {
            foreach (CubeLocationScript node in nodes)
            {
                int x = (int)node.cubeLoc.x;
                int y = (int)node.cubeLoc.y;
                int z = (int)node.cubeLoc.z;

                pathArray[index] = x;
                index += 1;
                pathArray[index] = y;
                index += 1;
                pathArray[index] = z;
                index += 1;
            }
        }
        return pathArray;
    }


    public void StopUnits() {


	}
}
