using UnityEngine;

public class CubeConnections : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static CubeConnections _instance;

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

    public static void SetCubeNeighbours(CubeLocationScript cubeScript)
    {
        if (cubeScript == null)
        {
            Debug.LogError("cubeScript == null! not sure what this means just yet");
        }
        else
        {
            // If any of the half neighbours are panels, then this cubeScript location can be set to walkable/climable...etc
            foreach (Vector3 vect in cubeScript.NeighbourHalfVects)
            {
                CubeLocationScript neighbourScript = LocationManager.GetHalfLocationScript(vect);
                // setup Panels in cubes
                if (neighbourScript != null && neighbourScript._isPanel)
                {
                   // Debug.Log("fuck we HAVE A PANEL in half neighbour");
                    SetUpPanelInCube(neighbourScript);
                    break;
                }
            }
        }
    }



	// If ANY kind of wall/floor/object make neighbour cubes walkable
	public static void SetUpPanelInCube(CubeLocationScript cubeScript) {

		PanelPieceScript panelScript = cubeScript._panelScriptChild;

        switch (panelScript.name) 
		{
		case "Floor":
			SetUpFloorPanel (cubeScript, panelScript);
			break;
		case "Wall":
			SetUpWallPanel (cubeScript, panelScript);
			break;
		case "FloorAngle": // angles put in half points
			SetUpFloorAnglePanel (cubeScript, panelScript);
			break;
		case "CeilingAngle": // This is the exact same as ceilingFloor
			SetUpCeilingAnglePanel (cubeScript, panelScript);
			break;
		default:
			Debug.Log ("fuck no issue:  " + panelScript.name);
			break;
		}
	}




	private static void SetUpFloorPanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 leftVect, rightVect;
		CubeLocationScript cubeScriptLeft = null;
		CubeLocationScript cubeScriptRight = null;

        Vector3 cubeLoc = cubeScript.CubeLocVector;

        leftVect = new Vector3 (cubeLoc.x, cubeLoc.y - 1, cubeLoc.z);
		cubeScriptLeft = LocationManager.GetLocationScript(leftVect); // underneath panel
		if (cubeScriptLeft != null) {
			panelScript.cubeScriptLeft = cubeScriptLeft;
			panelScript.cubeLeftVector = leftVect;
			panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);
			if (!cubeScriptLeft._isPanel) {
                cubeScriptLeft.IsAlienWalkable = true;
            }
			// make edges empty spaces for climbing over
			MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // South
			MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
			MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // North
			MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East
		}


        rightVect = new Vector3(cubeLoc.x, cubeLoc.y + 1, cubeLoc.z);
        cubeScriptRight = LocationManager.GetLocationScript(rightVect); // Ontop of panel
        if (cubeScriptRight != null)
        {
            panelScript.cubeScriptRight = cubeScriptRight;
            panelScript.cubeRightVector = rightVect;
            panelScript.rightPosNode = new Vector3(0, 0, 4.5f);
            if (!cubeScriptRight._isPanel)
            {
                cubeScriptRight.IsHumanWalkable = true;
                cubeScriptRight.IsAlienWalkable = true;
            }
            // make edges empty spaces for climbing over
            MakeClimbableEdges(new Vector3(rightVect.x, rightVect.y, rightVect.z - 2)); // South
            MakeClimbableEdges(new Vector3(rightVect.x - 2, rightVect.y, rightVect.z)); // West
            MakeClimbableEdges(new Vector3(rightVect.x, rightVect.y, rightVect.z + 2)); // North
            MakeClimbableEdges(new Vector3(rightVect.x + 2, rightVect.y, rightVect.z)); // East
        }

        // 8 points for each panel
        /*
        DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z - 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z + 0));
		// middle
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z + 0));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z + 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y, cubeLoc.z + 1));
		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z + 1));
        */

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
	}


	private static void SetUpWallPanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.CubeLocVector;

		Vector3 leftVect, rightVect;
		CubeLocationScript cubeScriptLeft = null;
		CubeLocationScript cubeScriptRight = null;

		int cubeAngle = cubeScript.CubeAngle;
		int panelAngle = panelScript.panelAngle;

		//panelScript._isLadder = true;

		int result = (cubeAngle - panelAngle);
		result = (((result + 180) % 360 + 360) % 360) - 180;
		//Debug.Log ("cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " result: " + result);

		if (result == 180 || result == -180 || result == 0) { // Down
			leftVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z - 1);
			cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);
				if (panelScript._isLadder ) {
					cubeScriptLeft.IsHumanClimbable = true;
                }
                cubeScriptLeft.IsAlienClimbable = true;
                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East
                */
            }

			rightVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z + 1);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);
				if (panelScript._isLadder && !cubeScriptRight._isPanel) {
					cubeScriptRight.IsHumanClimbable = true;
				}
                cubeScriptRight.IsAlienClimbable = true;
                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y - 2, rightVect.z)); // South
				MakeClimbableEdges (new Vector3 (rightVect.x - 2, rightVect.y, rightVect.z)); // West
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y + 2, rightVect.z)); // North
				MakeClimbableEdges (new Vector3 (rightVect.x + 2, rightVect.y, rightVect.z)); // East
                */
            }

			// 8 points for each panel
            /*
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
			// middle
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z + 0));
            */

		} else if (result == 90 || result == -90) { //across 

			leftVect = new Vector3 (cubeLoc.x - 1, cubeLoc.y, cubeLoc.z);
			cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (-4.5f, 0, 0);
				if (panelScript._isLadder && !cubeScriptLeft._isPanel) {
                    cubeScriptLeft.IsHumanClimbable = true;
				}
                cubeScriptLeft.IsAlienClimbable = true;
                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // East
                */
            }

			rightVect = new Vector3 (cubeLoc.x + 1, cubeLoc.y, cubeLoc.z);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (4.5f, 0, 0);
				if (panelScript._isLadder) {
                    cubeScriptRight.IsHumanClimbable = true;
				}
                cubeScriptRight.IsAlienClimbable = true;
                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y - 2, rightVect.z)); // South
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z - 2)); // West
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y + 2, rightVect.z)); // North
				MakeClimbableEdges (new Vector3 (rightVect.x, rightVect.y, rightVect.z + 2)); // East
                */
            }

			// 8 points for each panel
            /*
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z - 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 0, cubeLoc.z - 1));
			// middle
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 0, cubeLoc.z + 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z - 1));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 0));
			DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 1));
            */
		} else {
			Debug.Log ("SOMETHING weird: cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
		}

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
		}
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode; 
		}
	}


	private static void SetUpFloorAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.CubeLocVector;

		//cubeScript._isPanel = false; // this might cause issues

		Vector3 centerVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z);
		panelScript.cubeScriptLeft = cubeScript;
		panelScript.cubeLeftVector = centerVect;
		panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

		panelScript.cubeScriptRight = cubeScript;
		panelScript.cubeRightVector = centerVect;
		panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);

		//cubeScript._isHumanWalkable = true;

		//		// 8 points for each panel
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		// middle
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z + 1));
	}


	private static void SetUpCeilingAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript) {

		Vector3 cubeLoc = cubeScript.CubeLocVector;

		//cubeScript._isPanel = false; // this might cause issues

		Vector3 centerVect = new Vector3 (cubeLoc.x, cubeLoc.y, cubeLoc.z);
		panelScript.cubeScriptLeft = cubeScript;
		panelScript.cubeLeftVector = centerVect;
		panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

		panelScript.cubeScriptRight = cubeScript;
		panelScript.cubeRightVector = centerVect;
		panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);

		//cubeScript._isHumanWalkable = true;
		//		// 8 points for each panel
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 1, cubeLoc.z - 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		// middle
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y + 0, cubeLoc.z + 0));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x - 1, cubeLoc.y - 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 0, cubeLoc.y - 1, cubeLoc.z + 1));
		//		DoHalfPointsForWalls (new Vector3 (cubeLoc.x + 1, cubeLoc.y - 1, cubeLoc.z + 1));
	}

    /*
	private void DoHalfPointsForWalls(Vector3 nodeVect) {

		CubeLocationScript nodeScript = LocationManager.GetLocationScript(nodeVect);
		if (nodeScript != null) {
			nodeScript._isPanel = true;
			nodeScript.IsHumanWalkable = false;
		}
	}
    */


	private static void MakeClimbableEdges(Vector3 nodeVect) {

		CubeLocationScript nodeScript = LocationManager.GetLocationScript(nodeVect);
		if (nodeScript != null) {
			if (!nodeScript._isPanel) {
				//nodeScript.IsHumanClimbable = true; 
			}
		}
	}
}
