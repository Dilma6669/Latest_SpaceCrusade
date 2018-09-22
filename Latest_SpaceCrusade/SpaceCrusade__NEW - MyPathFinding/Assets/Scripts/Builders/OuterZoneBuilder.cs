using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterZoneBuilder : MonoBehaviour {

    public static OuterZoneBuilder instance = null;

    public GameObject outerZonePrefab;

    MapSettings _mapSettings;

    Vector3Int lowestXpos;
    Vector3Int highestXpos;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.LogError("OOPSALA we have an ERROR! More than one instance bein created");
            Destroy(gameObject);
        }

        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public void CreateOuterZoneForNode(WorldNode node)
    {
        Vector3Int centalVect = node.nodeLocation;

        BuildRestOfOuterZones(centalVect);
    }

    public void BuildRestOfOuterZones(Vector3Int centalVect)
    {
        int spreadDistanceX = 10;
        int spreadDistanceY = 10;

        int multiplierX = _mapSettings.sizeOfMapPiecesXZ * 3; // 3 is size of world nodes 3 is max at mo
        int multiplierY = ((_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY) * 3); // 1 is size of world nodes 3 is max at mo

        int startX = centalVect.x - (spreadDistanceX* multiplierX) - 1; // -1 to make line up properly (not sure exactly)

        int currX = startX;
        int currZ = centalVect.z - 1; // -1 to make line up properly (not sure exactly)
        int currY = centalVect.y - (spreadDistanceY * multiplierY);

        for (int y = 0; y < spreadDistanceY*2; y++)
        {
            for (int x = 0; x < spreadDistanceX*2; x++)
            {
                Vector3Int vect = new Vector3Int(currX, currY, currZ);

                GameObject outerZoneObject = Instantiate(outerZonePrefab, this.transform, false); // empty cube
                outerZoneObject.transform.SetParent(this.transform);
                outerZoneObject.transform.position = vect;
                outerZoneObject.transform.localScale = new Vector3Int(multiplierX, multiplierY, multiplierX);

                currX += multiplierX;
            }
            currX = startX;
            currY += multiplierY;
        }
    }
}
