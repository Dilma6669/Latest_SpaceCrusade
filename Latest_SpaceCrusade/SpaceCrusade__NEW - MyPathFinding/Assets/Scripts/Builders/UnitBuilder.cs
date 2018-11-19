using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuilder : MonoBehaviour {

    public List<GameObject> _unitPrefabs;



    public GameObject GetUnitModel(int choice)
    {
        return _unitPrefabs[choice];
    }
}
