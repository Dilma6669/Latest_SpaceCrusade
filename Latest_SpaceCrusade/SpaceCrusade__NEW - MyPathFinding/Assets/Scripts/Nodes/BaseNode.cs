using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour {

    public LocationManager _locationBuilder;

    public NodeTypes thisNodeType;
    public Vector3Int nodeLocation;
    public int nodeSize;
    public int nodeRotation;
    public int nodeLayerCount;

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
            _locationBuilder.AttachMapToNode(nodeType);
            _nodeCover.SetActive(false);
            return false; // this is not a fail, this is deactivation
        }
        else
        {
            _locationBuilder.AttachMapToNode(nodeType);
            _nodeCover.SetActive(true);
            return true;
        }
    }

}
