﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    GameManager _gameManager;

    LocationManager _locationManager;

    [HideInInspector]
    public WorldBuilder _worldBuilder;
    [HideInInspector]
    public GridBuilder _gridBuilder;
    [HideInInspector]
    public CubeBuilder _cubeBuilder;
    [HideInInspector]
    public MapPieceBuilder _mapPieceBuilder;
    [HideInInspector]
    public OuterZoneBuilder _outerZoneBuilder;
    [HideInInspector]
    public PlayerShipBuilder _playerShipBuilder;
    [HideInInspector]
    public NodeBuilder _nodeBuilder;


    [HideInInspector]
    public MapSettings _mapSettings;



    public List<WorldNode> _worldNodes;


    void Awake()
    {

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _locationManager = _gameManager._locationManager;
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }


        _worldBuilder = GetComponentInChildren<WorldBuilder>();
        if (_worldBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _gridBuilder = GetComponentInChildren<GridBuilder>();
        if (_gridBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cubeBuilder = GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _mapPieceBuilder = GetComponentInChildren<MapPieceBuilder>();
        if (_mapPieceBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _outerZoneBuilder = GetComponentInChildren<OuterZoneBuilder>();
        if (_outerZoneBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = GetComponentInChildren<NodeBuilder>();
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }



        _mapSettings = GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

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
            Vector3 nodeVect = mapNode.nodeLocation;
            int mapSize = mapNode.nodeSize;
            int layerCount = mapNode.nodeLayerCount;
            int rotation = mapNode.nodeRotation;
            int mapType = mapNode.nodeMapType;
            int mapPiece = mapCount;
            // for the players small ship
            if (mapCount == 4)
            {
                mapNode.playerShipMapPART1 = true;
            }
            if (mapCount == 13)
            {
                mapNode.playerShipMapPART2 = true;
            }
            _gridBuilder.BuildLocationGrid(mapNode, mapSize);
            List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();
            _mapPieceBuilder.SetWorldNodeNeighboursForDock(worldNode.neighbours); // for the ship docks
            _mapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
            _locationManager.AddCubeScriptToLocationLookup(_gridBuilder.GetGridLocations()); // needs to be after AttachMapPieceToMapNode
            _mapPieceBuilder.SetPanelsNeighbours();
            mapNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
            mapNode.mapVentData = _mapPieceBuilder.GetMapVentData();
            mapCount++;
        }
        _gameManager.StartGame(worldNode.nodeLocation); // <<<<<< start the fucking game bitch
    }

    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    private void AttachMapPieceToMapNode(MapNode mapNode)
    {
        Vector3 nodeVect = mapNode.nodeLocation;
        int mapSize = mapNode.nodeSize;
        int layerCount = mapNode.nodeLayerCount;
        int rotation = mapNode.nodeRotation;
        int mapType = mapNode.nodeMapType;
        int mapPiece = mapNode.nodeMapPiece;

        _gridBuilder.BuildLocationGrid(mapNode, mapSize);
        List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();
        _mapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
        _locationManager.AddCubeScriptToLocationLookup(_gridBuilder.GetGridLocations()); // needs to be after AttachMapPieceToMapNode
        _mapPieceBuilder.SetPanelsNeighbours();
        mapNode.RemoveDoorPanels();
        mapNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
        mapNode.mapVentData = _mapPieceBuilder.GetMapVentData();
    }

    //////////////////////////////////////////
    /// THESE NEED TO BE IN A DIFFFERNT SCRIPT
    private void AttachMapPieceToConnectorNode(ConnectorNode connectNode)
    {
        Vector3 nodeVect = connectNode.nodeLocation;
        int mapSize = connectNode.nodeSize;
        int layerCount = connectNode.nodeLayerCount;
        int rotation = connectNode.nodeRotation;
        int mapType = connectNode.nodeMapType;
        int mapPiece = connectNode.nodeMapPiece;

        _gridBuilder.BuildLocationGrid(connectNode, mapSize);
        List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();
        _mapPieceBuilder.AttachMapPieceToMapNode(connectNode, mapPieceNodes, layerCount, mapSize, mapType, mapPiece, rotation);
        _locationManager.AddCubeScriptToLocationLookup(_gridBuilder.GetGridLocations()); // needs to be after AttachMapPieceToMapNode
        _mapPieceBuilder.SetPanelsNeighbours();
        //connectNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
        //connectNode.mapVentData = _mapPieceBuilder.GetMapVentData();
    }

    /////////////////////////////////////////////////


    public void BuildMapForClient()
    {
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
            worldNode._worldManager = this;
            List<MapNode> wrapperNodes = worldAndMapNodes[worldNode];
            foreach (MapNode mapNode in wrapperNodes)
            {
                mapNode._worldManager = this;
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
                connectorNode._worldManager = this;

                Vector3 nodeVect = connectorNode.nodeLocation;

                int layerCount = connectorNode.nodeLayerCount;

                int mapSize = 1; // this is for Each connector

                int rotation = connectorNode.nodeRotation;

                if (connectorNode.connectorUp) // for the connectors going up ... FCKING ANNOYING THAT ITS USING A ROTATION AND NOT A TAG
                {
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").transform.localPosition = new Vector3(0, 0, 0); // to fix annoying vertical connector
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").transform.localScale = new Vector3(8, 8, 8);
                    connectorNode.gameObject.transform.Find("ConnectorNodeCover(Clone)").Find("Select").transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }

                connectorNode.gameObject.transform.localEulerAngles = new Vector3(0, rotation * 90, 0);
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
              List<Vector3> mapPieceNodes = _gridBuilder.GetGridNodePositions();
              _mapPieceBuilder.AttachMapPieceToMapNode(dockingNode, mapPieceNodes, layerCount, mapSize, 2); // 2= connectors, 3 = roofs
              yield return new WaitForSeconds(buildTime);
          }
          */

        yield return new WaitForSeconds(buildTime);

        Debug.Log("FINSIHED Building World!!!!!!!!");
    }



}
