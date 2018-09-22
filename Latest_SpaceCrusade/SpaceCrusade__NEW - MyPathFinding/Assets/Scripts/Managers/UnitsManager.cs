using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {

//	public static UnitsManager instance = null;
//
//	public GamePlayManager _gamePlayManager;
//	public LocationManager _locationManager;
//
//	public List<UnitsAgent> _unitAgents = new List<UnitsAgent> ();
//
//	void Awake() {
//
//		if (instance == null)
//			instance = this;
//		else if (instance != this) {
//			Debug.LogError ("OOPSALA we have an ERROR! More than one instance bein created");
//			Destroy (gameObject);
//		}
//
//		_gamePlayManager = transform.parent.GetComponentInChildren<GamePlayManager> ();
//		if(_gamePlayManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
//
//		_locationManager = transform.parent.GetComponentInChildren<LocationManager> ();
//		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
//	}
//
//	public void LoadPlayersUnitAgents() {
//
//		List<Vector3> player01UnitsPositions = new List<Vector3> ()
//		{
//			new Vector3(10,1,0),
//			//new Vector3(12,1,0),
//			new Vector3(10,1,2),
//			//new Vector3(12,1,2)
//		};
//
//		List<Vector3> player02UnitsPositions = new List<Vector3> ()
//		{
//			new Vector3(10,1,20),
//			new Vector3(12,1,20),
//			//new Vector3(10,1,22),
//		//	new Vector3(12,1,22)
//		};
//
//		List<List<Vector3>> playerList = new List<List<Vector3>> () {
//			player01UnitsPositions,
//			player02UnitsPositions
//		};
//
//		int count = 0;
//		foreach(UnitsAgent agent in _unitAgents)
//		{
//			if (!agent.LoadPlayersUnits (playerList[count])) {
//				Debug.LogError ("ERROR A unit couldnt be loaded!");
//			}
//			agent.AssignUniqueLayerToUnits ();
//			count += 1;
//		}
//
//	}
//
//
//	public bool SetUnitOnCube(UnitScript unitscript, Vector3 startingLoc) {
//
//		return _gamePlayManager.SetUnitOnCube (unitscript, startingLoc);
//	}
		



}
