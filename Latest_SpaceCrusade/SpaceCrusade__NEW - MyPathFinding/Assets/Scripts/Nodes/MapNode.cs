using System.Collections.Generic;
using UnityEngine;

public class MapNode : BaseNode {

    public List<int[,]> mapFloorData = new List<int[,]>();
    public List<int[,]> mapVentData = new List<int[,]>();




    void Awake()
    {
        thisNodeType = NodeTypes.MapNode;
    }

    public void RemoveDoorPanels()
    {
        Debug.Log("removing door panels");
    }



}
