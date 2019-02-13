using UnityEngine;

public class NodeBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static NodeBuilder _instance;

    ////////////////////////////////////////////////

    public GameObject _gridObjectPrefab; // Debugging purposes
    public GameObject _worldNodePrefab; // object that shows Map nodes
    public GameObject _mapNodePrefab; // object that shows Map nodes
    public GameObject _connectorNodePrefab; // object that shows Map nodes
    public GameObject _outerNodePrefab; // objects that define the out of bounds areas
    public GameObject _dockingNodePrefab; // node that players ship connect to when docking

    public GameObject _normalCoverPrefab; 
    public GameObject _openCoverPrefab;
    public GameObject _largeGarageCoverPrefab;
    public GameObject _connectorCoverPrefab;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        if (_gridObjectPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_worldNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_mapNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_connectorNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_outerNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_dockingNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_normalCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_openCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_largeGarageCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_connectorCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // node objects are spawned at bottom corner each map piece
    public GameObject InstantiateNodeObject(Vector3 loc, NodeTypes nodePrefab, Transform parent)
    {
        //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
        GameObject nodeObject = Instantiate(GetNodePrefab(nodePrefab), parent, false);
        nodeObject.transform.position = loc;
        nodeObject.transform.SetParent(parent);
        nodeObject.transform.localScale = new Vector3(1, 1, 1);

        return nodeObject;
    }

    public void AttachCoverToNode<T>(T nodeType, GameObject node, CoverTypes _cover) where T : BaseNode
    {
        //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
        GameObject cover = Instantiate(GetCoverPrefab(_cover), node.transform, false);
        cover.transform.SetParent(node.transform);
        cover.GetComponent<NodeCover>().parentNode = nodeType;
    }


    private GameObject GetNodePrefab(NodeTypes node)
    {
        switch (node)
        {
            case NodeTypes.GridNode:
                return _gridObjectPrefab;
            case NodeTypes.WorldNode:
                return _worldNodePrefab;
            case NodeTypes.MapNode:
                return _mapNodePrefab;
            case NodeTypes.ConnectorNode:
                return _connectorNodePrefab;
            case NodeTypes.OuterNode:
                return _outerNodePrefab;
            case NodeTypes.DockingNode:
                return _dockingNodePrefab;
            default:
                Debug.Log("OPPSALA WE HAVE AN ISSUE HERE");
                return null;
        }
    }

    private GameObject GetCoverPrefab(CoverTypes cover)
    {
        switch (cover)
        {
            case CoverTypes.NormalCover:
                return _normalCoverPrefab;
            case CoverTypes.OpenCover:
                return _openCoverPrefab;
            case CoverTypes.LargeGarageCover:
                return _largeGarageCoverPrefab;
            case CoverTypes.ConnectorCover:
                return _connectorCoverPrefab;
            default:
                Debug.Log("OPPSALA WE HAVE AN ISSUE HERE");
                return null;
        }
    }

}
