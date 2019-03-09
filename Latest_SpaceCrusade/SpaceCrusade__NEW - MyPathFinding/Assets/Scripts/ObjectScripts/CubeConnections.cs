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

    // an attempt to make cubes as platforms FROM the neighbouring cubes with panels
    // the panel half cube will be passed into this
    public static void SetCubeHalfNeighbours(CubeLocationScript cubeHalfScript)
    {
        if (cubeHalfScript != null)
        {
            SetUpPanelInCube(cubeHalfScript);
        }

        // so this is essentially going through the proper neighbour cubes around the half panel cube
        foreach (Vector3 vect in cubeHalfScript.NeighbourHalfVects)
        {
            CubeLocationScript cubeScript = LocationManager.GetLocationScript(vect);

            if (cubeScript != null)
            {
                cubeScript.SetNeighbourVects();
                cubeScript.CubePlatform = true;
                cubeScript.NeighboursSet = true;
            }
        }
    }

    ////////////////////////////////////////////////


    // If ANY kind of wall/floor/object make neighbour cubes walkable
    public static void SetUpPanelInCube(CubeLocationScript neighbourHalfScript) {

		PanelPieceScript panelScript = neighbourHalfScript._panelScriptChild;

        switch (panelScript.name) 
		{
		case "Floor":
			SetUpFloorPanel (neighbourHalfScript, panelScript);
			break;
		case "Wall":
			SetUpWallPanel (neighbourHalfScript, panelScript);
			break;
		case "FloorAngle": // angles put in half points
			SetUpFloorAnglePanel (neighbourHalfScript, panelScript);
			break;
		case "CeilingAngle": // This is the exact same as ceilingFloor
			SetUpCeilingAnglePanel (neighbourHalfScript, panelScript);
			break;
		default:
			Debug.Log ("fuck no issue:  " + panelScript.name);
			break;
		}
	}


    private static void SetHumanCubeRules(CubeLocationScript cubeScript, bool walkable, bool climable, bool jumpable)
    {
        cubeScript.IsHumanWalkable = walkable;
        cubeScript.IsHumanClimbable = climable;
        cubeScript.IsHumanJumpable = jumpable;
    }
    private static void SetAlienCubeRules(CubeLocationScript cubeScript, bool walkable, bool climable, bool jumpable)
    {
        cubeScript.IsAlienWalkable = walkable;
        cubeScript.IsAlienClimbable = climable;
        cubeScript.IsAlienJumpable = jumpable;
    }



    private static void SetUpFloorPanel(CubeLocationScript neighbourHalfScript, PanelPieceScript panelScript) {

        Vector3 cubeHalfLoc = neighbourHalfScript.CubeLocVector;

        Vector3 leftVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y - 1, cubeHalfLoc.z);
        CubeLocationScript cubeScriptLeft = LocationManager.GetLocationScript(leftVect); // underneath panel
		if (cubeScriptLeft != null) {
            panelScript.cubeScriptLeft = cubeScriptLeft;
			panelScript.cubeLeftVector = leftVect;
			panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

            SetHumanCubeRules(cubeScriptLeft, false, false, false);
            SetAlienCubeRules(cubeScriptLeft, true, true, true);

            // make edges empty spaces for climbing over
            MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // South
			MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
			MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // North
			MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East
		}


        Vector3 rightVect = new Vector3(cubeHalfLoc.x, cubeHalfLoc.y + 1, cubeHalfLoc.z);
        CubeLocationScript cubeScriptRight = LocationManager.GetLocationScript(rightVect); // Ontop of panel
        if (cubeScriptRight != null)
        {
            panelScript.cubeScriptRight = cubeScriptRight;
            panelScript.cubeRightVector = rightVect;
            panelScript.rightPosNode = new Vector3(0, 0, 4.5f);

            SetHumanCubeRules(cubeScriptRight, true, true, true);
            SetAlienCubeRules(cubeScriptRight, true, true, true);

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


	private static void SetUpWallPanel(CubeLocationScript neighbourHalfScript, PanelPieceScript panelScript) {

        CubeLocationScript cubeScriptLeft = null;
        CubeLocationScript cubeScriptRight = null;

        Vector3 cubeHalfLoc = neighbourHalfScript.CubeLocVector;

        int cubeAngle = neighbourHalfScript.CubeAngle;
		int panelAngle = panelScript.panelAngle;

		//panelScript._isLadder = true;

		int result = (cubeAngle - panelAngle);
		result = (((result + 180) % 360 + 360) % 360) - 180;
		//Debug.Log ("cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " result: " + result);

		if (result == 180 || result == -180 || result == 0) { // Down
            Vector3 leftVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y, cubeHalfLoc.z - 1);
            cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (0, 0, -4.5f);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptLeft, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptLeft, false, false, false);
                }

                SetAlienCubeRules(cubeScriptLeft, true, true, true);

                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x - 2, leftVect.y, leftVect.z)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x + 2, leftVect.y, leftVect.z)); // East
                */
            }

            Vector3 rightVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y, cubeHalfLoc.z + 1);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (0, 0, 4.5f);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptRight, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptRight, false, false, false);
                }

                SetAlienCubeRules(cubeScriptLeft, true, true, true);
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

            Vector3 leftVect = new Vector3 (cubeHalfLoc.x - 1, cubeHalfLoc.y, cubeHalfLoc.z);
			cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (-4.5f, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptLeft, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptLeft, false, false, false);
                }

                // make edges empty spaces for climbing over
                /*
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y - 2, leftVect.z)); // South
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z - 2)); // West
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y + 2, leftVect.z)); // North
				MakeClimbableEdges (new Vector3 (leftVect.x, leftVect.y, leftVect.z + 2)); // East
                */
            }

            Vector3 rightVect = new Vector3 (cubeHalfLoc.x + 1, cubeHalfLoc.y, cubeHalfLoc.z);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (4.5f, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptRight, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptRight, false, false, false);
                }

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
