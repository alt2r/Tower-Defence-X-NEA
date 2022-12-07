using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FlyingEnemy : MonoBehaviour
{
    float speed = 10f;
    private int targetIndex;
    Grid grid;
    List<Tile> path = new List<Tile>();
    private bool atEnd = false;
    public bool IsAtEnd() { return atEnd; }
    private float height = 4f;
    bool pink;
    float pinktimer;

    private float nonIceSpeed;
    public Enemy thisEnemy;
    private string colour;
    Renderer rend;
    bool iceDebuff = false;
    float iceTimer;

    public Image healthBar;

    public GameObject iceEffect;
    public GameObject pinkEffect;

    void Start()
    {
        thisEnemy = GameMaster.enemyList[GameMaster.enemyList.Count - 1];
        rend = GetComponent<Renderer>();
        colour = thisEnemy.GetColour();
        switch (colour)
        {
            case "Blue":
                rend.material.color = Color.blue;
                speed = 10;
                break;
            case "Yellow":
                rend.material.color = Color.yellow;
                speed = 15;
                break;
            case "Green":
                rend.material.color = Color.green;
                speed = 20;
                break;
            case "Red":
                rend.material.color = Color.red;
                speed = 15;
                break;
            case "Pink":
                rend.material.color = Color.magenta;
                speed = 12;
                pink = true;
                break;
            case "Orange":
                rend.material.color = new Color(0.92f, 0.49f, 0);
                speed = 15;
                InvokeRepeating("Orange", 0f, 0.5f);
                break;
            default:
                break;
                
        }
        speed *= 0.6f; //BALANCE CHANGE - flying enemies were too powerful 
        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, 0.3f);
        nonIceSpeed = speed;

        grid = Grid.instance;
        PathFind(Convert.ToInt32(transform.position.x / 4.5), Convert.ToInt32(transform.position.z / 4.5));
        //allows the enemy to pathfind from any position, 4.5 is the size of a tile + a gap in the game world (4 + 0.5)
        path.Add(grid.gridArray[14, 15]); //needs a buffer
    }

    void Orange()
    {
        speed *= 1.025f;
    }
    void Update()
    {

        if (iceDebuff)
        {
            if (iceTimer <= 0)
            {
                speed = nonIceSpeed;
                iceDebuff = false;
            }
            iceTimer -= Time.deltaTime;
            GameObject effectIns = (GameObject)Instantiate(iceEffect, transform.position, transform.rotation);
            Destroy(effectIns, 0.25f);
        }
        if (pink) //health is stored under general enemy class, which doesnt inherit from monobehavior, so we cant invokerepeating we have to do this 
        {
            if (pinktimer <= 0)
            {
                pinktimer = 1f;
                thisEnemy.Pink();
                GameObject effectIns = (GameObject)Instantiate(pinkEffect, transform.position, transform.rotation);
                Destroy(effectIns, 0.25f);
            }
            pinktimer -= Time.deltaTime;
        }

        if (this.transform.position.y < -400)
        {
            Destroy(gameObject);
        }

        Vector3 dir = path[targetIndex].getPos() - transform.position + new Vector3(0f, height, 0f); //new vector3 stops them going under the map
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World); //normalized converts it to a unit vector
        //i need a unit vector here as we only want the direction, not the magitude. Unit vector means vector with magnitude 1

        if (Vector3.Distance(transform.position, (path[targetIndex].getPos() + new Vector3(0f, height, 0f))) <= 0.5f && targetIndex < path.Count - 1) 
            //if we are very close to the waypoint. Doing this instead of transform.position = target.getposition() is more robust 
        {
            targetIndex++;
        }
        else if (gameObject.transform.position.x > 14.2 * 4.5 || gameObject.transform.position.z > 14.2 * 4.5) //if we are at the end
            //14.2 because it needs a buffer, would otherwise be 14. *4.5 bbecause thats the size of a tile (4) + the size of a gap (0.5)
        {
            GameMaster.enemyList.Remove(thisEnemy);
            GameMaster.playerHealth -= thisEnemy.GetDamage();
            Destroy(gameObject); //deletes the enemy game object when we get to the end
            return;
        }
    }
    public void PathFind(int x, int z)
    {
        if ((x == 14 && z == 14) || atEnd == true)
        {
            atEnd = true; //global variable that gets set to true for all calls of PathFind()
            return;
        }
        if (grid.gridArray[x, z].GetConnection(0).GetDanger() > grid.gridArray[x, z].GetConnection(1).GetDanger() && z < 14)
        //If connection 1 is safer than connection 0
        {
            path.Add(grid.gridArray[x, z].GetConnection(1)); //getconnection(0) is to thr right of the current node, getconnection(1) is above
            PathFind(x, z + 1); //repeat on node above
        }
        else if (grid.gridArray[x, z].GetConnection(0).GetDanger() < grid.gridArray[x, z].GetConnection(1).GetDanger() && x < 14)
        {
            path.Add(grid.gridArray[x, z].GetConnection(0)); //add the node to the right to the path
            PathFind(x + 1, z); //repeat on node to the right
        }
        else //if the danger levels are the same
        {
            if (x >= z && z < 14)
            {
                path.Add(grid.gridArray[x, z].GetConnection(1)); 
                PathFind(x, z + 1);
            } 
            else if(x < 14) //need to make sure it stays in bounds 0 < x < 15
            {
                path.Add(grid.gridArray[x, z].GetConnection(0));
                PathFind(x + 1, z);
            }
        }
    }
    public void editHealthBar(float health, float maxhealth)
    {
        healthBar.fillAmount = health / maxhealth;
    }
    public void IceDebuff(float speedReduction, float duration)
    {
        if (iceDebuff)
        {
            iceTimer = duration;
            return;
        }
        iceDebuff = true;
        speed *= speedReduction;
        iceTimer = duration;

    }
}
