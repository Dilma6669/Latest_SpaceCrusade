using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

    MapSettings _mapSettings;
    NodeBuilder _nodeBuilder;


    public bool _debugGridObjects; // debugging purposes
    public bool _debugNodeSpheres = false;


	public Dictionary<Vector3, CubeLocationScript> _GridLocToScriptLookup; 	// making a lookUp table for objects located at Vector3 Grid locations

	private List<Vector3Int> _GridNodePositions;

    public static GridBuilder instance = null;

    private int worldNodeSize = 0;

    void Awake() {

        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.LogError("OOPSALA we have an ERROR! More than one instance bein created");
            Destroy(gameObject);
        }

        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = transform.parent.GetComponentInChildren<NodeBuilder>();
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _GridLocToScriptLookup = new Dictionary<Vector3, CubeLocationScript>();
    }




	public Dictionary <Vector3, CubeLocationScript> GetGridLocations() {
		return _GridLocToScriptLookup;
	}

	public List<Vector3Int> GetGridNodePositions() {
		return _GridNodePositions;
	}



    public void BuildLocationGrid(Vector3 mapNode, int _worldNodeSize)
    {
        worldNodeSize = _worldNodeSize;

        _GridNodePositions = new List<Vector3Int>();

        // these are the bottom left corner axis for EACH map node
        int startGridLocX = (int)mapNode.x - (_mapSettings.sizeOfMapPiecesXZ / 2);
        int startGridLocY = (int)mapNode.y - (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY) / 2;
        int startGridLocZ = (int)mapNode.z - (_mapSettings.sizeOfMapPiecesXZ / 2);

        BuildGridLocations(startGridLocX, startGridLocY, startGridLocZ);

    }



	public void BuildGridLocations(int startX, int startY, int startZ) {

        int gridLocX = startX;
		int gridLocY = startY;
		int gridLocZ = startZ;

        int finishX = startX + 24;
        int finishY = startY + 6;
        int finishZ = startZ + 24;

		// Floors layer
		for (int y = startY; y < finishY; y++) { // this needs attention!!!!

			gridLocX = startX;
			gridLocZ = startZ;

            for (int z = startZ; z < finishZ; z++) {

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

                    // Create location positions
                    ///////////////////////////////
                    // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                    Vector3Int gridLoc = new Vector3Int(gridLocX, gridLocY, gridLocZ);

                    // Create empty objects at locations to see the locations (debugging purposes)
                    if (_debugGridObjects)
                    {
                        MakeDebugObject(gridLoc);
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLoc, startY);

                    gridLocX += 1;
				}
				gridLocZ += 1;
			}
			gridLocY += 1;
		}

   
		// Vents layer
		// An attempt to build the vents layer //seems to be working
		gridLocX = startX;
		//gridLocY = gridLocY;
		gridLocZ = startZ;

        startY += _mapSettings.sizeOfMapPiecesY;

		for (int y = startY; y < (startY + _mapSettings.sizeOfMapVentsY) ; y++) {

			gridLocX = startX;
			gridLocZ = startZ;

			for (int z = startZ; z < finishZ; z++) {

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

                    // Create location positions
                    ///////////////////////////////
                    // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                    Vector3Int gridLoc = new Vector3Int(gridLocX, gridLocY, gridLocZ);

                    // Create empty objects at locations to see the locations (debugging purposes)
                    if (_debugGridObjects)
                    {
                        MakeDebugObject(gridLoc);
                    }

                    // Adds null script for optimization
                    _GridLocToScriptLookup.Add(gridLoc, null);

                    // node objects are spawned at bottom corner each map piece
                    MakeMapNodeObject(gridLoc, startY);

					gridLocX += 1;
				}
				gridLocZ += 1;
			}
			gridLocY += 1;
		}
    }


    // Create empty objects at locations to see the locations (debugging purposes)
    private void MakeDebugObject(Vector3Int vect)
    {
        GameObject node = _nodeBuilder.InstantiateNodeObject(vect, NodeTypes.GridNode, this.transform);
    }

    // node objects are spawned at bottom corner each map piece
    private void MakeMapNodeObject(Vector3Int vect, int startY)
    {
        //////////////////////////////////////////
        int multiple = (worldNodeSize * _mapSettings.sizeOfMapPiecesXZ) / worldNodeSize;

        if (vect.x % multiple == 0 && vect.z % multiple == 0 && vect.y == startY)
        {
            GameObject node = _nodeBuilder.InstantiateNodeObject(vect, NodeTypes.MapNode, this.transform);
            _GridNodePositions.Add(vect);
        }
        /////////////////////////////////////////////
    }


}
	

