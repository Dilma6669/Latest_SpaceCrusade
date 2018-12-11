using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocationManager : MonoBehaviour {

    GameManager _gameManager;

    [HideInInspector]
    public WorldBuilder     _worldBuilder;
    [HideInInspector]
    public GridBuilder      _gridBuilder;
    [HideInInspector]
    public CubeBuilder      _cubeBuilder;
    [HideInInspector]
    public MapPieceBuilder  _mapPieceBuilder;
    [HideInInspector]
    public OuterZoneBuilder _outerZoneBuilder;
    [HideInInspector]
    public PlayerShipBuilder _playerShipBuilder;
    [HideInInspector]
    public NodeBuilder      _nodeBuilder;

    [HideInInspector]
    public CubeConnections  _cubeConnections;
    [HideInInspector]
    public MapSettings      _mapSettings;



    public List<WorldNode> _worldNodes;

    public Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();

    public CubeLocationScript _activeCube = null; // hmmm dont know if should be here

    void Awake() {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _worldBuilder = GetComponentInChildren<WorldBuilder>();
        if (_worldBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _gridBuilder = GetComponentInChildren<GridBuilder> ();
		if(_gridBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _cubeBuilder = GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapPieceBuilder = GetComponentInChildren<MapPieceBuilder> ();
		if(_mapPieceBuilder == null){Debug.LogError ("OOPSALA we have an ERROR!");}

        _outerZoneBuilder = GetComponentInChildren<OuterZoneBuilder>();
        if (_outerZoneBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = GetComponentInChildren<NodeBuilder>();
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }



        _cubeConnections = GetComponent<CubeConnections> ();
		if(_cubeConnections == null){Debug.LogError ("OOPSALA we have an ERROR!");}

		_mapSettings = GetComponent<MapSettings> ();
		if(_mapSettings == null){Debug.LogError ("OOPSALA we have an ERROR!");}

	}



    private void AddGridLocationsToMap(Dictionary<Vector3, CubeLocationScript> locs)
    {
        foreach (Vector3 vect in locs.Keys)
        {
            if (!_LocationLookup.ContainsKey(vect))
            {
                //Debug.Log("fucken adding to vect: " + vect + " script: " + locs[vect]);
                _LocationLookup.Add(vect, locs[vect]);
            }
            else
            {
                Debug.LogError("trying to use already taking location!!!");
            }
        }
    }



    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    public void AttachMapToNode<T>(T node) where T : BaseNode
    {
        if (node.thisNodeType == NodeTypes.WorldNode)
        {
            WorldNode worldNode = node as WorldNode;
            AttachMapPiecesToWorldNode(worldNode);
        }

        if (node.thisNodeType == NodeTypes.MapNode)
        {
            MapNode mapNode = node as MapNode;
            AttachMapPieceToMapNode(mapNode);
        }

        if (node.thisNodeType == NodeTypes.ConnectorNode)
        {
            ConnectorNode connectNode = node as ConnectorNode;
            AttachMapPieceToConnectorNode(connectNode);
        }
    }

    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    private void AttachMapPiecesToWorldNode(WorldNode worldNode)
    {
        int mapCount = 0;
        foreach (MapNode mapNode in worldNode.mapNodes)
        {
            mapNode.entrance = true;
            Vector3Int nodeVect = mapNode.nodeLocation;
            int mapSize = mapNode.nodeSize;
            int layerCount = mapNode.nodeLayerCount;
            int rotation = mapNode.nodeRotation;
            int mapType = mapNode.nodeMapType;
            int mapPiece = mapCount;
            // for the players small ship
            if(mapCount == 4)
            {
                mapNode.playerShipMapPART1 = true;
            }
            if (mapCount == 13)
            {
                mapNode.playerShipMapPART2 = true;
            }
            _gridBuilder.BuildLocationGrid(mapNode, mapSize);
            List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();
            _mapPieceBuilder.SetWorldNodeNeighboursForDock(worldNode.neighbours); // for the ship docks
            _mapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
            mapNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
            mapNode.mapVentData = _mapPieceBuilder.GetMapVentData();
            AddGridLocationsToMap(_gridBuilder.GetGridLocations());
            mapCount++;
        }
        _gameManager._gamePlayManager.StartGame(worldNode.nodeLocation); // this is not best place for this!! dont like this
    }

    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    private void AttachMapPieceToMapNode(MapNode mapNode)
    {
        Vector3Int nodeVect = mapNode.nodeLocation;
        int mapSize = mapNode.nodeSize;
        int layerCount = mapNode.nodeLayerCount;
        int rotation = mapNode.nodeRotation;
        int mapType = mapNode.nodeMapType;
        int mapPiece = mapNode.nodeMapPiece;

        _gridBuilder.BuildLocationGrid(mapNode, mapSize);
        List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();
        _mapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
        mapNode.RemoveDoorPanels();
        mapNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
        mapNode.mapVentData = _mapPieceBuilder.GetMapVentData();
        AddGridLocationsToMap(_gridBuilder.GetGridLocations());
    }

    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    private void AttachMapPieceToConnectorNode(ConnectorNode connectNode)
    {
        Vector3Int nodeVect = connectNode.nodeLocation;
        int mapSize = connectNode.nodeSize;
        int layerCount = connectNode.nodeLayerCount;
        int rotation = connectNode.nodeRotation;
        int mapType = connectNode.nodeMapType;
        int mapPiece = connectNode.nodeMapPiece;

        _gridBuilder.BuildLocationGrid(connectNode, mapSize);
        List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();
        _mapPieceBuilder.AttachMapPieceToMapNode(connectNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
        //connectNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
        //connectNode.mapVentData = _mapPieceBuilder.GetMapVentData();
        AddGridLocationsToMap(_gridBuilder.GetGridLocations());
    }

    /////////////////////////////////////////////////


    public void BuildMapForClient () {

        StartCoroutine(BuildGridEnumerator());
    }

    // this is make the game actually start at startup and not wait loading
    private IEnumerator BuildGridEnumerator()
    {
        float buildTime = 0.1f;

        // Get the World Nodes
        _worldBuilder.BuildWorldNodes(buildTime);
        _worldNodes = _worldBuilder.GetWorldNodes();

        // Get the Map Nodes around the World Nodes
        Dictionary<WorldNode, List<MapNode>> worldAndMapNodes = _worldBuilder.GetWorldAndWrapperNodes();

        // World Nodes and Maps
        foreach (WorldNode worldNode in worldAndMapNodes.Keys)
        {
            worldNode._locationBuilder = this;
            List<MapNode> wrapperNodes = worldAndMapNodes[worldNode];
            foreach (MapNode mapNode in wrapperNodes)
            {
                mapNode._locationBuilder = this;
            }
        }


        // Connectors
        Dictionary<WorldNode, List<ConnectorNode>> worldAndConnectorNodes = _worldBuilder.GetWorldAndConnectorNodes();

        //Debug.Log("worldAndConnectorNodes.Count: " + worldAndConnectorNodes.Count);

        foreach (WorldNode worldNode in worldAndConnectorNodes.Keys)
        {
            List<ConnectorNode> connectorNodes = worldAndConnectorNodes[worldNode];

            foreach (ConnectorNode connectorNode in connectorNodes)
            {
                connectorNode._locationBuilder = this;

                Vector3 nodeVect = connectorNode.nodeLocation;

                int layerCount = connectorNode.nodeLayerCount;

                int mapSize = 1; // this is for Each connector

                int rotation = connectorNode.nodeRotation;

                if (connectorNode.connectorUp) // for the connectors going up ... FCKING ANNOYING THAT ITS USING A ROTATION AND NOT A TAG
                {
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").transform.localPosition = new Vector3Int(0, 0, 0); // to fix annoying vertical connector
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").transform.localScale = new Vector3Int(8, 8, 8);
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").Find("Select").transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }

                connectorNode.gameObject.transform.localEulerAngles = new Vector3Int(0, rotation * 90, 0);
            }
        }



    
        // Outer Zones
      /*  List<MapNode> outerNodes = _worldBuilder.GetOuterNodes();
        foreach (MapNode outerNode in outerNodes)
        {
            Vector3 nodeVect = outerNode.nodeLocation;
            _outerZoneBuilder.CreateOuterZoneForNode(outerNode);
        }


        // Docking Zones
        List<MapNode> dockingNodes = _worldBuilder.GetDockingNodes();

        foreach (MapNode dockingNode in dockingNodes)
        {
            Vector3 nodeVect = dockingNode.nodeLocation;
            int layerCount = dockingNode.nodeLayerCount;
            int mapSize = 1; // this is for Each docking tile
            _gridBuilder.BuildLocationGrid(nodeVect, mapSize);
            List<Vector3Int> mapPieceNodes = _gridBuilder.GetGridNodePositions();
            _mapPieceBuilder.AttachMapPieceToMapNode(dockingNode, mapPieceNodes, layerCount, mapSize, 2); // 2= connectors, 3 = roofs
            yield return new WaitForSeconds(buildTime);
        }
        */

        yield return new WaitForSeconds(buildTime);

        Debug.Log("FINSIHED LOADING!!!!!!!!");
    }




    public bool CheckIfLocationExists(Vector3 loc) {

		if (_LocationLookup.ContainsKey (loc)) {
           // Debug.Log("fuck _LocationLookup.ContainsKey true: script >>>: " + _LocationLookup[loc]);
            return true;
		}
		return false;
	}


	public CubeLocationScript GetLocationScript(Vector3 loc) {

		if (CheckIfLocationExists(loc)) {
           // Debug.Log("fuck _LocationLookup[loc]: " + _LocationLookup[loc]);
            return _LocationLookup[loc];
		}
		return null;
	}

	public CubeLocationScript CheckIfCanMoveToCube(Vector3 loc) {

		CubeLocationScript cubeScript = GetLocationScript(loc);

		if (cubeScript != null) {
			if (cubeScript._cubeOccupied) {
				cubeScript = null;
			}
		}
        else
        {
            Debug.LogError("cubeScript == null");
            return null;
        }
		return cubeScript;
	}



    ////// Dont think this should be here
    public void SetCubeActive(bool onOff, Vector3 cubeVect = new Vector3(), Vector3 nodePosOffset = new Vector3())
    {
        if (_activeCube)
        {
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(false);
            _activeCube = null;
        }

        if (onOff)
        {
            _activeCube = GetLocationScript(cubeVect);
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(true);
            _gameManager._playerManager._playerObject.GetComponent<UnitsAgent>().MakeActiveUnitMove(cubeVect, nodePosOffset);
        }
    }
}
