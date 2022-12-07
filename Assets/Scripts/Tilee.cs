using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    GameObject thisTile;
    bool isPath = false;
    private List<Tile> connections = new List<Tile>();
    int danger = 0;
    Vector3 realPos;
    public Tile(GameObject tileObject, Vector3 pos, Transform parent)
    {
        thisTile = GameObject.Instantiate(tileObject, pos, new Quaternion(0, 0, 0, 0), parent);
        realPos = thisTile.transform.position;
    }
    public void SetAsPath()
    {
        isPath = true;
        GameObject.Destroy(thisTile);
    }
    public bool IsPath()
    {
        return isPath;
    }

    public void AddConnection(Tile x)
    {
        connections.Add(x);
    }
    public Tile GetConnection(int index)
    {
        return connections[index];
    }
    public void IncrementDanger()
    {
        danger++;
    }
    public void DeIncrementDanger()
    {
        danger--;
    }
    public int GetDanger()
    {
        return danger;
    }
    public Vector3 getPos()
    {
        return realPos; //returns the position of the tile in the game world, even if the tile gameobject has been destroyed by the path
    }
}
