﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitScript : NetworkBehaviour
{
    ////////////////////////////////////////////////

    GameManager _gameManager;
    UnitsManager _unitsManager;

    ////////////////////////////////////////////////

    // GamePlay data
    private List<CubeLocationScript> _pathFindingNodes;
    private bool _unitActive;
    // Visual
    private Renderer[] _rends;


    // Unit stats
    private int _unitModel;
    private bool _unitCanClimbWalls;
    private int[] _unitCombatStats;
    private Vector3 _startingWorldLoc;
    private int _playerControllerId;
    private NetworkInstanceId _netID;
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

    public int[] UnitCombatStats
    {
        get { return _unitCombatStats; }
        set { _unitCombatStats = value; }
    }

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

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {

        _pathFindingNodes = new List<CubeLocationScript>();
        _rends = GetComponentsInChildren<Renderer> ();

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _unitsManager = _gameManager._unitsManager;
        if (_unitsManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

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
            _unitsManager.SetUnitActive(true, this);
            PanelPieceChangeColor("Red");
        }
        else
        {
            _unitsManager.SetUnitActive(false);
            PanelPieceChangeColor("White");
        }
        _unitActive = onOff;
    }


    void OnMouseDown()
    {
        //if (!isLocalPlayer) return;
        if (PlayerControllerID == _gameManager._playerManager.PlayerID)
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
