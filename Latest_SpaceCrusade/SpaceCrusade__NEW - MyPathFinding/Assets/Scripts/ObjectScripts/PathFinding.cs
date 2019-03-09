﻿using UnityEngine;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static PathFinding _instance;

    ////////////////////////////////////////////////

    public static bool _debugPathfindingNodes;

    ////////////////////////////////////////////////

    private static bool _unitCanClimbWalls = true;
    private static List<CubeLocationScript> _previousNodes = new List<CubeLocationScript>();

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

    public static List<CubeLocationScript> FindPath(UnitScript unit, Vector3 startVect, Vector3 targetVect) {

        Debug.Log("FindPath startVect: " + startVect);

        CubeLocationScript cubeStartScript = LocationManager.GetLocationScript(startVect);
		CubeLocationScript cubeTargetScript = LocationManager.GetLocationScript(targetVect);

        List<CubeLocationScript> openSet = new List<CubeLocationScript>();
		openSet.Clear ();
		HashSet<CubeLocationScript> closedSet = new HashSet<CubeLocationScript>();
		closedSet.Clear ();

		openSet.Add(cubeStartScript);
        _previousNodes.Add(cubeStartScript);

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
                return RetracePath (cubeStartScript, cubeTargetScript);
			}

            List<Vector3> neighVects = node.NeighbourVects;

			foreach (Vector3 vect in neighVects) {

                // personal checks
                if (LocationManager.CheckIfCanMoveToCube(unit, node, vect) == null)
                {
                    continue;
                }

                CubeLocationScript neightbourScript = LocationManager.GetLocationScript(vect);

                if (closedSet.Contains (neightbourScript)) {
                    continue;
				}

                if (_debugPathfindingNodes)
                {
                    neightbourScript.CreatePathFindingNode(unit.PlayerControllerID);
                }
                _previousNodes.Add(neightbourScript);

                int newCostToNeighbour = node.gCost + GetDistance (node, neightbourScript);
				if (newCostToNeighbour < neightbourScript.gCost || !openSet.Contains (neightbourScript)) {
					neightbourScript.gCost = newCostToNeighbour;
					neightbourScript.hCost = GetDistance (neightbourScript, cubeTargetScript);
					neightbourScript._parentPathFinding = node;

					if (!openSet.Contains (neightbourScript))
						openSet.Add (neightbourScript);
				}
			}
		}
		Debug.Log ("SHIT NO WAY OF GETTING TO THAT SPOT!");
		return null;
	}

	private static List<CubeLocationScript> RetracePath(CubeLocationScript startNode, CubeLocationScript endNode) {
		List<CubeLocationScript> path = new List<CubeLocationScript>();
		CubeLocationScript currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode._parentPathFinding;
		}
		path.Reverse();
        ResetPath();
        return path;
	}



	private static int GetDistance(CubeLocationScript nodeA, CubeLocationScript nodeB) {
		int dstX = (int)Mathf.Abs(nodeA.CubeLocVector.x - nodeB.CubeLocVector.x);
		int dstY = (int)Mathf.Abs(nodeA.CubeLocVector.y - nodeB.CubeLocVector.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    private static void ResetPath()
    {
        foreach (CubeLocationScript node in _previousNodes)
        {
            //Debug.Log("pathfinding DestroyPathFindingNode: ");
            node.DestroyPathFindingNode();
        }
        _previousNodes.Clear();
    }
}