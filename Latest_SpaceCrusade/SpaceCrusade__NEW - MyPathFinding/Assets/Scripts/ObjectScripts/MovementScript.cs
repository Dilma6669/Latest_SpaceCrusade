using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementScript : NetworkBehaviour {

    public GameManager _gameManager;

    private bool moveInProgress = false;

    private List<CubeLocationScript> _nodes;

    private bool collision = false;

    public int locCount = 0;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    // Use this for initialization
    void Update() {

        if (moveInProgress) {
            StartMoving();
        }
    }

    private void StartMoving() {

        Vector3 unitCurrPos = transform.position;

        if (locCount < _nodes.Count) {

            CubeLocationScript target = _nodes[locCount];

            if (target != null) {

                Vector3 currTarget = new Vector3(target.cubeLoc.x, target.cubeLoc.y, target.cubeLoc.z);

                if (!target._cubeOccupied || target._flagToSayIsMine == this || target._flagToSayIsMine == null) {

                    target._flagToSayIsMine = this;
                    target._cubeOccupied = true;
                    collision = false;

                    /*
					if (!target.pathFindingNode) {
						target.CreatePathFindingNode (); // puts circles in path, visual reference
					}
                    */

                    if (unitCurrPos != currTarget) {
                        transform.position = Vector3.MoveTowards(unitCurrPos, currTarget, _nodes.Count * Time.deltaTime);
                    } else {
                        /*
						if (target.pathFindingNode) {
							Destroy (target.pathFindingNode);
						}*/
                        target._cubeOccupied = false;
                        target._flagToSayIsMine = null;
                        _nodes[locCount] = null;
                        locCount += 1;
                        if (locCount == _nodes.Count) {
                            FinishMoving();
                        }
                    }
                } else if (target._flagToSayIsMine != this && target._flagToSayIsMine != false) {

                    if (collision == false) {
                        collision = true;
                        CubeLocationScript nodeToRemove = _nodes[_nodes.Count - 1];
                        _nodes.Remove(nodeToRemove);
                    }
                }
            }
        }
    }

    private void FinishMoving() {
        Debug.Log("FINFISHED!");
        moveInProgress = false;
        GetComponent<UnitScript>().movePath.Clear();
        _nodes.Clear();
    }


    public void MoveUnit(int[] pathArray)
    {
        List<CubeLocationScript> nodes = new List<CubeLocationScript>();
        List<Vector3> moveVectors = ConvertPathArrayIntoVectors(pathArray);
        foreach (Vector3 vect in moveVectors)
        {
            CubeLocationScript cubeScript = _gameManager._locationManager.GetLocationScript(vect);
            if (cubeScript != null)
            {
                nodes.Add(cubeScript);
            }
        }
        _nodes = nodes;
        GetComponent<UnitScript>().movePath = nodes;
        moveInProgress = true;
    }


    List<Vector3> ConvertPathArrayIntoVectors(int[] pathArray)
    {
        List<Vector3> moveVectors = new List<Vector3>();

        int index = 0;
        for(int i = 0; i < (pathArray.Length/3); i++)
        {
            Vector3 vect = new Vector3();

            vect.x = pathArray[index];
            index += 1;
            vect.y = pathArray[index];
            index += 1;
            vect.z = pathArray[index];
            index += 1;

            moveVectors.Add(vect);
        }

        return moveVectors;
    }
}
