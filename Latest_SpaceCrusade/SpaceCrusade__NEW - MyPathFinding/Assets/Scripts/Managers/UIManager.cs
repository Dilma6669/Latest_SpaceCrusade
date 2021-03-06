﻿using UnityEngine.UI;
using UnityEngine;


public class UIManager : MonoBehaviour {

    GameManager _gameManager;

    public delegate void ChangeLayerEvent(int change);
	public static event ChangeLayerEvent OnChangeLayerClick;

	public PlayerAgent _playerAgent;

	public Text _playerIDGUINum;

	public Text _playerTotalGUINum;


	// Use this for initialization
	void Awake () {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


	public void ChangeLayer(bool UpDown) {

		if (UpDown) {
			if(OnChangeLayerClick != null)
				OnChangeLayerClick(1);
		} else {
			if(OnChangeLayerClick != null)
				OnChangeLayerClick(-1);
		}
	}


	public void UpdatePlayerGUINum(int ID, int total) {

		_playerIDGUINum.text = ID.ToString();
		_playerTotalGUINum.text = total.ToString();

	}



	//	public void MoveUnits(bool goStop) {
	//
	//		if (goStop) {
	//			_gameManager._movementManager.MoveUnits ();
	//		} else {
	//			_gameManager._movementManager.StopUnits ();
	//		}
	//	}

}
