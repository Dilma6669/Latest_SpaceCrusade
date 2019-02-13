using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UnitsManager _instance;

    ////////////////////////////////////////////////

    public UnitBuilder _unitBuilder;

    ////////////////////////////////////////////////

    GameManager _gameManager;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        if (_unitBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void LoadPlayersUnits(Vector3 worldNodeLoc)
    {
        List<UnitData> units = _gameManager._playerManager.PlayerUnitData;

        foreach (UnitData unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldNodeLoc.x, localStart.y + worldNodeLoc.y, localStart.z + worldNodeLoc.z);

            CreateUnitOnNetwork(unit, worldStart);
        }
    }


    private void CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart)
    {
        int playerID = _gameManager._playerManager.PlayerID;
        _gameManager._playerManager.PlayerObject.GetComponent<NetworkAgent>().CmdTellServerToSpawnPlayerUnit(unitData, playerID, worldStart);
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
