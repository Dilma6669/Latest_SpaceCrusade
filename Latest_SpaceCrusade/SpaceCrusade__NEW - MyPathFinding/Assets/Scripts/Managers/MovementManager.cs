using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

	PathFinding _pathFinding;

	private List<GameObject> unitsToMove = new List<GameObject>();


	void Awake() {

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
        int unitsMovementStat = unitScript._unitStats[0];
        unitScript.movePath = _pathFinding.FindPath (unitsMovementStat, canClimbWalls, start, end, posOffset);
	}



	public void MoveUnits() {

		foreach (GameObject unit in unitsToMove) {
			List<CubeLocationScript> nodes = unit.GetComponent<UnitScript> ().movePath;
			foreach (CubeLocationScript node in nodes) {
				if (node.pathFindingNode) {
					Destroy (node.pathFindingNode);
				}
			}
			unit.GetComponent<MovementScript>().MoveUnit (unit, nodes);
		}
		unitsToMove.Clear ();
	}


	public void StopUnits() {


	}
}
