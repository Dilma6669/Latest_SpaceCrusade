using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UnitsAgent : NetworkBehaviour {

//	UnitsManager _unitsManager;
//
//	public PlayerAgent _playerAgent;
//
//	public GameObject _unitPrefab;
//
//	public GameObject _activeUnit = null;
//
//	public List<UnitScript> unitScripts = new List<UnitScript> ();
//
//	// Use this for initialization
//	void Awake () {
//
//		_unitsManager = FindObjectOfType<UnitsManager> ();
//		if(_unitsManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
//		_unitsManager._unitAgents.Add (this);
//	}
//
//
//	public bool LoadPlayersUnits(List<Vector3> unitLocs) {
//
//		foreach (Vector3 loc in unitLocs) {
//
//			if (!CreateUnit (loc)) {
//				return false;
//			}
//		}
//		return true;
//	}
//
//
//	private bool CreateUnit(Vector3 startingLoc) {
//
//		CubeLocationScript cubeScript = _unitsManager._locationManager.CheckIfCanMoveToCube(startingLoc);
//
//		if (cubeScript != null) {
//
//			GameObject unit = Instantiate (_unitPrefab, transform, false);
//			unit.transform.SetParent (this.gameObject.transform);
//			UnitScript unitscript = unit.gameObject.GetComponent<UnitScript> ();
//			unitScripts.Add (unitscript);
//
//			unit.transform.position = startingLoc;
//			unitscript._cubeUnitIsOn = cubeScript;
//			return true;
//
//		} else {
//			Debug.LogError ("Cant Place Unit on startUp: " + startingLoc);
//			return false;
//		}
//	}
//
//
//	public void AssignUniqueLayerToUnits() {
//
//		string layerStr = "Player0" + _playerAgent._playerUniqueID.ToString () + "Units";
//		gameObject.layer = LayerMask.NameToLayer (layerStr);
//
//		Transform[] children = gameObject.GetComponentsInChildren<Transform> ();
//		foreach(Transform child in children) {
//			child.gameObject.layer = LayerMask.NameToLayer (layerStr);
//		}
//	}
//
//
//	public void SetUnitActive(bool onOff, GameObject unit = null){
//		if (onOff) {
//			if (_activeUnit) {
//				_activeUnit.GetComponent<UnitScript>().ActivateUnit (false);
//			}
//			//_gameManager._cubeManager.SetCubeActive (false);
//			_activeUnit = unit;
//		} else {
//			_activeUnit = null;
//		}
//	}

//	public void MakeActiveUnitMove(Vector3 vectorToMoveTo, Vector3 offsetPosToMoveTo) {
//		if (_activeUnit) {
//
//			UnitScript unitScript = _activeUnit.GetComponent<UnitScript> ();
//
//			_gameManager._movementManager.SetUnitsPath (_activeUnit, unitScript._unitCanClimbWalls, unitScript.cubeVectorUnitIsOn, vectorToMoveTo, offsetPosToMoveTo);
//		}
//	}
}
