﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UnitsAgent : NetworkBehaviour {

    GameManager _gameManager;

    NetworkManager _networkManager;

    PlayerManager _playerManager;


    public GameObject _activeUnit = null;

    public List<GameObject> unitObjects = new List<GameObject>();
    public List<UnitScript> unitScripts = new List<UnitScript>();

    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }



    public void SetUnitActive(bool onOff, GameObject unit = null)
    {
        if (onOff)
        {
            if (_activeUnit)
            {
                _activeUnit.GetComponent<UnitScript>().ActivateUnit(false);
            }
            //_gameManager._cubeManager.SetCubeActive (false);
            _activeUnit = unit;
        }
        else
        {
            _activeUnit = null;
        }
    }

    public void MakeActiveUnitMove(Vector3 vectorToMoveTo, Vector3 offsetPosToMoveTo)
    {
        if (_activeUnit)
        {
            UnitScript unitScript = _activeUnit.GetComponent<UnitScript>();
            _gameManager._gamePlayManager._movementManager.SetUnitsPath(_activeUnit, unitScript.UnitCanClimbWalls, unitScript.CubeUnitIsOn.cubeLoc, vectorToMoveTo, offsetPosToMoveTo);
        }
    }



    public void SetUpUnitForPlayer(GameObject unit)
    {
        if (isLocalPlayer)
        {
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            unitScript.CubeUnitIsOn = _gameManager._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
            unitScript.PlayerControllerID = GetComponent<PlayerAgent>().PlayerID;
        }
    }


}
