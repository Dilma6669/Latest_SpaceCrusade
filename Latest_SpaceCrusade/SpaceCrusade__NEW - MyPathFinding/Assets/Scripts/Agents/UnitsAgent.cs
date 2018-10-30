using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UnitsAgent : NetworkBehaviour {

    GameManager _gameManager;


    public GameObject _unitPrefab;

    public GameObject _activeUnit = null;

    public List<GameObject> unitObjects = new List<GameObject>();
    public List<UnitScript> unitScripts = new List<UnitScript>();

    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _gameManager._playerManager._unitsAgent = this;
    }

    public void LoadPlayersUnits(Vector3 worldNodeLoc)
    {
        List<Vector3> unitsLocations = _gameManager._playerManager.GetPlayerUnitStartPositions();

        foreach (Vector3 loc in unitsLocations)
        {
            Vector3 location = new Vector3(loc.x + worldNodeLoc.x, loc.y + worldNodeLoc.y, loc.z + worldNodeLoc.z);
            if (CreateUnit(location))
            {
               // AssignUniqueLayerToUnits();
            }
            else
            {
                Debug.LogError("Cant Place Unit on startUp: " + location);
            }
        }
    }


    private bool CreateUnit(Vector3 startingLoc)
    {
        CubeLocationScript cubeScript = _gameManager._locationManager.CheckIfCanMoveToCube(startingLoc);

        if (cubeScript != null)
        {
         //   var unit = Instantiate(_unitPrefab, _gameManager._unitsManager.gameObject.transform, false);

            _gameManager._networkManager._syncedVars.CmdTellServerToSpawnPlayerUnits(_unitPrefab, startingLoc);

          /*  unit.transform.SetParent(_gameManager._unitsManager.gameObject.transform);
            UnitScript unitscript = unit.gameObject.GetComponent<UnitScript>();
            unitScripts.Add(unitscript);

           // unit.transform.position = startingLoc;
           unitscript._cubeUnitIsOn = cubeScript;
           */
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetUpUnitForPlayer(GameObject unit)
    {
        Debug.Log("fucken unit 3: " + unit);
    }


    public void AssignUniqueLayerToUnits()
    {
        string layerStr = "Player0" + _gameManager._playerManager._playerAgent._playerUniqueID.ToString() + "Units";
        gameObject.layer = LayerMask.NameToLayer(layerStr);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerStr);
        }
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
            _gameManager._gamePlayManager._movementManager.SetUnitsPath(_activeUnit, unitScript._unitCanClimbWalls, unitScript._cubeUnitIsOn.cubeLoc, vectorToMoveTo, offsetPosToMoveTo);
        }
    }
}
