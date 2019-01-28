using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitScript : NetworkBehaviour {

    GameManager _gameManager;

    // GamePlay data
    List<CubeLocationScript> _pathFindingNodes;
	bool _unitActive;
	// Visual
	Renderer[] _rends;


    // Unit stats
    int _unitModel;
    bool _unitCanClimbWalls;
    public int[] _unitCombatStats;
    Vector3 _startingWorldLoc;
    int _playerControllerId;
    NetworkInstanceId _netID;
    public CubeLocationScript _cubeUnitIsOn;

    UnitData _unitData;

    public int UnitModel
    {
        get { return _unitModel; }
        set { _unitModel = value; }
    }

    public bool UnitCanClimbWalls
    {
        get { return _unitCanClimbWalls; }
        set { _unitCanClimbWalls = value; }
    }

    /*
    public int[] UnitCombatStats
    {
        get { return _unitCombatStats; }
        set { _unitCombatStats = value; }
    }
    */

    public Vector3 UnitStartingWorldLoc
    {
        get { return _startingWorldLoc; }
        set { _startingWorldLoc = value; }
    }

    public CubeLocationScript CubeUnitIsOn
    {
        get { return _cubeUnitIsOn; }
        set { _cubeUnitIsOn = value; }
    }

    public int PlayerControllerID
    {
        get { return _playerControllerId; }
        set { _playerControllerId = value; }
    }

    public NetworkInstanceId NetID
    {
        get { return _netID; }
        set { _netID = value; }
    }

    public UnitData UnitData
    {
        get { return _unitData; }
        set { _unitData = value; }
    }

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


	// Use this for initialization
	void Start () {
        _pathFindingNodes = new List<CubeLocationScript>();
        _rends = GetComponentsInChildren<Renderer> ();
	}
		

	public void PanelPieceChangeColor(string color) {

		foreach (Renderer rend in _rends) {
			switch (color) {
			case "Red":
				rend.material.color = Color.red;
				break;
			case "Black":
				rend.material.color = Color.black;
				break;
			case "White":
				rend.material.color = Color.white;
				break;
			case "Green":
				rend.material.color = Color.green;
				break;
			default:
				break;
			}
		}
	}




	public void ActivateUnit(bool onOff) {

        if (onOff)
        {
            _gameManager._playerManager._playerObject.GetComponent<UnitsAgent>().SetUnitActive(true, this);
            PanelPieceChangeColor("Red");
        }
        else
        {
            _gameManager._playerManager._playerObject.GetComponent<UnitsAgent>().SetUnitActive(false);
            PanelPieceChangeColor("White");
        }
        _unitActive = onOff;
    }


    void OnMouseDown()
    {
        //if (!isLocalPlayer) return;
        if (PlayerControllerID == _gameManager._playerManager._playerObject.GetComponent<PlayerAgent>().PlayerID)
        {
            if (!_unitActive)
            {
                ActivateUnit(true);
            }
            else
            {
                ActivateUnit(false);
            }
        }
	}

	void OnMouseOver() {
		if (!_unitActive) {
		//	if (cubeScript.cubeVisible) {
			PanelPieceChangeColor ("Green");
		//		cubeScript.CubeHighlight ("Move");
		//	}
		}
	}
	void OnMouseExit() {
		if (!_unitActive) {
	//	if (cubeScript.cubeVisible) {
			PanelPieceChangeColor ("White");
	//		cubeScript.CubeUnHighlight ("Move");
	//	}
		}
	}


    public void AssignPathFindingNodes(List<CubeLocationScript> nodes)
    {
        //ClearPathFindingNodes();
        _pathFindingNodes = nodes;
    }

    public void ClearPathFindingNodes()
    {
        foreach(CubeLocationScript node in _pathFindingNodes)
        {
            node.DestroyPathFindingNode();
        }
        _pathFindingNodes.Clear();
    }
}
