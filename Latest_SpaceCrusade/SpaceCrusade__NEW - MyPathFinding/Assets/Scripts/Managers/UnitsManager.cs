using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {

    GameManager _gameManager;


    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public bool SetUnitOnCube(UnitScript unitscript, Vector3 startingLoc)
    {
        return _gameManager._gamePlayManager.SetUnitOnCube(unitscript, startingLoc);
    }

}
