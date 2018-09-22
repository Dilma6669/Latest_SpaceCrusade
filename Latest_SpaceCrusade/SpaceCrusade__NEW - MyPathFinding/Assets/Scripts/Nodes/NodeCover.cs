using UnityEngine;

public class NodeCover : MonoBehaviour {

    BaseNode parentNode;
    GameObject selector;

    bool _active = true;

    void Awake()
    {
        parentNode = transform.parent.GetComponent<BaseNode>();
        selector = transform.Find("Select").gameObject;
        selector.SetActive(false);
    }

    void OnMouseDown()
    {
        _active = parentNode.ActivateMapPiece(_active, this);
    }

    void OnMouseOver()
    {
        selector.SetActive(true);
    }

    void OnMouseExit()
    {
        selector.SetActive(false);
    }
}
