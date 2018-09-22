using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour {

	private static GamePlayManager instance = null;

	protected LocationManager _locationManager;
	protected MovementManager _movementManager;
	protected CombatManager _combatManager;
	protected NetWorkManager _networkManager;

	Dictionary<UnitScript, CubeLocationScript> _unitLocation = new Dictionary<UnitScript, CubeLocationScript>();

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this) {
			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
			Destroy (gameObject);
		}

		_locationManager = transform.parent.GetComponentInChildren<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_movementManager = GetComponentInChildren<MovementManager> ();
		if(_movementManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_combatManager = GetComponentInChildren<CombatManager> ();
		if(_combatManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_networkManager = transform.parent.GetComponentInChildren<NetWorkManager> ();
		if(_networkManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}




	public bool SetUnitOnCube(UnitScript unitScript, Vector3 loc) {
		
		CubeLocationScript cubescript = _locationManager.CheckIfCanMoveToCube (loc);
		if (cubescript != null) {
			if (!_unitLocation.ContainsKey (unitScript)) {
				_unitLocation.Add (unitScript, cubescript);
			} else {
				CubeLocationScript oldCubescript = _unitLocation [unitScript];
				oldCubescript._cubeOccupied = true;
				_unitLocation [unitScript] = cubescript;
			}
			cubescript._cubeOccupied = false;
			return true;
		} else {
			Debug.LogError ("Unit cannot move to a location");
			return false;
		}
		Debug.LogError ("OOPSALA we have an ERROR!");
		return false;
	}

}
