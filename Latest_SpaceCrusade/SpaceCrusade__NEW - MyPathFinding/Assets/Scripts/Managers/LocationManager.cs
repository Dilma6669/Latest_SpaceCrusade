using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LocationManager _instance;

    ////////////////////////////////////////////////

    public CubeConnections _cubeConnections;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    UnitsManager _unitsManager;

    ////////////////////////////////////////////////

    public Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();
    public Dictionary<int, CubeLocationScript> _unitLocation = new Dictionary<int, CubeLocationScript>();

    private CubeLocationScript _activeCube = null; // hmmm dont know if should be here

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
        if (_cubeConnections == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _unitsManager = _gameManager._unitsManager;
        if (_unitsManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void AddCubeScriptToLocationLookup(Dictionary<Vector3, CubeLocationScript> locs)
    {
        foreach (Vector3 vect in locs.Keys)
        {
            if (!_LocationLookup.ContainsKey(vect))
            {
                //Debug.Log("fucken adding to vect: " + vect + " script: " + locs[vect]);
                _LocationLookup.Add(vect, locs[vect]);
            }
            else
            {
                Debug.LogError("trying to use already taking location!!!");
            }
        }
    }



    public CubeLocationScript GetLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationLookup.ContainsKey(loc)) {
            return _LocationLookup[loc];
		}
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
	}


    public CubeLocationScript CheckIfCanMoveToCube(UnitScript unit, CubeLocationScript node, Vector3 neighloc)
    {
        //Debug.Log("CheckIfCanMoveToCube loc: " + neighloc);

        CubeLocationScript cubeScript = GetLocationScript(neighloc);

        if (cubeScript == null)
        {
            //Debug.LogError("FAIL move cubeScript == null");
            return null;
        }

        if (cubeScript.CubeOccupied)
        {
            //Debug.LogWarning("FAIL move Cube is Occupied at vect:" + neighloc);
            return null;
        }

        if (cubeScript._isPanel) // this might cause problems
        {
            //Debug.LogWarning("FAIL move  cubeScript _isPanel");
            return null;
        }

        if (!unit.UnitCanClimbWalls)  // if human
        {
            if (cubeScript.IsHumanWalkable == false && cubeScript.IsHumanClimbable == false && cubeScript.IsHumanJumpable == false)
            {
                //Debug.LogWarning("FAIL move error");
                return null;
            }
        }
        else // alien
        {
            if (cubeScript.IsAlienWalkable == false && cubeScript.IsAlienClimbable == false && cubeScript.IsAlienJumpable == false)
            {
               //Debug.LogWarning("FAIL move error");
                return null;
            }
        }
        if (unit.CubeUnitIsOn != null)
        {
            int neighHalfIndex = node.NeighbourVects.IndexOf(neighloc);
            Vector3 neighHalfVect = node.NeighbourHalfVects[neighHalfIndex];
            CubeLocationScript neighbourHalfScript = GetLocationScript(neighHalfVect);

            if (neighbourHalfScript == null)
            {
                //Debug.LogWarning("FAIL move neighbourHalfScript == null");
                return null;
            }

            if (neighbourHalfScript._isPanel)
            {
                //Debug.LogWarning("FAIL move neighbourHalfScript._isPanel");
                return null;
            }
        }

        //Debug.Log("SUCCES move to loc: " + neighloc);

        // success
        return cubeScript;
    }



    public bool SetUnitOnCube(UnitScript unitScript, Vector3 loc)
    {
        //Debug.Log("SetUnitOnCube");

        int unitNetId = (int)unitScript.NetID.Value;
        CubeLocationScript cubescript = CheckIfCanMoveToCube(unitScript, unitScript.CubeUnitIsOn, loc);
        if (cubescript != null)
        {
            if (!_unitLocation.ContainsKey(unitNetId))
            {
                _unitLocation.Add(unitNetId, cubescript);
            }
            else
            {
                CubeLocationScript oldCubescript = _unitLocation[unitNetId];
                oldCubescript.CubeOccupied = false;
                oldCubescript.FlagToSayIsMine = null;
                _unitLocation[unitNetId] = cubescript;
            }
            cubescript.CubeOccupied = true;
            unitScript.CubeUnitIsOn = cubescript;
            return true;
        }
        else
        {
            Debug.LogError("Unit cannot move to a location");
            return false;
        }
    }



    ////// Dont think this should be here
    public void SetCubeActive(bool onOff, Vector3 cubeVect = new Vector3())
    {
        if (_activeCube)
        {
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(false);
            _activeCube = null;
        }

        if (onOff)
        {
            _activeCube = GetLocationScript(cubeVect);
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(true);
            _unitsManager.MakeActiveUnitMove(cubeVect);
        }
    }

    // tries to spawn visual nodes in all current moveable locations for a unit
    public void DebugTestPathFindingNodes(UnitScript unit)
    {
        foreach (KeyValuePair<Vector3, CubeLocationScript> element in _LocationLookup)
        {
            CubeLocationScript script = CheckIfCanMoveToCube(unit, unit.CubeUnitIsOn, element.Key);
            if (script != null)
            {
                script.CreatePathFindingNode((int)unit.NetID.Value);
            }
        }
    }
}
