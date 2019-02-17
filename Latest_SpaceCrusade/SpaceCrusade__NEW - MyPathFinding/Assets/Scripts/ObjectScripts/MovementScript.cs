using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    ////////////////////////////////////////////////

    GameManager _gameManager;
    UnitsManager _unitsManager;
    LocationManager _locationManager;

    ////////////////////////////////////////////////

    private bool moveInProgress = false;

	private List<CubeLocationScript> _nodes;

    private CubeLocationScript _currTarget;
    private Vector3 _currTargetVect;
    private CubeLocationScript _finalTarget;
    private Vector3 _finalTargetVect;
    private bool collision = false;

    private int locCount;

    private int _unitsSpeed;

    private bool _newPath = false;
    private List<CubeLocationScript> _tempNodes;
    private bool _unitInterrupted = false;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {

    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _unitsManager = _gameManager._unitsManager;
        if (_unitsManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _locationManager = _gameManager._locationManager;
        if (_locationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _nodes = new List<CubeLocationScript>();
        _tempNodes = new List<CubeLocationScript>();
    }


    // Use this for initialization
    void Update () {

		if (moveInProgress) {
			StartMoving ();
		}
	}

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    private void StartMoving() {

        if (locCount < _nodes.Count)
        {

            Vector3 unitCurrPos = transform.position;

            if (_currTarget != null)
            {
                _currTarget.FlagToSayIsMine = this;
                collision = false;

                if (unitCurrPos != _currTargetVect)
                {
                    UnitMoveTowardsTarget(unitCurrPos, _currTargetVect);
                }
                else
                {
                    UnitReachedTarget(_currTargetVect);
                }
            }
            else if (_currTarget.FlagToSayIsMine != this && _currTarget.FlagToSayIsMine != false)
            {
                if (collision == false)
                {
                    collision = true;
                }
            }
        }
	}


    private void UnitMoveTowardsTarget(Vector3 unitCurrPos, Vector3 _vectTarget)
    {
        transform.position = Vector3.MoveTowards(unitCurrPos, _vectTarget, Time.deltaTime * _unitsSpeed);
    }


    private void UnitReachedTarget(Vector3 _vectTarget)
    {
        locCount += 1;
        if (locCount == _nodes.Count)
        {
            FinishMoving();
        }
        else
        {
            SetTarget();
        }
    }

	private void FinishMoving() {
		Debug.Log ("FINISHED MOVING!");
        if (_tempNodes.Count > 0)
        {
            SetNewpath();
        }
        else
        {
            Reset();
            moveInProgress = false;
        }
    }

    private void SetTarget()
    {
        if(_unitInterrupted)
        {
            _unitsManager.MakeUnitRecalculateMove(GetComponent<UnitScript>(), _finalTargetVect);
            _unitInterrupted = false;
        }

        if (_nodes.Count > 0)
        {
            if (!_newPath)
            {
                _currTarget = _nodes[locCount];
                _currTargetVect = new Vector3(_currTarget.CubeLocVector.x, _currTarget.CubeLocVector.y, _currTarget.CubeLocVector.z);
                if (!_locationManager.SetUnitOnCube(GetComponent<UnitScript>(), _currTargetVect))
                {
                    Debug.LogWarning("units movement interrupted >> recalculating");
                    _unitInterrupted = true;
                    Reset();
                    StartCoroutine(RecalculateMove(3.0f));
                }
            }
            else
            {
                SetNewpath();
            }
        }
    }


    private void Reset()
    {
        locCount = 0;
        //_currTarget = null;
        //_finalTarget = null;
        //moveInProgress = false;
        _newPath = false;
        foreach(CubeLocationScript node in _nodes)
        {
            node.DestroyPathFindingNode();
        }
        _nodes.Clear();
        //_tempNodes.Clear();
    }


    public void MoveUnit(List<CubeLocationScript> _pathNodes)
    {
        Debug.Log("MoveUnit!");

        int[] stats = GetComponent<UnitScript>().UnitCombatStats;
        _unitsSpeed = stats[0];

        if (_pathNodes.Count > 0)
        {
            _finalTarget = _pathNodes[_pathNodes.Count - 1];
            _finalTargetVect = _finalTarget.CubeLocVector;

            if (moveInProgress)
            {
                _tempNodes = _pathNodes;
                _newPath = true;
            }
            else
            {
                Reset();
                _nodes = _pathNodes;
                moveInProgress = true;
                SetTarget();
            }
        }
    }


    void SetNewpath()
    {
        Reset();
        _nodes = _tempNodes;
        SetTarget();
    }

    private IEnumerator RecalculateMove(float waitTime)
    {
        Debug.Log("IEnumerator RecalculateMove");
        yield return new WaitForSeconds(waitTime);
        _unitsManager.MakeUnitRecalculateMove(GetComponent<UnitScript>(), _finalTargetVect);
    }
}
