using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour {

    GameManager _gameManager;

    [HideInInspector]
    public MovementManager _movementManager;
    [HideInInspector]
    public CombatManager _combatManager;

	Dictionary<UnitScript, CubeLocationScript> _unitLocation = new Dictionary<UnitScript, CubeLocationScript>();

	void Awake()
    {
        Debug.Log("fucken 1 _movementManager: " + _movementManager);
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

		_movementManager = GetComponentInChildren<MovementManager> ();
		if(_movementManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_combatManager = GetComponentInChildren<CombatManager> ();
		if(_combatManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}


    public void StartGame(Vector3 worldNodeLoc)
    {
        _gameManager._playerManager._unitsAgent.LoadPlayersUnits(worldNodeLoc); // this is not best place for this!! dont like this
    }

    public void SetTurnToMoveUnits()
    {
        _movementManager.MoveUnits();
    }

    public bool SetUnitOnCube(UnitScript unitScript, Vector3 loc)
    {

        CubeLocationScript cubescript = _gameManager._locationManager.CheckIfCanMoveToCube(loc);
        if (cubescript != null)
        {
            if (!_unitLocation.ContainsKey(unitScript))
            {
                _unitLocation.Add(unitScript, cubescript);
            }
            else
            {
                CubeLocationScript oldCubescript = _unitLocation[unitScript];
                oldCubescript._cubeOccupied = true;
                _unitLocation[unitScript] = cubescript;
            }
            cubescript._cubeOccupied = false;
            return true;
        }
        else
        {
            Debug.LogError("Unit cannot move to a location");
            return false;
        }
    }


}
