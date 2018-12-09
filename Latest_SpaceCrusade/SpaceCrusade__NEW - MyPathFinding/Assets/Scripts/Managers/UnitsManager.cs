using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour {

    GameManager _gameManager;

    public UnitBuilder _unitBuilder;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _unitBuilder = GetComponentInChildren<UnitBuilder>();
        if (_unitBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public bool SetUnitOnCube(UnitScript unitscript, Vector3 startingLoc)
    {
        return _gameManager._gamePlayManager.SetUnitOnCube(unitscript, startingLoc);
    }


    public void LoadPlayersUnits(Vector3 worldNodeLoc)
    {
        List<UnitScript> units = _gameManager._playerManager.GetPlayerUnitScripts();

        foreach (UnitScript unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldNodeLoc.x, localStart.y + worldNodeLoc.y, localStart.z + worldNodeLoc.z);
            unit.UnitStartingWorldLoc = worldStart;
            if (CreateUnitOnNetwork(unit))
            {
                // AssignUniqueLayerToUnits();
            }
            else
            {
                Debug.LogError("Cant Place Unit on startUp: " + worldStart);
            }
        }
    }


    private bool CreateUnitOnNetwork(UnitScript unit)
    {
        CubeLocationScript cubeScript = _gameManager._locationManager.CheckIfCanMoveToCube(unit.UnitStartingWorldLoc);

        if (cubeScript != null)
        {
            int unitModel = unit.UnitModel;
            bool unitCanClimbwalls = unit.UnitCanClimbWalls; 
            Vector3 startLocation = unit.UnitStartingWorldLoc;
            int[] unitCombatStats = unit.UnitCombatStats;
            _gameManager._playerManager._playerObject.GetComponent<NetworkAgent>().TellServerToSpawnPlayerUnit(unitModel, unitCanClimbwalls, startLocation, unitCombatStats);
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
    public void AssignUniqueLayerToUnits()
    {
        string layerStr = "Player0" + _gameManager._playerManager._playerAgent.PlayerUniqueID.ToString() + "Units";
        gameObject.layer = LayerMask.NameToLayer(layerStr);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerStr);
        }
    }
    */


}
