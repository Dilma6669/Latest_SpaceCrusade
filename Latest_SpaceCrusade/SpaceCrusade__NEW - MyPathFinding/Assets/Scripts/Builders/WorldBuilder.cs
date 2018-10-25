﻿using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    MapSettings _mapSettings;
    NodeBuilder _nodeBuilder;

    private List<Vector3Int> worldVects;
    private Dictionary<WorldNode, List<KeyValuePair<Vector3Int, int>>> connectorVectsAndRotations;
    private List<Vector3Int> outerVects;
    private List<Vector3Int> dockingVects;

    private List<WorldNode> WorldNodes;
    private Dictionary<WorldNode, List<MapNode>> worldNodeAndWrapperNodes;
    private Dictionary<WorldNode, List<ConnectorNode>> worldNodeAndconnectorNodes;
    private List<MapNode> outerNodes;
    //private List<MapNode> dockingNodes;

    private int lowestYpos = 10000;
    private int highestYpos = 0;

    Dictionary<Vector3, int[]> NEW_WORLD_GRID = new Dictionary<Vector3, int[]>();

    bool LOADPREBUILT_STRUCTURE = true;


    void Awake()
    {
        _mapSettings = transform.parent.GetComponent<MapSettings>();
        if (_mapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodeBuilder = transform.parent.GetComponentInChildren<NodeBuilder>();
        if (_nodeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public List<WorldNode> GetWorldNodes() { return WorldNodes; }
    public Dictionary<WorldNode, List<MapNode>> GetWorldAndWrapperNodes() { return worldNodeAndWrapperNodes; }
    public Dictionary<WorldNode, List<ConnectorNode>> GetWorldAndConnectorNodes() { return worldNodeAndconnectorNodes; }
    public List<MapNode> GetOuterNodes() { return outerNodes; }
    //public List<MapNode> GetDockingNodes() { return dockingNodes; }



    public void BuildWorldNodes(float waitTime)
    {
        List<List<Vector3Int>> container = GetWorld_Outer_DockingVects();
        worldVects = container[0];
        outerVects = container[1];
        //dockingVects = container[2];
        outerNodes = CreateOuterNodes(this.transform, outerVects);

        WorldNodes = CreateWorldNodes(worldVects);
        GetWorldNodeNeighbours();
        worldNodeAndWrapperNodes = CreateMapNodes(WorldNodes);

        connectorVectsAndRotations = GetConnectorVects(WorldNodes);
        worldNodeAndconnectorNodes = CreateConnectorNodes(connectorVectsAndRotations);

        //dockingNodes = CreateDockingNodes(dockingVects);

    }

    ////////////////////////////////////////////
    // Get the Vects //////////////////////////
    ////////////////////////////////////////////


    // Get World Outer Docking Vects ////////////////////////////////////////////
    private List<List<Vector3Int>> GetWorld_Outer_DockingVects()
    {
        List<List<Vector3Int>> container = new List<List<Vector3Int>>();

        List<Vector3Int> worldVects = new List<Vector3Int>();
        List<Vector3Int> outerVects = new List<Vector3Int>();
        List<Vector3Int> dockingVects = new List<Vector3Int>();

        int countY = _mapSettings.worldPadding; // First node position
        int countZ = _mapSettings.worldPadding;
        int countX = _mapSettings.worldPadding;

        int nodeDistanceXZ = _mapSettings.worldNodeDistanceXZ + 1; // + 1 cause distance is only space IN-between nodes
        int nodeDistanceY = _mapSettings.worldNodeDistanceY + 1;

        int worldSizeY = _mapSettings.worldSizeY + 2; // +2 for Outer/Ship Zones
        int worldSizeZ = _mapSettings.worldSizeZ + 2; // +2 for Outer/Ship Zones
        int worldSizeX = _mapSettings.worldSizeX + 2; // +2 for Outer/Ship Zones

        //int centralOuterNodeX = 2;
        //int dockingNodeX = 3;

        for (int y = 0; y < worldSizeY; y++)
        {
            for (int z = 0; z < worldSizeZ; z++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    //Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    int resultY = countY * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
                    int resultZ = countZ * _mapSettings.sizeOfMapPiecesXZ;
                    int resultX = countX * _mapSettings.sizeOfMapPiecesXZ;
                   
                    if ((x == 0) || (z == 0) || (x == (worldSizeX - 1)) || (z == (worldSizeZ - 1))
                        || y == 0 || y == worldSizeY-1)
                    {
                        // Get outer Zone central node
                       // if (z == (worldSizeX - 2) && x == centralOuterNodeX && y == 0)
                      //  {
                            outerVects.Add(new Vector3Int(resultX, resultY, resultZ));
                      //  } 

                        // Get docking lines
                       /* if (z == 0 && x == dockingNodeX)
                        {
                            dockingVects.Add(new Vector3Int(resultX, resultY, resultZ));
                        }*/
                    }
                    else // All central map nodes
                    {
                        worldVects.Add(new Vector3Int(resultX, resultY, resultZ));
                    }
                    

                    countX += nodeDistanceXZ;
                }
                countX = _mapSettings.worldPadding;
                countZ += nodeDistanceXZ;
            }
            countX = _mapSettings.worldPadding;
            countZ = _mapSettings.worldPadding;
            countY += nodeDistanceY;
        }

        container.Add(worldVects);
        container.Add(outerVects);
        container.Add(dockingVects);

        return container;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get Map Vects ////////////////////////////////////////////////////////////
    private List<Vector3Int> GetMapVects(WorldNode nodeScript)
    {
        Vector3Int loc = nodeScript.nodeLocation;
        int size = nodeScript.nodeSize;

        List<Vector3Int> nodeVects = new List<Vector3Int>();

        int multiplierXZ = (int)Mathf.Floor(size / 2) * _mapSettings.sizeOfMapPiecesXZ;
        int multiplierY = (int)Mathf.Floor(size / 2) * (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);

        int countY = loc.y - multiplierY;
        int countZ = loc.z - multiplierXZ;
        int countX = loc.x - multiplierXZ;

        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    nodeVects.Add(new Vector3Int(countX - 1, countY, countZ - 1)); // -1's to fix annoying postiioning issue
                    countX += _mapSettings.sizeOfMapPiecesXZ;
                }
                countX = loc.x - multiplierXZ;
                countZ += _mapSettings.sizeOfMapPiecesXZ;
            }
            countX = loc.x - multiplierXZ;
            countZ = loc.z - multiplierXZ;
            countY += (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
        }

        return nodeVects;
    }
    ////////////////////////////////////////////////////////////////////////////


    List<int[]> doorPanelLocations = new List<int[]>()
    {
        new int[]{}
    };


    // Get Connector Vects /////////////////////////////////////////////////////
    private Dictionary<WorldNode, List<KeyValuePair<Vector3Int, int>>> GetConnectorVects(List<WorldNode> worldNodes)
    {
        Dictionary<WorldNode, List<KeyValuePair<Vector3Int, int>>> connectorVectsAndRotations = new Dictionary<WorldNode, List<KeyValuePair<Vector3Int, int>>>();

       // int floorBounds = _mapSettings.worldSizeX * _mapSettings.worldSizeZ; dont need this now for some weird reason
        int roofBounds = ((_mapSettings.worldSizeX * _mapSettings.worldSizeZ) * _mapSettings.worldSizeY);

        foreach (WorldNode worldNode in worldNodes)
        {
            int nodeCount = worldNode.worldNodeCount;
            int[] worldNeighbours = worldNode.neighbours;
            List<KeyValuePair<Vector3Int, int>> vectList = new List<KeyValuePair<Vector3Int, int>>();

            if (worldNode.nodeSize == 1)
            {
                foreach (int worldNeigh in worldNeighbours)
                {
                    if (worldNeigh != -1) // out of bounds check
                    {
                       if (worldNeigh < roofBounds) // keeping nodes inside bounds
                       {
                            WorldNode neighbour = worldNodes[worldNeigh];
                            KeyValuePair<Vector3Int, int> vectorAndRot = GetVectsAndRotation(worldNode, neighbour, worldNeigh);
                            if (vectorAndRot.Value != -1) // weird issue still not sure why
                            {
                                vectList.Add(vectorAndRot);
                            }
                        }
                    }
                }
                connectorVectsAndRotations.Add(worldNode, vectList);
            }
        }
        return connectorVectsAndRotations;
    }

    public KeyValuePair<Vector3Int, int> GetVectsAndRotation(WorldNode node0, WorldNode node1, int neighCount)
    {
        Vector3Int connectionVect = new Vector3Int();

        bool initialSmaller = false;
        WorldNode smallerNode = null;
        WorldNode biggerNode = null;

        if (node0.nodeSize <= node1.nodeSize)
        {
            initialSmaller = true;
            smallerNode = node0;
            biggerNode = node1;
        }
        else if (node0.nodeSize > node1.nodeSize)
        {
            initialSmaller = false;
            smallerNode = node1;
            biggerNode = node0;
        }
        else
        {
            Debug.LogError("Something went wrong here");
        }


        int rotation = -1;

        Vector3Int finalVect;
        Vector3Int direction; // this is to seperate what axis x,y,z neighbour is

        if (initialSmaller)
        {
            direction = (biggerNode.nodeLocation - smallerNode.nodeLocation);
        }
        else
        {
            direction = (smallerNode.nodeLocation - biggerNode.nodeLocation);
        }


        finalVect = node0.nodeLocation;

        if (direction.x != 0 && direction.y == 0 && direction.z == 0)
        {
            if (direction.x > 0)
            {
                rotation = 1;
                finalVect = new Vector3Int(finalVect.x + (_mapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
            }
            else if (direction.x < 0)
            {
                rotation = 3;
                finalVect = new Vector3Int(finalVect.x - (_mapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
            }
        }
        else if (direction.x == 0 && direction.y != 0 && direction.z == 0)
        {
            if (direction.y > 0)
            {
                rotation = 4; // these are the bastards making the connectors go UP
                finalVect = new Vector3Int(finalVect.x, finalVect.y + ((_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY)), finalVect.z);
            }
            else if (direction.y < 0)
            {
                rotation = 4;// these are the bastards making the connectors go UP
                finalVect = new Vector3Int(finalVect.x, finalVect.y - ((_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY)), finalVect.z);
            }
        }
        else if (direction.x == 0 && direction.y == 0 && direction.z != 0)
        {
            if (direction.z > 0)
            {
                rotation = 0;
                finalVect = new Vector3Int(finalVect.x, finalVect.y, finalVect.z + (_mapSettings.sizeOfMapPiecesXZ));
            }
            else if (direction.z < 0)
            {
                rotation = 2;
                finalVect = new Vector3Int(finalVect.x, finalVect.y, finalVect.z - (_mapSettings.sizeOfMapPiecesXZ));
            }
        }
        else
        {
            Debug.LogError("SOMETHING WRONG HERE direction: " + direction);
            Debug.LogFormat("initialSmaller: {0} -node0: {1} -node1: {2}", initialSmaller, node0.nodeLocation, node1.nodeLocation);
            return new KeyValuePair<Vector3Int, int>(new Vector3Int(-1, -1, -1), -1);
        }

        connectionVect = new Vector3Int(finalVect.x - 1, finalVect.y, finalVect.z - 1); // -1's to fix annoying postiioning issue

        return new KeyValuePair<Vector3Int, int>(connectionVect, rotation);
    }
    ////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////
    // Create the Nodes from the Vects ///////
    ////////////////////////////////////////////


    // Create World Nodes ///////////////////////////////////////////////////
    private List<WorldNode> CreateWorldNodes(List<Vector3Int> nodeVects)
    {
        // build inital map Node
        List<WorldNode> worldNodes = new List<WorldNode>();

        int rowMultipler = _mapSettings.worldSizeX;
        int colMultiplier = _mapSettings.worldSizeZ;

        int totalMultiplier = _mapSettings.worldSizeX * _mapSettings.worldSizeZ;

        WorldNodeData_01 worldFloor = ScriptableObject.CreateInstance<WorldNodeData_01>();
        List<int[,]> floors = worldFloor.floors;

        int countFloorX = 0; // complicated to explain to future me
        int countFloorZ = 0;
        int countFloorY = 1;

        int count = 1;
        foreach (Vector3Int vect in nodeVects)
        {
            int rotation = 0;
            WorldNode nodeScript = CreateNode<WorldNode>(this.transform, vect, rotation, NodeTypes.WorldNode);
            nodeScript.worldNodeCount = (count - 1);

            // for the specified map structures
            if (LOADPREBUILT_STRUCTURE)
            {
                int[,] floor = floors[countFloorY - 1];
                if (floor[countFloorX, countFloorZ] == 01)
                {
                    int randSize = _mapSettings.getRandomMapSize;
                    nodeScript.nodeSize = randSize;
                }
                else
                {
                    nodeScript.nodeSize = 0;
                }
            }
            else
            {
                int randSize = _mapSettings.getRandomMapSize;
                nodeScript.nodeSize = randSize;
            }

            worldNodes.Add(nodeScript);

            // for counting, best not to change, even tho its ugly
            countFloorX++;
            if (count % totalMultiplier == 0)
            {
                countFloorY++;
            }
            if (countFloorX % rowMultipler == 0)
            {
                countFloorX = 0;
                countFloorZ++;
            }
            if (countFloorZ % colMultiplier == 0)
            {
                countFloorZ = 0;
            }
            count++;
        }
        return worldNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Map Nodes ////////////////////////////////////////////////////////
    private Dictionary<WorldNode, List<MapNode>> CreateMapNodes(List<WorldNode> worldNodes)
    {
        // Wrap map Nodes around around Initial
        Dictionary<WorldNode, List<MapNode>> worldNodeAndWrapperNodes = new Dictionary<WorldNode, List<MapNode>>();

        foreach (WorldNode worldNode in worldNodes)
        {
            List<Vector3Int> mapVects = GetMapVects(worldNode);
            List<MapNode> mapNodes = new List<MapNode>();

            bool shipEntrance = false;
            int shipEntranceProbablity = 20;

            if (worldNode.nodeSize == 3 && Random.Range(0, shipEntranceProbablity) == 0)
            {
                shipEntrance = true;
                _nodeBuilder.AttachCoverToNode(worldNode, worldNode.gameObject, CoverTypes.LargeGarageCover);
            }

            int mapCount = 0;
            foreach (Vector3Int vect in mapVects)
            {
                int rotation = 0;

                MapNode mapNode = CreateNode<MapNode>(worldNode.gameObject.transform, vect, rotation, NodeTypes.MapNode);
                mapNode.nodeSize = 1;
                mapNode.neighbours = new int[6];
                for (int i = 0; i < mapNode.neighbours.Length; i++)
                {
                    /*
                    if (!shipEntrance)
                    {*/
                        mapNode.neighbours[i] = 1;
                  /*  }
                    else
                    {
                        mapNode.neighbours[i] = -1; // less connections in the shipEntrance, mostly open
                    }*/
                }

                mapNodes.Add(mapNode);
                if (!shipEntrance)
                {
                    _nodeBuilder.AttachCoverToNode(mapNode, mapNode.gameObject, CoverTypes.NormalCover);
                }

                /*// dont think need this anymore But not deleteing incase still do
                if (vect.y <= lowestYpos) // this is find lowest point to make LayerCounts
                {
                    lowestYpos = vect.y;
                }
                if (vect.y >= highestYpos) // this is find highest point to make LayerCounts
                {
                    highestYpos = vect.y;
                }*/

                mapCount++;
            }
            worldNode.mapNodes = mapNodes;
            worldNodeAndWrapperNodes.Add(worldNode, mapNodes);

            //////// Map Neighbours
            int[] worldNodeNeighbours = worldNode.neighbours;

            if (worldNode.nodeSize == 1)
            {
                MapNode mapNode = worldNode.mapNodes[0];
                int[] mapNeighbours = mapNode.neighbours;

                for (int i = 0; i < worldNodeNeighbours.Length; i++)
                {
                    if (worldNodeNeighbours[i] != -1)
                    {
                        mapNeighbours[i] = 1;
                    }
                    else
                    {
                        mapNeighbours[i] = -1;
                        mapNode.entranceSides.Add(i);
                    }
                }
            }
            ////////
            if (worldNode.nodeSize == 3)
            {
                // bottom
                SetMapNeighboursWithMultipleLinks(worldNode, 0, 4, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, shipEntrance);
                // Front
                SetMapNeighboursWithMultipleLinks(worldNode, 1, 10, new int[] { 0, 1, 2, 9, 10, 11, 18, 19, 20 }, shipEntrance);
                // Left
                SetMapNeighboursWithMultipleLinks(worldNode, 2, 12, new int[] { 0, 3, 6, 9, 12, 15, 18, 21, 24 }, shipEntrance);
                // Right
                SetMapNeighboursWithMultipleLinks(worldNode, 3, 14, new int[] { 2, 5, 8, 11, 14, 17, 20, 23, 26 }, shipEntrance);
                // Back
                SetMapNeighboursWithMultipleLinks(worldNode, 4, 16, new int[] { 6, 7, 8, 15, 16, 17, 24, 25, 26 }, shipEntrance);
                // Top
                SetMapNeighboursWithMultipleLinks(worldNode, 5, 22, new int[] { 18, 19, 20, 21, 22, 23, 24, 25, 26 }, shipEntrance);
            }
        }
    

        /* // dont think need this anymore But not deleteing incase still do
        // figure out LayerCount (DONT LIKE THIS) basicly have to re-run whats just happened to get count
        foreach (WorldNode worldNode in worldNodeAndWrapperNodes.Keys)
        {
            List<MapNode> wrapperNodes = worldNodeAndWrapperNodes[worldNode];
            foreach (MapNode nodeScript in wrapperNodes)
            {
                nodeScript.nodeLayerCount = GetLayerCountForNodePos(nodeScript.nodeLocation);
            }
        }*/
        return worldNodeAndWrapperNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // SetUp MapNode Connections to neighbours ////////////////////////////////////////////////////////
    private void SetMapNeighboursWithMultipleLinks(WorldNode worldNode, int worldNeighCount, int singleLinkCount, int[] multipleLinkCounts, bool shipEntrance)
    {
        int[] worldNodeNeighbours = worldNode.neighbours;

        if (worldNodeNeighbours[worldNeighCount] != -1)
        {
            WorldNode worldNeighbour = WorldNodes[worldNodeNeighbours[worldNeighCount]];

            if (worldNeighbour.nodeSize == 1)
            {
                foreach (int link in multipleLinkCounts)
                {
                    worldNode.mapNodes[link].neighbours[worldNeighCount] = -1;
                }
                worldNode.mapNodes[singleLinkCount].neighbours[worldNeighCount] = 1; // for the middle front connector
            }
            if (worldNeighbour.nodeSize == 3)
            {
                foreach (int link in multipleLinkCounts)
                {
                    worldNode.mapNodes[link].neighbours[worldNeighCount] = 1;
                }
            }
        }
        else
        {
            foreach (int link in multipleLinkCounts)
            {
                worldNode.mapNodes[link].neighbours[worldNeighCount] = -1;
                worldNode.mapNodes[link].entranceSides.Add(worldNeighCount);
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Connector Nodes ////////////////////////////////////////////////
    private Dictionary<WorldNode, List<ConnectorNode>> CreateConnectorNodes(Dictionary<WorldNode, List<KeyValuePair<Vector3Int, int>>> connectorVects)
    {
        // Wrap map Nodes around around Initial
        Dictionary<WorldNode, List<ConnectorNode>> worldNodeAndConnectorNodes = new Dictionary<WorldNode, List<ConnectorNode>>();

        foreach (WorldNode worldNode in connectorVects.Keys)
        {
            List<ConnectorNode> connectorNodes = new List<ConnectorNode>();
            List<KeyValuePair<Vector3Int, int>> vectsAndRot = connectorVects[worldNode];

            foreach (KeyValuePair<Vector3Int, int> pair in vectsAndRot)
            {
                Vector3Int vector = pair.Key;
                int rotation = pair.Value;

                ConnectorNode node = CreateNode<ConnectorNode>(worldNode.gameObject.transform, vector, rotation, NodeTypes.ConnectorNode);
                _nodeBuilder.AttachCoverToNode(node, node.gameObject, CoverTypes.ConnectorCover);
                node.nodeSize = 1;
                connectorNodes.Add(node);
                if(rotation == 4)
                {
                    node.connectorUp = true;
                }
            }
            worldNode.connectorNodes = connectorNodes;
            worldNodeAndConnectorNodes.Add(worldNode, connectorNodes);
        }
        return worldNodeAndConnectorNodes;
   }
    ////////////////////////////////////////////////////////////////////////////

    // Create Outer Nodes /////////////////////////////////////////////////////
    private List<MapNode> CreateOuterNodes(Transform parent, List<Vector3Int> outerVects)
    {
        List<MapNode> outerNodes = new List<MapNode>();
        foreach (Vector3Int vect in outerVects)
        {
            int rotation = 0;
            outerNodes.Add(CreateNode<MapNode>(parent, vect, rotation, NodeTypes.OuterNode));
        }
        return outerNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Create Docking Nodes /////////////////////////////////////////////////////
    private List<MapNode> CreateDockingNodes(Transform parent, List<Vector3Int> dockingVects)
    {
        List<MapNode> dockingNodes = new List<MapNode>();
        foreach (Vector3Int vect in dockingVects)
        {
            int rotation = 0;
            dockingNodes.Add(CreateNode<MapNode>(parent, vect, rotation, NodeTypes.DockingNode));
        }
        return dockingNodes;
    }
    ////////////////////////////////////////////////////////////////////////////



    ////////////////////////////////////////////
    // Helper Functions ////////////////////////
    ////////////////////////////////////////////


    // Create Generic Node /////////////////////////////////////////////////////
    private T CreateNode<T>(Transform parentNode, Vector3Int vect, int rotation, NodeTypes nodeType) where T : BaseNode
    {
        //Debug.Log("Vector3 (gridLoc): x: " + vect.x + " y: " + vect.y + " z: " + vect.z);
        GameObject node = _nodeBuilder.InstantiateNodeObject(vect, nodeType, parentNode);
        T nodeScript = node.GetComponent<T>();
        nodeScript.nodeLocation = vect;
        nodeScript.nodeRotation = rotation;
        nodeScript.nodeLayerCount = GetLayerCountForNodePos(vect);
        return nodeScript;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get Layer Count for Node Position ////////////////////////////////////////
    private int GetLayerCountForNodePos(Vector3Int nodePos)
    {
        int yPos = nodePos.y;
        int currHeight = lowestYpos;
        int mapHeight = (_mapSettings.sizeOfMapPiecesY + _mapSettings.sizeOfMapVentsY);
        for (int layer = 0; layer < 50; layer += 2) // needs to be 2 coz 2 layers needed each map piece floor and roof
        {
            if (yPos == currHeight)
            {
                return layer;
            }
            currHeight += mapHeight;
        }
        return 0;
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get World Node Neighbours ///////////////////////////////////////////////
    private void GetWorldNodeNeighbours()
    {
        // build inital map Node
        List<WorldNode> worldNodes = new List<WorldNode>();
        bool left = true;
        bool right = false;
        bool front = false;
        bool back = true;

        int rowMultipler = _mapSettings.worldSizeX;
        int colMultiplier = _mapSettings.worldSizeZ;

        int totalMultiplier = _mapSettings.worldSizeX * _mapSettings.worldSizeZ;

        int countFloorY = 1;

        int count = 1;
        foreach (WorldNode worldNode in WorldNodes)
        {

            //Debug.Log("worldNode.worldNodeCount: " + worldNode.worldNodeCount);
            // for neighbours
            right = (count % rowMultipler == 0) ? true : false;
            left = (count == 1 || ((count - 1) % rowMultipler == 0)) ? true : false;
            front = ((count + _mapSettings.worldSizeX) > (totalMultiplier * countFloorY) && count <= (totalMultiplier * countFloorY)) ? true : false;
            back = (count >= ((totalMultiplier + 1) * (countFloorY - 1)) && count <= (totalMultiplier * (countFloorY - 1)) + _mapSettings.worldSizeX) ? true : false;

            worldNode.neighbours = GetNeighbours((count - 1), left, right, front, back);

            // for counting, best not to change, even tho its ugly
            if (count % totalMultiplier == 0)
            {
                countFloorY++;
            }
            count++;
        }
    }



    private int[] GetNeighbours(int count, bool left, bool right, bool front, bool back)
    {
        int[] neighbours = new int[6];

        neighbours[0] = count - (_mapSettings.worldSizeX * _mapSettings.worldSizeZ);//(x, y - 1, z)
        neighbours[1] = (back) ? -1 : count - _mapSettings.worldSizeX;              //(x, y, z - 1)
        neighbours[2] = (left) ? -1 : count - 1;                                    //(x - 1, y, z)
        neighbours[3] = (right) ? -1 : count + 1;                                   //(x + 1, y, z)
        neighbours[4] = (front) ? -1 : count + _mapSettings.worldSizeX;             //(x, y, z + 1)
        neighbours[5] = count + (_mapSettings.worldSizeX * _mapSettings.worldSizeZ);//(x, y + 1, z)

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] <= -1)
            {
                neighbours[i] = -1;
            }
            else if (neighbours[i] >= WorldNodes.Count)
            {
               neighbours[i] = -1;
            }
            else if (WorldNodes[neighbours[i]].nodeSize < 1)
            {
                neighbours[i] = -1;
            }
        }
        return neighbours;
    }
    ////////////////////////////////////////////////////////////////////////////


    /*
    private bool AddValueToWorldGridLoc(Vector3 loc, int[] values)
    {
        if (!NEW_WORLD_GRID.ContainsKey(loc))
        {
            NEW_WORLD_GRID[loc] = values;
            return true;
        }
        else
        {
            Debug.LogFormat("ISSUE HERE GRID LOCATION ALREADY ASSIGNED!!! Vector: {0}", loc);
            return false;
        }
    }
    */

}
