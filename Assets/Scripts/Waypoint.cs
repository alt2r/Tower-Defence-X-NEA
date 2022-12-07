using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    GameObject thisWaypoint;
    Vector3 position;
    GameObject waypointGO;
    public Waypoint(GameObject WaypointGO, Vector3 Position)
    {
        waypointGO = WaypointGO;
        position = Position;
        //parent = Parent;
    }
    public Vector3 getPosition()
    {
        return position; //for use by the enemies later. subject to change
    }
    public void Activate()
    {
        thisWaypoint = GameObject.Instantiate(waypointGO, position, new Quaternion(0, 0, 0, 0)); //instantiates gameobject
    }
}
