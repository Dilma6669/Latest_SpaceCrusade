using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLocationScript : MonoBehaviour {

	public Vector3 cubeLoc;


	public int cubeUniqueID;

	public GameObject _pathFindingPrefab;

	public bool _cubeOccupied = false; // If a guy is on square
	public MovementScript _flagToSayIsMine = null;

	public bool _isPanel = false;

	public int cubeAngle = 0;

	public bool _isHumanWalkable = false;
	public bool _isHumanClimbable = false;
	public bool _isHumanJumpable = false;

	public int fCost;
	public int hCost;
	public int gCost;


	public PanelPieceScript panelScriptChild = null;

	public GameObject _activePanel;

	[HideInInspector]
	public CubeLocationScript parentPathFinding;

	public List<Vector3> neighVects = new List<Vector3>();
	public List<Vector3> neighHalfVects = new List<Vector3>();

	public bool[] neighBools = new bool[27];

	public GameObject pathFindingNode = null;

	public bool cubeVisible = true;
	public bool cubSelected = false;

	void Awake() {

		cubeLoc = new Vector3 (-1, -1, -1);
	}


	public void CubeActive(bool onOff) {
		
		if (onOff) {
			cubSelected = true;
		} else {
			cubSelected = false;
			_activePanel.GetComponent<PanelPieceScript> ().ActivatePanel (false);
		}
	}

	///////////////////////////////
	/// this is for when panel is clicked
	public void CubeSelect(bool onOff, Vector3 nodePos = new Vector3(), GameObject panelSelected = null) {

		if (onOff) {
			CubeActive (true);
			_activePanel = panelSelected;
			//_cubeManager.SetCubeActive (true, new Vector3(cubeLoc.x, cubeLoc.y, cubeLoc.z), nodePos);
		} else {
			CubeActive (false);
			_activePanel = null;
			//_cubeManager.SetCubeActive (false);
		}
	}

	////////////////////////////////////////////////
	// Special green box highlighting cube 
//	public void CubeHighlight(string selectType) {
//	//	if (cubeWalkable && cubeVisible) {
//			if (transform.Find ("CubeSelect")) {
//				switch (selectType) { 
//				case "Move":
//					cubeSelectTrans.SetActive (true);
//					break;
//				default:
//					break;
//				}
//			}
//	//	}
//	}
//	public void CubeUnHighlight(string selectType) {
//	//	if (transform.Find ("CubeSelect")) {
//			switch (selectType) { 
//			case "Move":
//				cubeSelectTrans.SetActive (false);
//				break;
//			default:
//				break;
//			}
//		//}
//	}
	////////////////////////////////////////////////

	public void GetHalfNeighbourConnections() {

		Vector3 ownVect = new Vector3(cubeLoc.x, cubeLoc.y, cubeLoc.z);

		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z - 1)); // 0
		//neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z - 1)); // 1
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z - 1)); // 2
		//
		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z + 0)); // 3
		neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z + 0)); // 4 directly below
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z + 0)); // 5
		//
		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z + 1)); // 6
		//neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z + 1)); // 7
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z + 1)); // 8

		/////////////////////////////////
		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z - 1)); // 9
		neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z - 1)); // 10 infront (south)
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z - 1)); // 11

		neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z + 0)); // 12 side (west)
		neighHalfVects.Add(ownVect);												   // 13 //// MIDDLE
		neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z + 0)); // 14 side (east)

		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z + 1)); // 15
		neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z + 1)); // 16 back (North)
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z + 1)); // 17 
		/////////////////////////////////

		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z - 1)); // 18
		//neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z - 1)); // 19
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z - 1)); // 20
		//
		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z + 0)); // 21
		neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z + 0)); // 22 directly above
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z + 0)); // 23
		//
		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z + 1)); // 24
		//neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z + 1)); // 25
		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z + 1)); // 26

		for( int i = 0; i < neighHalfVects.Count; i++) {
			if (neighHalfVects[i].x <= -1 || neighHalfVects[i].y <= -1 || neighHalfVects[i].z <= -1) {
				neighHalfVects[i] = new Vector3 (-6, -6, -6);
			}
		}
		/////////////////////////////////

	}

	public void GetNeighbourConnections() {

		Vector3 ownVect = new Vector3(cubeLoc.x, cubeLoc.y, cubeLoc.z);

		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z - 2)); // 0
		//neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z - 2)); // 1
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z - 2)); // 2
		//
		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z + 0)); // 3
		neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z + 0)); // 4 directly below
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z + 0)); // 5
		//
		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z + 2)); // 6
		//neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z + 2)); // 7
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z + 2)); // 8

		/////////////////////////////////
		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z - 2)); // 9
		neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z - 2)); // 10 infront (south)
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z - 2)); // 11

		neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z + 0)); // 12 side (west)
		neighVects.Add(ownVect);												   // 13 //// MIDDLE
		neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z + 0)); // 14 side (east)

		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z + 2)); // 15
		neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z + 2)); // 16 back (North)
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z + 2)); // 17 
		/////////////////////////////////

		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z - 2)); // 18
		//neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z - 2)); // 19
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 2, ownVect.z - 2)); // 20
		//
		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z + 0)); // 21
		neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z + 0)); // 22 directly above
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 2, ownVect.z + 0)); // 23
		//
		//neighVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z + 2)); // 24
		//neighVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z + 2)); // 25
		//neighVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 2, ownVect.z + 2)); // 26

		for( int i = 0; i < neighVects.Count; i++) {
			if (neighVects[i].x <= -1 || neighVects[i].y <= -1 || neighVects[i].z <= -1) {
				neighVects[i] = new Vector3 (-6, -6, -6);
			}
		}
		/////////////////////////////////

	}

	public void CreatePathFindingNode() {

		GameObject nodeObject = Instantiate (_pathFindingPrefab, transform, false); // empty cube
		nodeObject.transform.SetParent (transform);
		nodeObject.transform.position = transform.position;
	}

	////////////////////////////////////////////////
	// If player canNOT see this cube
	public void CubeNotVisible() {
		Debug.Log ("CubeNotVisible");
		cubeVisible = false;
//		foreach (Transform child in transform) {
//			if (child.gameObject.activeSelf && child.GetComponent<PanelPieceScript> ()) {
//				child.GetComponent<PanelPieceScript> ().PanelPieceChangeColor ("Black");
//			}
//		}
	}
	// If player can see this cube
	public void CubeVisible() {
		cubeVisible = true;
//		foreach (Transform child in transform) {
//			if (child.gameObject.activeSelf && child.GetComponent<PanelPieceScript> ()) {
//				child.GetComponent<PanelPieceScript> ().PanelPieceChangeColor ("White");
//			}
//		}
	}
	////////////////////////////////////////////////
}
