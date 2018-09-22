using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour {

	UnitsAgent _unitsAgent;

	// Data
	public List<CubeLocationScript> movePath = new List<CubeLocationScript>();

	bool _unitActive = false;

	public bool _unitCanClimbWalls = false;

	public int[] _unitStats = new int[1];

	public CubeLocationScript _cubeUnitIsOn = null;


	// Visual
	Renderer[] _rends;


	void Awake() {
		
		_unitsAgent = transform.parent.GetComponent<UnitsAgent> ();
		if(_unitsAgent == null){Debug.LogError ("OOPSALA we have an ERROR!");}
	}

	// Use this for initialization
	void Start () {
		_rends = GetComponentsInChildren<Renderer> ();

		_unitCanClimbWalls = true;
		_unitStats [0] = 4;// Movement
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
//
//		if (onOff) {
//			_unitsAgent.SetUnitActive (true, this.gameObject);
//			PanelPieceChangeColor ("Red");
//		} else {
//			_unitsAgent.SetUnitActive (false);
//			PanelPieceChangeColor ("White");
//		}
//		_unitActive = onOff;
	}


	void OnMouseDown() {
		if (!_unitActive) {
			ActivateUnit (true);
		} else {
			ActivateUnit (false);
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

//	public void PanelPieceGoTransparent() {
//
//		if (_rend) {
//			_rend.material.shader = Shader.Find ("Transparent/Diffuse");
//			Color tempColor = _rend.material.color;
//			tempColor.a = 0.3F;
//			_rend.material.color = tempColor;
//		}
//	}
}
