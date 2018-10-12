using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPieceBuilder : MonoBehaviour {

    LocationManager _locationManager;
    CubeBuilder _cubeBuilder;
    MapSettings _mapSettings;


    public List<int[,]> floors = new List<int[,]>();
    public List<int[,]> vents = new List<int[,]>();
    public List<int[,]> floorDataToReturn = new List<int[,]>();
    public List<int[,]> ventDataToReturn = new List<int[,]>();

    private bool loadVents = false;

    private int worldNodeSize = 0;
    private int sizeSquared = 0;
    private int[] neighbours;

    int layerCount = -1;

    void Awake() {

        _locationManager = transform.parent.GetComponent<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    public List<int[,]> GetMapFloorData()
    {
        return floorDataToReturn;
    }

    public List<int[,]> GetMapVentData()
    {
        return ventDataToReturn;
    }

    public void AttachMapPieceToMapNode(Transform parent, List<Vector3Int> nodes, int[] neighs, int _LayerCount, int _worldNodeSize, int _mapType = -1, int _mapPiece = -1, int _rotation = -1)
    {
        floorDataToReturn.Clear(); // storing data for serialzatino
        ventDataToReturn.Clear();

        worldNodeSize = _worldNodeSize;
        sizeSquared = (worldNodeSize * worldNodeSize);
        neighbours = neighs;

        layerCount = _LayerCount;

        for (int j = 0; j < nodes.Count; j++)
        {
            //Debug.Log("fucken layerCount 2<<<<<: " + layerCount);
            BuildMapsByIEnum(parent, nodes[j], j, _mapType, _mapPiece, _rotation);

            layerCount += 1;
        }
    }


    private void BuildMapsByIEnum(Transform parent, Vector3 nodeLoc, int j, int _mapType = -1, int _mapPiece = -1, int _rotation = -1)
    {
        int startGridLocX = (int)nodeLoc.x - (_mapSettings.sizeOfMapPiecesXZ / 2);
        int startGridLocY = (int)nodeLoc.y;
        int startGridLocZ = (int)nodeLoc.z - (_mapSettings.sizeOfMapPiecesXZ / 2);

        Vector3 GridLoc;

        List<int[,]> layers = new List<int[,]>();
        int[,] floor;


        bool floorORRoof = (layerCount % 2 == 0) ? true : false; // floors/Vents
        int mapPieceType;

        if (floorORRoof) // Floor
        {
            mapPieceType = _mapType;
        }
        else // Roof
        {
            mapPieceType = _mapType + 1;
        }

        int mapPiece = (_mapPiece == -1) ? Random.Range(1, 4) : _mapPiece; //Map pieces // 0 = Entrance so dont use here
        int rotation = (_rotation == -1) ? Random.Range(0, 4) : _rotation;

        if (rotation == 4)
        {
            mapPiece = 0; //ConnectorPiece UP not sure if going to work
        }

        layers = GetMapPiece(mapPieceType, mapPiece);
        int rotations = rotation;

        int objectsCountX = startGridLocX;
        int objectsCountY = startGridLocY;
        int objectsCountZ = startGridLocZ;

        for (int y = 0; y < layers.Count; y++)
        {

            objectsCountX = startGridLocX;
            objectsCountZ = startGridLocZ;

            floor = layers[y];

            for (int r = 0; r < rotations; r++)
            {
                floor = TransposeArray(floor, _mapSettings.sizeOfMapPiecesXZ - 1);

                if(floorORRoof) // storing the map data for serilisation and other shit still to work out
                {
                    floorDataToReturn.Add(floor);
                }
                else
                {
                    ventDataToReturn.Add(floor);
                }
            }

            for (int z = 0; z < floor.GetLength(0); z++)
            {
                objectsCountX = startGridLocX;

                for (int x = 0; x < floor.GetLength(1); x++)
                {
                    int cubeType = floor[z, x];
                    cubeType = FigureOutDoors(_mapType, cubeType, rotations);

                    if (cubeType != 0)
                    {
                        GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);
                        _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount, parent); // Create the cube
                    }
                    objectsCountX += 1;
                }
                objectsCountZ += 1;
            }
            objectsCountY += 1;
        }
    }


    ///////
    private int FigureOutDoors(int mapType, int _cubeType, int rotations)
    {
        Dictionary<int, int> cubeTypeAndWallType = new Dictionary<int, int>()
        {
            { 50, 1 },
            { 51, 3 },
            { 52, 2 },
            { 53, 2 },
            { 54, 3 },
            { 55, 1 }
        };

        if (cubeTypeAndWallType.ContainsKey(_cubeType))
        {
            int index = _cubeType - 50;

            if (mapType == 4) // shipYard
            {
                if (neighbours[index] != -1) // if neighbour present
                {
                    return cubeTypeAndWallType[_cubeType];
                }
                else
                {
                    if(index == 0) // if floor still make panel
                    {
                        return cubeTypeAndWallType[_cubeType];
                    }
                    return _cubeType; // if no neighbour return nothing
                }
            }
            if (neighbours[index] == -1)
            {
                return cubeTypeAndWallType[_cubeType]; // floor
            }
        }
        return _cubeType;
    }



    // Get map by type and piece (NOTE: at the moment needs to be same amount of case's in each type)
    private List<int[,]> GetMapPiece(int type, int map)
    {
        switch (type)
        {
            case 0: // MAp Floor
                BaseMapPiece mapPiece = null;
                switch (map)
                {
                    case 0:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Entrance_01>();
                        break;
                    case 1:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Corridor_01>();
                        break;
                    case 2:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Room_01>();
                        break;
                    case 3:
                        mapPiece = ScriptableObject.CreateInstance<MapPiece_Room_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                        break;
                }
                return mapPiece.floors;

            case 1: // Map Roof
                BaseMapPiece ventPiece = null;
                switch (map)
                {
                    case 0:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 1:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 2:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    case 3:
                        ventPiece = ScriptableObject.CreateInstance<MapPiece_Vents_Room_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                        break;
                }
                return ventPiece.floors;

            case 2: // Connector FLoor
                BaseMapPiece connectFloor = null;
                switch (map)
                {
                    case 0:
                        connectFloor = ScriptableObject.CreateInstance<MapPiece_Corridor_Up_01>();
                        break;
                    case 1:
                        connectFloor = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    case 2:
                        connectFloor = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    case 3:
                        connectFloor = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return connectFloor.floors;

            case 3: // connector Roof
                BaseMapPiece connectRoof = null;
                switch (map)
                {
                    case 0:
                        connectRoof = ScriptableObject.CreateInstance<MapPiece_Vents_Up_01>();
                        break;
                    case 1:
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    case 2:
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    case 3:
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return connectRoof.floors;

            case 4: // Ship entrance Floor
                BaseMapPiece garageFloor = null;
                switch (map)
                {
                    case 0:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_LFG_01>();
                        break;
                    case 1:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MFG_01>();
                        break;
                    case 2:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_RFG_01>();
                        break;
                    case 3:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_LCG_01>();
                        break;
                    case 4:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCG_01>();
                        break;
                    case 5:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_RCG_01>();
                        break;
                    case 6:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_LBG_01>();
                        break;
                    case 7:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MBG_01>();
                        break;
                    case 8:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_RBG_01>();
                        break;
                    case 9:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 10:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MFI_01>();
                        break;
                    case 11:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 12:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_LCI_01>();
                        break;
                    case 13:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 14:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_RCI_01>();
                        break;
                    case 15:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 16:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MBI_01>();
                        break;
                    case 17:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 18:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 19:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 20:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 21:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 22:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 23:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 24:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 25:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    case 26:
                        garageFloor = ScriptableObject.CreateInstance<MapLarge_MCI_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return garageFloor.floors;

            case 5: // Ship Entrance roof
                BaseMapPiece garageRoof = null;
                switch (map)
                {
                    case 0:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_LFG_Vents_01>();
                        break;
                    case 1:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MFG_Vents_01>();
                        break;
                    case 2:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_RFG_Vents_01>();
                        break;
                    case 3:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_LCG_Vents_01>();
                        break;
                    case 4:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCG_Vents_01>();
                        break;
                    case 5:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_RCG_Vents_01>();
                        break;
                    case 6:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_LBG_Vents_01>();
                        break;
                    case 7:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MBG_Vents_01>();
                        break;
                    case 8:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_RBG_Vents_01>();
                        break;
                    case 9:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 10:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MFI_Vents_01>();
                        break;
                    case 11:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 12:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_LCI_Vents_01>();
                        break;
                    case 13:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 14:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_RCI_Vents_01>();
                        break;
                    case 15:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 16:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MBI_Vents_01>();
                        break;
                    case 17:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 18:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 19:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 20:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 21:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 22:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 23:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 24:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 25:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    case 26:
                        garageRoof = ScriptableObject.CreateInstance<MapLarge_MCI_Vents_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return garageRoof.floors;

            default:
                Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                return null;
        }
    }




	private int[,] TransposeArray(int[,] array, int size) {

		int[,] ret = new int[size, size];

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				ret [i, j] = array [size - j - 1, i];
			}
		}

		return ret;
	}
		
}
