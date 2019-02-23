using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LayerManager _instance;

    ////////////////////////////////////////////////

    GameManager _gameManager;

    ////////////////////////////////////////////////

    // Layer INfo
    private int _startLayer;
    private int _maxLayer; // This needs to change with the amout of y levels, basicly level*2 because of vents layer ontop of layer
    private int _minLayer;
    private int _currLayer;

    ////////////////////////////////////////////////

    public int LayerStart
    {
        get { return _startLayer; }
        set { _startLayer = value; }
    }
    public int LayerMax
    {
        get { return _maxLayer; }
        set { _maxLayer = value; }
    }
    public int LayerMin
    {
        get { return _minLayer; }
        set { _minLayer = value; }
    }
    public int LayerCurr
    {
        get { return _currLayer; }
        set { _currLayer = value; }
    }

    ////////////////////////////////////////////////

    public Dictionary<int, List<CubeLocationScript>> _layerCubeList;
    public Dictionary<int, List<BaseNode>> _layerNodeList;

    int layerID;
    BaseNode parentWorldNode;
    int parentWorldLayerID;


    int LAYERVISIBLE = 8;
    int LAYERNOTVISIBLE = 9;

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
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _layerCubeList = new Dictionary<int, List<CubeLocationScript>>();
        _layerNodeList = new Dictionary<int, List<BaseNode>>();


        // change layer callback event
        UIManager.OnChangeLayerClick += ChangeCameraLayerByHUD;

        LayerCurr = -1;
    }

    //////////////////

    public void ChangeCameraLayerByHUD(int change)
    {
        if (change == 1)
        {
            layerID++;
            ChangeSpecificCubesVisibility(LAYERVISIBLE);
        }
        if (change == -1)
        {
            ChangeSpecificCubesVisibility(LAYERNOTVISIBLE);
            layerID--;
        }
        LayerCurr = layerID;
    }


    public void ChangeCameraLayer(CubeLocationScript cubeScript)
    {
        parentWorldNode = cubeScript.transform.parent.parent.GetComponent<BaseNode>();
        parentWorldLayerID = parentWorldNode.nodeLayerCount;
        ChangeSpecificNodeVisibility(LAYERNOTVISIBLE); // set specfic world node to not visible

        if (LayerCurr != -1)
        {
            //ChangeSpecificNodeVisibility(parentWorldNode, LAYERVISIBLE);
            //ChangeSpecificCubesVisibility(parentWorldNode, LAYERNOTVISIBLE);
        }
    
        layerID = cubeScript.CubeLayerID;

        for (int i = parentWorldLayerID; i <= cubeScript.CubeLayerID; i++)
        {
            layerID = i;
            ChangeSpecificCubesVisibility(LAYERVISIBLE);
        }

        LayerCurr = layerID;
    }



    public void ChangeSpecificNodeVisibility(int visibilityLayer)
    {
        parentWorldNode.gameObject.layer = visibilityLayer;

        Transform[] allChildren = parentWorldNode.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = visibilityLayer;
        }
    }

    public void ChangeSpecificCubesVisibility(int visibilityLayer)
    {
        CubeLocationScript[] allChildren = parentWorldNode.GetComponentsInChildren<CubeLocationScript>();

        foreach (CubeLocationScript child in allChildren)
        {
            if (child.CubeLayerID == layerID)
            {
                child.gameObject.layer = visibilityLayer;

                Transform[] grandChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform grand in grandChildren)
                {
                    grand.gameObject.layer = visibilityLayer;
                }
            }
        }
    }


    //////////////////

    public void AddCubeToLayer(CubeLocationScript script)
    {
        int cubeLayer = script.CubeLayerID;
        if (_layerCubeList.ContainsKey(cubeLayer))
        {
            _layerCubeList[cubeLayer].Add(script);
        }
        else
        {
            _layerCubeList.Add(cubeLayer, new List<CubeLocationScript> { script });
        }
    }

    public void ChangeCubeLayerVisibility(int layerID, int visibilityLayer)
    {
        if (_layerCubeList.ContainsKey(layerID))
        {
            List<CubeLocationScript> scripts = _layerCubeList[layerID];

            foreach (CubeLocationScript script in scripts)
            {
                script.gameObject.layer = visibilityLayer;

                Transform[] allChildren = script.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    child.gameObject.layer = visibilityLayer;
                }
            }
        }
        else
        {
            Debug.LogError("Got an issue here NO layerID assigned or not in list: " + layerID);
        }
    }


    public void MakeAllCubeLayersVisible()
    {
        Debug.Log("fuekcn MakeAllLayersVisible");
        foreach (int layerID in _layerCubeList.Keys)
        {
            ChangeCubeLayerVisibility(layerID, LAYERVISIBLE);
        }
    }

    public void MakeAllCubeLayersNotVisible()
    {
        Debug.Log("fuekcn MakeAllLayersNotVisible");
        foreach (int layerID in _layerCubeList.Keys)
        {
            ChangeCubeLayerVisibility(layerID, LAYERNOTVISIBLE);
        }
    }

    //////////////////

    public void AddNodeToLayer(BaseNode node)
    {
        int nodeLayer = node.nodeLayerCount;
        if (_layerNodeList.ContainsKey(nodeLayer))
        {
            _layerNodeList[nodeLayer].Add(node);
        }
        else
        {
            _layerNodeList.Add(nodeLayer, new List<BaseNode> { node });
        }
    }

    public void ChangeNodeLayerVisibility(int layerID, int visibilityLayer)
    {
        if (_layerNodeList.ContainsKey(layerID))
        {
            List<BaseNode> nodes = _layerNodeList[layerID];

            foreach (BaseNode node in nodes)
            {
                node.gameObject.layer = visibilityLayer;

                Transform[] allChildren = node.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    child.gameObject.layer = visibilityLayer;
                }
            }
        }
        else
        {
            Debug.LogError("Got an issue here NO layerID assigned or not in list: " + layerID);
        }
    }

    public void MakeAllNodeLayersVisible()
    {
        Debug.Log("fuekcn MakeAllLayersVisible");
        foreach (int layerID in _layerNodeList.Keys)
        {
            ChangeNodeLayerVisibility(layerID, LAYERVISIBLE);
        }
    }

    public void MakeAllNodeLayersNotVisible()
    {
        Debug.Log("fuekcn MakeAllLayersNotVisible");
        foreach (int layerID in _layerNodeList.Keys)
        {
            ChangeNodeLayerVisibility(layerID, LAYERNOTVISIBLE);
        }
    }







}
