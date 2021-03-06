﻿using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [HideInInspector]
    public CameraAgent _cameraAgent;

    // Layer INfo
    int startLayer = 0;
    int maxLayer = 20; // This needs to change with the amout of y levels, basicly level*2 because of vents layer ontop of layer
    int minLayer = 0;


    public int LayerStart
    {
        get { return startLayer; }
        set { startLayer = value; }
    }
    public int LayerMax
    {
        get { return maxLayer; }
        set { maxLayer = value; }
    }
    public int LayerMin
    {
        get { return minLayer; }
        set { minLayer = value; }
    }


	public KeyValuePair<Vector3, Vector3> GetCameraStartPosition(int playerID) {

        //Debug.Log("Finding Camera for player: " + playerID);

		List<KeyValuePair<Vector3, Vector3>> cameraPositions = new List<KeyValuePair<Vector3, Vector3>> ();

		KeyValuePair<Vector3, Vector3> cam0 = new KeyValuePair<Vector3, Vector3> (new Vector3 (-124.3f, 475, 895.9f), new Vector3 (0, 90, 0));
		KeyValuePair<Vector3, Vector3> cam1 = new KeyValuePair<Vector3, Vector3> (new Vector3 (11, 572, -879), new Vector3 (0, 90, 0));
		KeyValuePair<Vector3, Vector3> cam2 = new KeyValuePair<Vector3, Vector3> (new Vector3 (-955, 489.4f, -71), new Vector3 (0, 0, 0));
		KeyValuePair<Vector3, Vector3> cam3 = new KeyValuePair<Vector3, Vector3> (new Vector3 (738, 344, -210), new Vector3 (0, 0, 0));

		cameraPositions.Add (cam0);
		cameraPositions.Add (cam1);
		cameraPositions.Add (cam2);
		cameraPositions.Add (cam3);

		return cameraPositions [playerID];
	}
}
