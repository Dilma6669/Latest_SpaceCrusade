using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

    GameManager _gameManager;

    MapSettings _mapSettings;
    NodeBuilder _nodeBuilder;


    public bool _debugGridObjects; // debugging purposes
    public bool _debugNodeSpheres = false;

    private BaseNode _node;

    private Dictionary<Vector3, CubeLocationScript> _GridLocToScriptLookup; 	// making a lookUp table for objects located at Vector3 Grid locations

	private List<Vector3> _GridNodePositions;

    private int _worldNodeSize = 0;

    void Awake() {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapSettings = _gameManager._worldManager._mapSettings;
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = _gameManager._worldManager._nodeBuilder;
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _GridLocToScriptLookup = new Dictionary<Vector3, CubeLocationScript>();
    }

    public void SetCubeScriptToGridLocation(Vector3 vect, CubeLocationScript script)
    {
        if (_GridLocToScriptLookup.ContainsKey(vect))
        {
            if (_GridLocToScriptLookup[vect] == null)
            {
                //Debug.Log("fucken adding to vect: " + vect + " script: " + script);
                _GridLocToScriptLookup[vect] = script;
            }
            else
            {
                Debug.LogError("GOT a same location trying to have script assigned too");
            }
        }
        else
        {
            Debug.LogError("_GridLocToScriptLookup.ContainsKey(vect) does not exist");
        }
    }

    public CubeLocationScript GetGridLocationScript(Vector3 vect)
    {
        return _GridLocToScriptLookup[vect];
    }

    public Dictionary <Vector3, CubeLocationScript> GetGridLocations() {
		return _GridLocToScriptLookup;
	}

	public List<Vector3> GetGridNodePositions() {
		return _GridNodePositions;
	}



    public void BuildLocationGrid<T>(T node, int worldNodeSize) where T : BaseNode
    {
        _node = node as T;

        _worldNodeSize = worldNodeSize;


        _GridNodePositions = new List<Vector3>();
        _GridLocToScriptLookup = new Dictionary<Vector3, CubeLocationScript>();

        // these are the bottom left corner axis for EACH map node
        int startGridLocX = (int)node.nodeLocation.x - (_mapSettings.sizeOfMapPiecesXZ / 2);
        int startGridLocY = (int)node.nodeLocation.y - (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY) / 2;
        int startGridLocZ = (int)node.nodeLocation.z - (_mapSettings.sizeOfMapPiecesXZ / 2);

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
                    Vector3 gridLoc = new Vector3(gridLocX, gridLocY, gridLocZ);

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

        int offset = 0;

        if (_node.neighbours[5] == -1) // for the roofs of the vents only appearing if no map piece above vent
        {
            offset = 1;
        }

        for (int y = startY; y < (startY + _mapSettings.sizeOfMapVentsY) + offset; y++) {

			gridLocX = startX;
			gridLocZ = startZ;

			for (int z = startZ; z < finishZ; z++) {

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

                    // Create location positions
                    ///////////////////////////////
                    // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                    Vector3 gridLoc = new Vector3(gridLocX, gridLocY, gridLocZ);

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
    private void MakeDebugObject(Vector3 vect)
    {
        GameObject node = _nodeBuilder.InstantiateNodeObject(vect, NodeTypes.GridNode, this.transform);
    }

    // node objects are spawned at bottom corner each map piece
    private void MakeMapNodeObject(Vector3 vect, int startY)
    {
        //////////////////////////////////////////
        int multiple = (_worldNodeSize * _mapSettings.sizeOfMapPiecesXZ) / _worldNodeSize;

        if (vect.x % multiple == 0 && vect.z % multiple == 0 && vect.y == startY)
        {
            GameObject node = _nodeBuilder.InstantiateNodeObject(vect, NodeTypes.MapNode, this.transform);
            _GridNodePositions.Add(vect);
        }
        /////////////////////////////////////////////
    }


}
	

