using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [HideInInspector]
    public PlayerAgent _playerAgent;
    [HideInInspector]
    public CameraAgent _cameraAgent;
    [HideInInspector]
    public UnitsAgent _unitsAgent;


    BasePlayerData _playerData;

    public void LoadPlayerDataInToManager(int playerID)
    {
        BasePlayerData data = null;

        switch (playerID)
        {
            case 0:
                data = new PlayerData_00();
                break;
            case 1:
                data = new PlayerData_01();
                break;
            case 2:
                data = new PlayerData_02();
                break;
            case 3:
                data = new PlayerData_03();
                break;
            default:
                Debug.Log("SOMETHING WENT WRONG HERE");
                break;
        }
        _playerData = data;
    }


    public string GetPlayerName()
    {
        return _playerData.name;
    }

    public List<int[,]> GetPlayerShipSmallFloorDataPART1()
    {
        return _playerData.smallShipFloorsPART1;
    }

    public List<int[,]> GetPlayerShipSmallFloorDataPART2()
    {
        return _playerData.smallShipFloorsPART2;
    }

    public List<int[,]> GetPlayerShipSmallVentDataPART1()
    {
        return _playerData.smallShipVentsPART1;
    }

    public List<int[,]> GetPlayerShipSmallVentDataPART2()
    {
        return _playerData.smallShipVentsPART2;
    }

    public List<Vector3> GetPlayerUnitStartPositions()
    {
        return _playerData.unitStartPositions;
    }
}
