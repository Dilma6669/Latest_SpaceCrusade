using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPieceBuilder : MonoBehaviour {

    GameManager _gameManager;
    LocationManager _locationManager;
    CubeBuilder _cubeBuilder;
    MapSettings _mapSettings;
    PlayerManager _playerManager;


    public List<int[,]> floors = new List<int[,]>();
    public List<int[,]> vents = new List<int[,]>();
    public List<int[,]> floorDataToReturn = new List<int[,]>();
    public List<int[,]> ventDataToReturn = new List<int[,]>();

    private bool loadVents = false;

    private int worldNodeSize = 0;
    private int sizeSquared = 0;
    private int[] neighbours;

    private int[] worldNeighbours;

    int layerCount = -1;

    int MAPTYPE_MAP_FLOOR = 0;
    int MAPTYPE_MAP_VENTS = 1;
    int MAPTYPE_CONNECT_FLOOR = 2;
    int MAPTYPE_CONNECT_VENTS = 3;
    int MAPTYPE_CONNECT_UP_FLOOR = 4;
    int MAPTYPE_CONNECT_UP_VENTS = 5;
    int MAPTYPE_SHIPPORT_FLOOR = 6;
    int MAPTYPE_SHIPPORT_VENTS = 7;

    void Awake() {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _locationManager = transform.parent.GetComponent<LocationManager>();
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    public List<int[,]> GetMapFloorData()
    {
        return floorDataToReturn;
    }

    public List<int[,]> GetMapVentData()
    {
        return ventDataToReturn;
    }

    public void SetWorldNodeNeighboursForDock(int[] worldNodes)
    {
        worldNeighbours = worldNodes;
    }

    public void AttachMapPieceToMapNode<T>(T node, List<Vector3Int> nodes, int _LayerCount, int _worldNodeSize, int _mapType = -1, int _mapPiece = -1, int _rotation = -1) where T : BaseNode
    {
        floorDataToReturn.Clear(); // storing data for serialzatino
        ventDataToReturn.Clear();

        worldNodeSize = _worldNodeSize;
        sizeSquared = (worldNodeSize * worldNodeSize);


        neighbours = node.neighbours;
        layerCount = node.nodeLayerCount;

        for (int j = 0; j < nodes.Count; j++)
        {
            //Debug.Log("fucken layerCount 2<<<<<: " + layerCount);
            BuildMapsByIEnum(node, nodes[j], _mapType, _mapPiece, _rotation);

            layerCount += 1;
        }
    }


    private void BuildMapsByIEnum<T>(T node, Vector3 nodeLoc, int _mapType = -1, int _mapPiece = -1, int _rotation = -1) where T : BaseNode
    {
        int startGridLocX = (int)nodeLoc.x - (_mapSettings.sizeOfMapPiecesXZ / 2);
        int startGridLocY = (int)nodeLoc.y;
        int startGridLocZ = (int)nodeLoc.z - (_mapSettings.sizeOfMapPiecesXZ / 2);

        Vector3 GridLoc;

        List<int[,]> layers = new List<int[,]>();
        int rotations = 0;
        int[,] floor;


        bool floorORRoof = (layerCount % 2 == 0) ? true : false; // floors/Vents
        int mapType;
        int mapPiece;
        int rotation;

        if (floorORRoof) // Floor
        {
            mapType = _mapType;
        }
        else // Roof
        {
            mapType = _mapType + 1;
        }

        // PLACING PLAYERS SHIP //////////////////////////////////
        if (node.playerShipMapPART1 || node.playerShipMapPART2)
        {
            if (floorORRoof)
            {
                if (node.playerShipMapPART1)
                {
                    layers = _playerManager.GetPlayerShipSmallFloorDataPART1();
                }
                else
                {
                    layers = _playerManager.GetPlayerShipSmallFloorDataPART2();
                }
            }
            else
            {
                if (node.playerShipMapPART1)
                {
                    layers = _playerManager.GetPlayerShipSmallVentDataPART1();
                }
                else
                {
                    layers = _playerManager.GetPlayerShipSmallVentDataPART2();
                }
            }
            rotation = 0;
        }/////////////////////////////////////////////////////////////
        else
        {
            mapPiece = (_mapPiece == -1) ? Random.Range(0, 3) : _mapPiece;
            rotation = (_rotation == -1) ? Random.Range(0, 3) : _rotation;

            if (node.entrance)
            {
                KeyValuePair<int, int> mapAndRot = GetShipEntranceMap(_mapPiece);
                mapPiece = mapAndRot.Key;
                rotation = mapAndRot.Value;
            }

            layers = GetMapPiece(mapType, mapPiece);
        }

        rotations = rotation;
        node.nodeRotation = rotation;

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

                /*
                if(floorORRoof) // storing the map data for serilisation and other shit still to work out
                { // have to implement this at some stage
                    floorDataToReturn.Add(floor);
                }
                else
                {
                    ventDataToReturn.Add(floor);
                }
                */
            }

            for (int z = 0; z < floor.GetLength(0); z++)
            {
                objectsCountX = startGridLocX;

                for (int x = 0; x < floor.GetLength(1); x++)
                {
                    int cubeType = floor[z, x];
                    cubeType = FigureOutDoors(node, _mapPiece, _mapType, cubeType, rotations);

                    if (cubeType != 0)
                    {
                        GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);
                        _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount, node.gameObject.transform); // Create the cube
                    }
                    objectsCountX += 1;
                }
                objectsCountZ += 1;
            }
            objectsCountY += 1;
        }
    }

    private KeyValuePair<int, int> GetShipEntranceMap(int mapCount)
    {
        if (mapCount == 0 || mapCount == 9 || mapCount == 18)
        {
            return GetMapPieceAndRotation(mapCount, -1, -1, new int[] { 1, 2 }, new int[] { 0, 0, 0, 3 });
        }
        if (mapCount == 1 || mapCount == 10 || mapCount == 19)
        {
            return GetMapPieceAndRotation(mapCount, 1, 0, new int[] { -1 }, new int[] { -1 });
        }
        if (mapCount == 2 || mapCount == 11 || mapCount == 20)
        {
            return GetMapPieceAndRotation(mapCount, -1, -1, new int[] { 1, 3 }, new int[] { 0, 1, 0, 1 });
        }
        if (mapCount == 3 || mapCount == 12 || mapCount == 21)
        {
            return GetMapPieceAndRotation(mapCount, 2, 3, new int[] { -1 }, new int[] { -1 });
        } 
        if (mapCount == 4) // middle Grnd // PLAYER SHIP IS PLACED HERE AND 13
        {
            return new KeyValuePair<int, int>( 0, 0 );
        }
        if (mapCount == 13 || mapCount == 22) // middle nothing/celing/empty space
        {
            return new KeyValuePair<int, int>(3, 0);
        }
        if (mapCount == 5 || mapCount == 14 || mapCount == 23)
        {
            return GetMapPieceAndRotation(mapCount, 3, 1, new int[] { -1 }, new int[] { -1 });
        }
        if (mapCount == 6 || mapCount == 15 || mapCount == 24)
        {
            return GetMapPieceAndRotation(mapCount, -1, -1, new int[] { 2, 4 }, new int[] { 1, 3, 3, 2 });
        }
        if (mapCount == 7 || mapCount == 16 || mapCount == 25)
        {
            return GetMapPieceAndRotation(mapCount, 4, 2, new int[] { -1 }, new int[] { -1 });
        }
        if (mapCount == 8 || mapCount == 17 || mapCount == 26)
        {
            return GetMapPieceAndRotation(mapCount, -1, -1, new int[] { 3, 4 }, new int[] { 2, 2, 1, 2 });
        }
        Debug.LogError("OPSALA SOMETHING WRONG HERE! mapCount: " + mapCount);
        return new KeyValuePair<int, int>(-1, -1);
    }

    private KeyValuePair<int, int> GetMapPieceAndRotation(int nodeCount, int neigh1, int rot1, int[] neighs2, int[] rots2)
    {
        int mapPiece = -1; // either 0,1,2 : just floor/Floor and wall/floor and corner
        int rotation = -1;
        int var1 = -1; // 0 = no wall, 1 = wall closed door, 
        int var2 = -1; // 0 = no wall, 1 = wall closed door, 

        if (nodeCount == 1 || nodeCount == 3 || nodeCount == 5 || nodeCount == 7) // FOR THE GROUND FLOOR
        {
            // if no world neighbour is present, then just make a floor, else make a wall
            var1 = (worldNeighbours[neigh1] == -1) ? 0 : 1;

            if (var1 == 0)
            {
                mapPiece = 0; // just floor
                rotation = rot1; // does not matter
            }
            else if (var1 == 1)
            {
                mapPiece = 1; // Floor and wall
                rotation = rot1;
            }
            else
            {
                Debug.LogError("OPSALA SOMETHING WRONG HERE! map: ");
            }
        }
        else if (nodeCount == 10 || nodeCount == 12 || nodeCount == 14 || nodeCount == 16 || // FOR THE HIGHER FLOORS
                 nodeCount == 19 || nodeCount == 21 || nodeCount == 23 || nodeCount == 25)
        {
            // if no world neighbour is present, then just make a floor, else make a wall
            var1 = (worldNeighbours[neigh1] == -1) ? 0 : 1;

            if (var1 == 0)
            {
                mapPiece = 3; // just floor
                rotation = rot1; // does not matter
            }
            else if (var1 == 1)
            {
                mapPiece = 4; // Floor and wall
                rotation = rot1;
            }
            else
            {
                Debug.LogError("OPSALA SOMETHING WRONG HERE! map: ");
            }
        }
        else if (nodeCount == 0 || nodeCount == 2 || nodeCount == 6 || nodeCount == 8) // FOR THE GROUND FLOOR
        {
            var1 = (worldNeighbours[neighs2[0]] == -1) ? 0 : 1;
            var2 = (worldNeighbours[neighs2[1]] == -1) ? 0 : 1;


            if (var1 == 0 && var2 == 0)
            {
                mapPiece = 0; // just floor
                rotation = rots2[0]; // does not matter
            }
            else if (var1 == 1 && var2 == 1)
            {
                mapPiece = 2; // corner
                rotation = rots2[1]; 
            }
            else if (var1 == 1 && var2 == 0)
            {
                mapPiece = 1; // wall
                rotation = rots2[2]; 
            }
            else if (var1 == 0 && var2 == 1)
            {
                mapPiece = 1; // wall
                rotation = rots2[3];
            }
            else
            {
                Debug.LogError("OPSALA SOMETHING WRONG HERE! map: ");
            }
        }
        else if (nodeCount == 9 || nodeCount == 11 || nodeCount == 15 || nodeCount == 17 || // FOR THE HIGHER FLOORS
                nodeCount == 18 || nodeCount == 20 || nodeCount == 24 || nodeCount == 26)
        {
            var1 = (worldNeighbours[neighs2[0]] == -1) ? 0 : 1;
            var2 = (worldNeighbours[neighs2[1]] == -1) ? 0 : 1;


            if (var1 == 0 && var2 == 0)
            {
                mapPiece = 3; // just floor
                rotation = rots2[0]; // does not matter
            }
            else if (var1 == 1 && var2 == 1)
            {
                mapPiece = 5; // corner
                rotation = rots2[1];
            }
            else if (var1 == 1 && var2 == 0)
            {
                mapPiece = 4; // wall
                rotation = rots2[2];
            }
            else if (var1 == 0 && var2 == 1)
            {
                mapPiece = 4; // wall
                rotation = rots2[3];
            }
            else
            {
                Debug.LogError("OPSALA SOMETHING WRONG HERE! map: ");
            }
        }

        return new KeyValuePair<int, int>(mapPiece, rotation);
    }




    ///////
    private int FigureOutDoors<T>(T node, int _mapPiece, int _mapType, int _cubeType, int rotations) where T : BaseNode
    {
        int originalCubeType = _cubeType;

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
            if (rotations == 1)
            { 
                if (_cubeType == 51)
                {
                    _cubeType = 53;
                }
                else if (_cubeType == 52)
                {
                    _cubeType = 51;
                }
                else if (_cubeType == 53)
                {
                    _cubeType = 54;
                }
                else if (_cubeType == 54)
                {
                    _cubeType = 52;
                }
            }
            else if (rotations == 2)
            {
                if (_cubeType == 51)
                {
                    _cubeType = 54;
                }
                else if (_cubeType == 52)
                {
                    _cubeType = 53;
                }
                else if (_cubeType == 53)
                {
                    _cubeType = 52;
                }
                else if (_cubeType == 54)
                {
                    _cubeType = 51;
                }
            }
            else if (rotations == 3)
            {
                if (_cubeType == 51)
                {
                    _cubeType = 52;
                }
                else if (_cubeType == 52)
                {
                    _cubeType = 54;
                }
                else if (_cubeType == 53)
                {
                    _cubeType = 51;
                }
                else if (_cubeType == 54)
                {
                    _cubeType = 53;
                }
            }
            int index = _cubeType - 50;

            //Debug.Log("fuceken jezus _cubeType: " + _cubeType);
            //Debug.Log("fuceken jezus index: " + index);

            // stay in here
            if (_mapType == 4) // shipYard
            {
                if (worldNeighbours[index] != -1) // if world neighbour present
                {
                    if (neighbours[index] != -1) // if neighbour present
                    {
                        return 0; // return no wall
                    }
                    else
                    {
                        return cubeTypeAndWallType[originalCubeType]; // return wall
                    }
                }
                else // if world neighbour IS NOT present
                {
                    return cubeTypeAndWallType[originalCubeType]; // return wall
                }
            }
            ///////

            if (neighbours[index] == -1)
            {
                return cubeTypeAndWallType[originalCubeType]; // floor
            }
        }
        return originalCubeType;
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
                        connectFloor = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    case 1:
                        connectFloor = ScriptableObject.CreateInstance<ConnectorPiece_01>();
                        break;
                    case 2:
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
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    case 1:
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    case 2:
                        connectRoof = ScriptableObject.CreateInstance<ConnectorPiece_Roof_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return connectRoof.floors;

            case 4: // Connector Going UP
                BaseMapPiece connectFloorUP = null;
                switch (map)
                {
                    case 0:
                        connectFloorUP = ScriptableObject.CreateInstance<MapPiece_Corridor_Up_01>();
                        break;
                    case 1:
                        connectFloorUP = ScriptableObject.CreateInstance<MapPiece_Corridor_Up_01>();
                        break;
                    case 2:
                        connectFloorUP = ScriptableObject.CreateInstance<MapPiece_Corridor_Up_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return connectFloorUP.floors;

            case 5: // connector Roof Goin UP
                BaseMapPiece connectRoofUP = null;
                switch (map)
                {
                    case 0:
                        connectRoofUP = ScriptableObject.CreateInstance<MapPiece_Vents_Up_01>();
                        break;
                    case 1:
                        connectRoofUP = ScriptableObject.CreateInstance<MapPiece_Vents_Up_01>();
                        break;
                    case 2:
                        connectRoofUP = ScriptableObject.CreateInstance<MapPiece_Vents_Up_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return connectRoofUP.floors;

            case 6: // Ship entrance Floor
                BaseMapPiece garageFloor = null;
                switch (map)
                {
                    case 0:
                     garageFloor = ScriptableObject.CreateInstance<ShipPort_Grnd_Empty_01>();
                        break;
                    case 1:
                     garageFloor = ScriptableObject.CreateInstance<ShipPort_Grnd_Wall_01>();
                        break;
                    case 2:
                        garageFloor = ScriptableObject.CreateInstance<ShipPort_Grnd_Corner_01>();
                        break;
                    case 3:
                        garageFloor = ScriptableObject.CreateInstance<ShipPort_Mid_Empty_01>();
                        break;
                    case 4:
                        garageFloor = ScriptableObject.CreateInstance<ShipPort_Mid_Wall_01>();
                        break;
                    case 5:
                        garageFloor = ScriptableObject.CreateInstance<ShipPort_Mid_Corner_01>();
                        break;
                    default:
                        Debug.LogError("OPSALA SOMETHING WRONG HERE! map: " + map);
                        break;
                }
                return garageFloor.floors;

            case 7: // Ship Entrance roof
                BaseMapPiece garageRoof = null;
                switch (map)
                {
                    case 0:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Empty_Vents_01>();
                        break;
                    case 1:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Wall_Vents_01>();
                        break;
                    case 2:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Corner_Vents_01>();
                        break;
                    case 3:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Empty_Vents_01>();
                        break;
                    case 4:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Wall_Vents_01>();
                        break;
                    case 5:
                        garageRoof = ScriptableObject.CreateInstance<ShipPort_Corner_Vents_01>();
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
