using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPieceBuilder: MonoBehaviour {

	LocationManager _locationManager;
	CubeBuilder _cubeBuilder;
	MapSettings _mapSettings;


	public List<int[,]> floors = new List<int[,]>();
	public List<int[,]> vents = new List<int[,]>();

	private bool loadVents = false;

    private int worldNodeSize = 0;
    private int sizeSquared = 0;

    int layerCount = -1;

    void Awake() {

		_locationManager = transform.parent.GetComponent<LocationManager> ();
		if(_locationManager == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder> ();
		if(_cubeBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = transform.parent.GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}

    public void AttachMapPieceToMapNode(Transform parent, List<Vector3Int> nodes, int _LayerCount, int _worldNodeSize, int _mapType = -1, int _rotation = -1)
    {

        worldNodeSize = _worldNodeSize;
        sizeSquared = (worldNodeSize * worldNodeSize);

        layerCount = _LayerCount;

        for (int j = 0; j < nodes.Count; j++)
        {
            Debug.Log("fucken layerCount 2<<<<<: " + layerCount);
            BuildMapsByIEnum(parent, nodes[j], j, _mapType, _rotation);

            layerCount += 1;
        }
    }


    private void BuildMapsByIEnum(Transform parent, Vector3 nodeLoc, int j, int _mapType = -1, int _rotation = -1)
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

        int mapPiece = Random.Range(1, 4); //Map pieces // 0 = Entrance so dont use here
        int rotation = (_rotation == -1) ? Random.Range(0, 4) : _rotation;
        
        if(rotation == 4)
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
            }

            for (int z = 0; z < floor.GetLength(0); z++)
            {
                objectsCountX = startGridLocX;

                for (int x = 0; x < floor.GetLength(1); x++)
                {
                    int cubeType = floor[z, x];
                    GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);

                    Debug.Log("fucken CreateCubeObject: " + parent);
                    _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount, parent); // Create the cube

                    objectsCountX += 1;
                }
                objectsCountZ += 1;
            }
            objectsCountY += 1;
        }
    }



	// Get map by type and piece (NOTE: at the moment needs to be same amount of case's in each type)
	private List<int[,]> GetMapPiece(int type, int map) {

		switch (type) { 
		case 0: // Floor
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

		case 1:
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

         case 2:
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

           case 3:
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
		default:
			Debug.LogError ("OPSALA SOMETHING WRONG HERE!");
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
