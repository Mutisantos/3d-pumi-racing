using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint 
{

    public Transform WaypointTransform;
    public int Index;
    public bool Passed;

    public Waypoint(Transform transform, int index)
    {
        WaypointTransform = transform;
        Passed = false;
        this.Index = index;
    }

    public void PassWaypoint(bool isPassed)
    {
        Passed = isPassed;
    }

}
