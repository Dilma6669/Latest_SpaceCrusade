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
        List<UnitData> units = _gameManager._playerManager.GetPlayerUnitData();

        foreach (UnitData unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldNodeLoc.x, localStart.y + worldNodeLoc.y, localStart.z + worldNodeLoc.z);

            if (CreateUnitOnNetwork(unit, worldStart))
            {
                // AssignUniqueLayerToUnits();
            }
            else
            {
                Debug.LogError("Cant Place Unit on startUp: " + worldStart);
            }
        }
    }


    private bool CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart)
    {
        CubeLocationScript cubeScript = _gameManager._locationManager.CheckIfCanMoveToCube(worldStart);

        if (cubeScript != null)
        {
            int playerID = _gameManager._playerManager.GetPlayerID();
            _gameManager._playerManager._playerObject.GetComponent<NetworkAgent>().CmdTellServerToSpawnPlayerUnit(unitData, playerID , worldStart);
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
