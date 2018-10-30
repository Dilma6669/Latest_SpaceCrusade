using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    GameManager _gameManager;

    bool _unitCanClimbWalls = true;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    // My personal checks for pathfinding
    private bool PathFindingChecks(CubeLocationScript neightbourScript, CubeLocationScript neightbourHalfScript) {

        if (neightbourHalfScript == null || neightbourScript == null) {
            return false;
        }

        if (neightbourScript._isPanel) // this might cause problems
        { 
            return false;
        }

        if (neightbourHalfScript._isPanel)
        {
            return false;
        }

        if (neightbourScript._cubeOccupied)
        {
            return false;
        }

        if (!_unitCanClimbWalls)  // if human
        {
            if (!neightbourScript._isHumanWalkable && !neightbourScript._isHumanClimbable && !neightbourScript._isHumanJumpable)
            {
                return false;
            }
        }

		return true;
	}
		

	public List<CubeLocationScript> FindPath(int movement, bool canClimbWalls, Vector3 startVect, Vector3 targetVect, Vector3 posOffset) {

		_unitCanClimbWalls = canClimbWalls;

		CubeLocationScript cubeStartScript = _gameManager._locationManager.GetLocationScript(startVect);
		CubeLocationScript cubeTargetScript = _gameManager._locationManager.GetLocationScript(targetVect);

        List<CubeLocationScript> openSet = new List<CubeLocationScript>();
		openSet.Clear ();
		HashSet<CubeLocationScript> closedSet = new HashSet<CubeLocationScript>();
		closedSet.Clear ();

		openSet.Add(cubeStartScript);

        cubeStartScript.AssignCubeNeighbours();
        cubeTargetScript.AssignCubeNeighbours();

        while (openSet.Count > 0) {
			CubeLocationScript node = openSet [0];
			for (int i = 0; i < openSet.Count; i++) {
				if (openSet [i].fCost < node.fCost || openSet [i].fCost == node.fCost) {
					if (openSet [i].hCost < node.hCost)
						node = openSet [i];
				}
			}

			openSet.Remove (node);
			closedSet.Add (node);

			if (node == cubeTargetScript) {
                return RetracePath (movement, cubeStartScript, cubeTargetScript);
			}

            node.AssignCubeNeighbours();

            List<Vector3> neighVects = node.neighVects;
			List<Vector3> neighHalfVects = node.neighHalfVects;

			for (int i = 0; i < neighVects.Count; i++) {

				Vector3 neighbourVect = neighVects [i];
				Vector3 neighbourHalfVect = neighHalfVects [i];

				if (!_gameManager._locationManager.CheckIfLocationExists(neighbourVect)) {
                    continue;
				}
				CubeLocationScript neightbourScript = _gameManager._locationManager.GetLocationScript(neighbourVect);
                neightbourScript.AssignCubeNeighbours();
                CubeLocationScript neighbourHalfScript = _gameManager._locationManager.GetLocationScript(neighbourHalfVect);
                if (neighbourHalfScript != null)
                {
                    neighbourHalfScript.AssignCubeNeighbours();
                }

                if (closedSet.Contains (neightbourScript)) {
                    continue;
				}

				// personal checks
				if (!PathFindingChecks (neightbourScript, neighbourHalfScript)) {
                    continue;
				}
						
				int newCostToNeighbour = node.gCost + GetDistance (node, neightbourScript);
				if (newCostToNeighbour < neightbourScript.gCost || !openSet.Contains (neightbourScript)) {
					neightbourScript.gCost = newCostToNeighbour;
					neightbourScript.hCost = GetDistance (neightbourScript, cubeTargetScript);
					neightbourScript.parentPathFinding = node;

					if (!openSet.Contains (neightbourScript))
						openSet.Add (neightbourScript);
				}
			}
		}
		Debug.Log ("SHIT NO WAY OF GETTING TO THAT SPOT!");
		return null;
	}

	private List<CubeLocationScript> RetracePath(int movement, CubeLocationScript startNode, CubeLocationScript endNode) {
		List<CubeLocationScript> path = new List<CubeLocationScript>();
		CubeLocationScript currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parentPathFinding;
		}
		path.Reverse();

		List<CubeLocationScript> _finalPath = new List<CubeLocationScript>();

		int pathLength = movement;
		if (movement > path.Count) {
			movement = path.Count;
		}

		for(int i = 0; i < movement; i++) {
            _finalPath.Add(path[i]);
        }
			
		foreach (CubeLocationScript script in _finalPath) {
            script.CreatePathFindingNode(); // puts circles in path, visual reference
		}
		return _finalPath;

	}

	int GetDistance(CubeLocationScript nodeA, CubeLocationScript nodeB) {
		int dstX = (int)Mathf.Abs(nodeA.cubeLoc.x - nodeB.cubeLoc.x);
		int dstY = (int)Mathf.Abs(nodeA.cubeLoc.y - nodeB.cubeLoc.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}