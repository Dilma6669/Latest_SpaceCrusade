using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {

	public GameManager _gameManager;

	private bool moveInProgress = false;

	private GameObject _unit = null;
	private List<CubeLocationScript> _nodes;

	private bool collision = false;

	public int locCount = 0;

	// Use this for initialization
	void Update () {

		if (moveInProgress) {
			StartMoving ();
		}
	}


	private void StartMoving() {

		Vector3 unitCurrPos = _unit.transform.position;

		if (locCount < _nodes.Count) {
				
			CubeLocationScript target = _nodes [locCount];
				
			if (target != null) {
				
				Vector3 currTarget = new Vector3 (target.cubeLoc.x, target.cubeLoc.y, target.cubeLoc.z);

				if (!target._cubeOccupied || target._flagToSayIsMine == this || target._flagToSayIsMine == null) {
						
					target._flagToSayIsMine = this;
					target._cubeOccupied = true;
					collision = false;

					if (!target.gameObject.transform.Find ("pathFindingNode(Clone)")) {
						target.CreatePathFindingNode (); // puts circles in path, visual reference
					}

					if (unitCurrPos != currTarget) {
						_unit.transform.position = Vector3.MoveTowards (unitCurrPos, currTarget, _nodes.Count * Time.deltaTime);
					} else {
						if (target.gameObject.transform.Find ("pathFindingNode(Clone)").gameObject) {
							Destroy (target.gameObject.transform.Find ("pathFindingNode(Clone)").gameObject);
						}
						target._cubeOccupied = false;
						target._flagToSayIsMine = null;
						_nodes [locCount] = null;
						locCount += 1;
						if (locCount == _nodes.Count) {
							FinishMoving ();
						}
					}
				} else if (target._flagToSayIsMine != this && target._flagToSayIsMine != false) {

					if (collision == false) {
						collision = true;
						CubeLocationScript nodeToRemove = _nodes [_nodes.Count - 1];
						_nodes.Remove (nodeToRemove);
					}
				}
			}
		}
	}

	private void FinishMoving() {
		Debug.Log ("FINFISHED!");
		moveInProgress = false;
		GetComponent<UnitScript> ().movePath.Clear ();
		_nodes.Clear ();
	}


	public void MoveUnit(GameObject unit, List<CubeLocationScript> nodes) {
		
		_unit = unit;
		_nodes = nodes;

		moveInProgress = true;
	}
}
