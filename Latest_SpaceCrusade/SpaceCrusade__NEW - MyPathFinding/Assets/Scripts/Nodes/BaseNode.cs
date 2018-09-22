using UnityEngine;

public class BaseNode : MonoBehaviour {

    public BaseNode thisNodeType;

    public LocationManager _locationBuilder;

    public Vector3Int nodeLocation;

    public int nodeSize;

    public int nodeRotation;

    public int nodeLayerCount;

    GameObject _nodeCover;

    public bool ActivateMapPiece(bool coverActive, NodeCover cover) // turn on all map objects
    {
        _nodeCover = cover.gameObject;

        if (coverActive)
        {
            Debug.Log("Activating MapPiece");
            _locationBuilder.AttachMapToNode(this);
            _nodeCover.SetActive(false);
            return false; // this is not a fail, this is deactivation
        }
        else
        {
            Debug.Log("DeActivating MapPiece");
            _locationBuilder.AttachMapToNode(this);
            _nodeCover.SetActive(true);
            return true;
        }
    }
}
