using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour {

    public WorldManager _worldManager;

    public WorldNode worldNodeParent;

    public NodeTypes thisNodeType;
    public Vector3 nodeLocation;
    public int nodeSize;
    public int nodeRotation;
    public int nodeLayerCount;
    public int nodeMapType = -1;
    public int nodeMapPiece = -1;

    protected GameObject _nodeCover;

    public int[] neighbours;
    public bool entrance = false;
    public List<int> entranceSides = new List<int>();

    public bool playerShipMapPART1 = false;
    public bool playerShipMapPART2 = false;

    public bool connectorUp = false;

    public virtual bool ActivateMapPiece<T>(T nodeType, bool coverActive, GameObject cover) where T : BaseNode // turn on all map objects
    {
         _nodeCover = cover.gameObject;

        if (coverActive)
        {
            WorldBuilder.AttachMapToNode(nodeType);
            _nodeCover.SetActive(false);
            return false; // this is not a fail, this is deactivation
        }
        else
        {
            WorldBuilder.AttachMapToNode(nodeType);
            _nodeCover.SetActive(true);
            return true;
        }
    }

}
