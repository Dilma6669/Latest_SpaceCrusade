using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public BasePlayerData GetPlayerData(int playerID)
    {
        switch (playerID)
        {
            case 0:
                return new PlayerData_00();
            case 1:
                return new PlayerData_01();
            case 2:
                return new PlayerData_02();
            case 3:
                return new PlayerData_03();
            default:
                Debug.Log("SOMETHING WENT WRONG HERE");
                break;
        }
        return null;
    }
}
