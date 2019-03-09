using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LocationManager _instance;

    ////////////////////////////////////////////////

    public static Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();   // movable locations
    public static Dictionary<Vector3, CubeLocationScript> _LocationHalfLookup = new Dictionary<Vector3, CubeLocationScript>(); // not moveable locations BUT important for neighbour system

    public static Dictionary<int, CubeLocationScript> _unitLocation = new Dictionary<int, CubeLocationScript>(); // sever shit

    private static CubeLocationScript _activeCube = null; // hmmm dont know if should be here

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

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void SetCubeScriptToLocation(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding to vect: " + vect + " script: " + locs[vect]);
            _LocationLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }

    public static void SetCubeScriptToHalfLocation(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationHalfLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding to vect: " + vect + " script: " + locs[vect]);
            _LocationHalfLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }

    public static CubeLocationScript GetLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationLookup.ContainsKey(loc)) {
            return _LocationLookup[loc];
		}
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
	}

    public static CubeLocationScript GetHalfLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationHalfLookup.ContainsKey(loc))
        {
            return _LocationHalfLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
    }


    public static CubeLocationScript CheckIfCanMoveToCube(UnitScript unit, CubeLocationScript node, Vector3 neighloc)
    {
        //Debug.Log("CheckIfCanMoveToCube loc: " + neighloc);

        CubeLocationScript cubeScript = GetLocationScript(neighloc);

        if (cubeScript == null)
        {
            Debug.LogError("FAIL move cubeScript == null: " + neighloc);
            return null;
        }

        if (!cubeScript.CubeMovable)
        {
            Debug.LogWarning("FAIL move cubeScript not CubeMovable: " + neighloc);
            return null;
        }

        if (cubeScript.CubeOccupied)
        {
            Debug.LogWarning("FAIL move Cube is Occupied at vect:" + neighloc);
            return null;
        }

        if (!unit.UnitCanClimbWalls)  // if human
        {
            if (cubeScript.IsHumanWalkable == false && cubeScript.IsHumanClimbable == false && cubeScript.IsHumanJumpable == false)
            {
                Debug.LogWarning("FAIL move error Human walkable/climable/jumpable: " + neighloc);
                return null;
            }
        }
        else // alien
        {
            if (cubeScript.IsAlienWalkable == false && cubeScript.IsAlienClimbable == false && cubeScript.IsAlienJumpable == false)
            {
                Debug.LogWarning("FAIL move error ALIEN walkable/climable/jumpable: " + neighloc);
                return null;
            }
        }
        /*
        if (unit.CubeUnitIsOn != null)
        {
            int neighHalfIndex = node.NeighbourVects.IndexOf(neighloc);
            Vector3 neighHalfVect = node.NeighbourHalfVects[neighHalfIndex];
            CubeLocationScript neighbourHalfScript = GetLocationScript(neighHalfVect);

            if (neighbourHalfScript == null)
            {
                Debug.LogWarning("FAIL move neighbourHalfScript == null: " + neighloc);
                return null;
            }

            if (neighbourHalfScript._isPanel)
            {
                Debug.LogWarning("FAIL move neighbourHalfScript._isPanel: " + neighloc);
                return null;
            }
        }
        */

        //Debug.Log("SUCCES move to loc: " + neighloc);

        // success
        return cubeScript;
    }



    public static bool SetUnitOnCube(UnitScript unitScript, Vector3 loc)
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
    public static void SetCubeActive(bool onOff, Vector3 cubeVect = new Vector3())
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
            UnitsManager.MakeActiveUnitMove(cubeVect);
        }
    }

    // tries to spawn visual nodes in all current moveable locations for a unit
    public static void DebugTestPathFindingNodes(UnitScript unit)
    {
        foreach (KeyValuePair<Vector3, CubeLocationScript> element in _LocationLookup)
        {
            CubeLocationScript script = CheckIfCanMoveToCube(unit, unit.CubeUnitIsOn, element.Key);

            if (script != null)
            {
                if (script.CubeMovable)
                {
                    script.CreatePathFindingNode((int)unit.NetID.Value);
                }
            }
        }
    }
}
