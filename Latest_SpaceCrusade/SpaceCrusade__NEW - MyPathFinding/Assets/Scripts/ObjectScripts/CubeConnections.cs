using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeConnections : MonoBehaviour {

	LocationManager _locationManager;

	private static CubeConnections instance = null;

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_locationManager = GetComponentInChildren<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


	public void SetCubeNeighbours(Dictionary<Vector3, CubeLocationScript> _LocationLookup) {
		
		foreach (CubeLocationScript cubeScript in _LocationLookup.Values) {
			cubeScript.GetNeighbourConnections ();
			cubeScript.GetHalfNeighbourConnections ();

			// setup Panels in cubes
			if (cubeScript.panelScriptChild) {
				SetUpPanelInCube (cubeScript);
			}
		}
	}



	// If ANY kind of wall/floor/object make neighbour cubes walkable
	public void SetUpPanelInCube(CubeLocationScript cubeScript) {

		cubeScript._isPanel = true;

		PanelPieceScript panelScript = cubeScript.panelScriptChild;

		switch(panelScript.name) 
		{
		case "Floor":
			SetUpFloorPanel (cubeScript, panelScript);
			break;
		case "Wall":
			SetUpWallPanel (cubeScript, panelScript);
			break;
		case "FloorAngle": // angles put in half points
			SetUpFloorAnglePanel (cubeScript, panelScript);
			break;
		case "CeilingAngle": // This is the exact same as ceilingFloor
			SetUpCeilingAnglePanel (cubeScript, panelScript);
			break;
		default:
			Debug.Log ("fuck no issue:  " + panelScript.name);
			break;
		}
	}




	private void SetUpFloorPanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 leftVect, rightVect;
		GameObject cubeLeft, cubeRight;
		CubeLocationScript cubeScriptLeft = null;
		CubeLocationScript cubeScriptRight = null;

		Vector3 cubeLoc = cubeScript.cubeLoc;

		leftVect = new Vector3 (cubeLoc.x, cubeLoc.y - 1, cubeLoc.z);
		cubeScriptLeft = _locationManager.GetLocationScript(leftVect);
		if (cubeScriptLeft != null) {
			panelScript.cubeScriptLeft = cubeScriptLeft;
			panelScript.cubeLeftVector = leftVect;
			panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);
			if (!cubeScriptLeft._isPanel) {
				cubeScriptLeft._isHumanWalkable = true; 
			}
			// make edges empty spaces for climbing over
			MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // South
			MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
			MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // North
			MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East

		}

		rightVect = new Vector3 (cubeLoc.x, cubeLoc.y + 1, cubeLoc.z);
		cubeScriptRight = _locationManager.GetLocationScript(rightVect);
		if (cubeScriptRight != null) {
			panelScript.cubeScriptRight = cubeScriptRight;
			panelScript.cubeRightVector = rightVect;
			panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);
			if (!cubeScriptRight._isPanel) {
				cubeScriptRight._isHumanWalkable = true; 
			}
			// make edges empty spaces for climbing over
			MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z - 2)); // South
			MakeClimbableEdges (new Vector3 (rightVect.x - 2, rightVect.y, rightVect.z)); // West
			MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z + 2)); // North
			MakeClimbableEdges (new Vector3 (rightVect.x + 2, rightVect.y, rightVect.z)); // East
		}

		// 8 points for each panel
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z + 0));
		// middle
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z + 0));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z + 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y, cubeLoc.z + 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z + 1));

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
		}
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode; 
		}
	}


	private void SetUpWallPanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.cubeLoc;

		Vector3 leftVect, rightVect;
		GameObject cubeLeft, cubeRight;
		CubeLocationScript cubeScriptLeft = null;
		CubeLocationScript cubeScriptRight = null;

		int cubeAngle = (int)cubeScript.cubeAngle;
		int panelAngle = (int)panelScript.panelAngle;

		panelScript._isLadder = true;

		int result = (cubeAngle - panelAngle);
		result = (((result + 180) % 360 + 360) % 360) - 180;
		//Debug.Log ("cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " result: " + result);

		if (result == 180 || result == -180 || result == 0) { // Down
			leftVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z - 1);
			cubeScriptLeft = _locationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);
				if (panelScript._isLadder || !cubeScriptLeft._isPanel) {
					cubeScriptLeft._isHumanClimbable = true;
				}
				// make edges empty spaces for climbing over
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East
			}

			rightVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z + 1);
			cubeScriptRight = _locationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);
				if (panelScript._isLadder || !cubeScriptRight._isPanel) {
					cubeScriptRight._isHumanClimbable = true;
				}
				// make edges empty spaces for climbing over
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y - 2, rightVect.z)); // South
				MakeClimbableEdges (new Vector3 (rightVect.x - 2, rightVect.y, rightVect.z)); // West
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y + 2, rightVect.z)); // North
				MakeClimbableEdges (new Vector3 (rightVect.x + 2, rightVect.y, rightVect.z)); // East
			}

			// 8 points for each panel
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
			// middle
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z + 0));

		} else if (result == 90 || result == -90) { //across 

			leftVect = new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z);
			cubeScriptLeft = _locationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (-4.5f, 0, 0);
				if (panelScript._isLadder || !cubeScriptLeft._isPanel) {
					cubeScriptLeft._isHumanClimbable = true;
				}
				// make edges empty spaces for climbing over
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // East
			}

			rightVect = new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z);
			cubeScriptRight = _locationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (4.5f, 0, 0);
				if (panelScript._isLadder || !cubeScriptRight._isPanel) {
					cubeScriptRight._isHumanClimbable = true;
				}
				// make edges empty spaces for climbing over
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y - 2, rightVect.z)); // South
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z - 2)); // West
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y + 2, rightVect.z)); // North
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z + 2)); // East
			}

			// 8 points for each panel
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z - 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 0, cubeLoc.z - 1));
			// middle
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 0, cubeLoc.z + 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z - 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 1));
		} else {
			Debug.Log ("SOMETHING weird: cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
		}

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
		}
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode; 
		}
	}


	private void SetUpFloorAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.cubeLoc;

		cubeScript._isPanel = false; // this might cause issues

		Vector3 centerVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z);
		panelScript.cubeScriptLeft = cubeScript;
		panelScript.cubeLeftVector = centerVect;
		panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

		panelScript.cubeScriptRight = cubeScript;
		panelScript.cubeRightVector = centerVect;
		panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);

		//cubeScript._isHumanWalkable = true;

		//		// 8 points for each panel
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		// middle
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z + 1));
	}


	private void SetUpCeilingAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.cubeLoc;

		cubeScript._isPanel = false; // this might cause issues

		Vector3 centerVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z);
		panelScript.cubeScriptLeft = cubeScript;
		panelScript.cubeLeftVector = centerVect;
		panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

		panelScript.cubeScriptRight = cubeScript;
		panelScript.cubeRightVector = centerVect;
		panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);

		//cubeScript._isHumanWalkable = true;
		//		// 8 points for each panel
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		// middle
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z + 1));
	}


	private void DoHalfPointsForWalls(Vector3 nodeVect) {

		CubeLocationScript nodeScript = _locationManager.GetLocationScript(nodeVect);
		if (nodeScript != null) {
			nodeScript._isPanel = true;
			nodeScript._isHumanWalkable = false;
		}
	}


	private void MakeClimbableEdges(Vector3 nodeVect) {

		CubeLocationScript nodeScript = _locationManager.GetLocationScript(nodeVect);
		if (nodeScript != null) {
			if (!nodeScript._isPanel) {
				nodeScript._isHumanClimbable = true; 
			}
		}
	}
}
