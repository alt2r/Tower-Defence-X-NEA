using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandardEnemy : MonoBehaviour
{
    private Waypoint target;
    private int waypointIndex = 0;
    private float speed = 10f;
    private float nonIceSpeed;
    public Enemy thisEnemy;
    private string colour;
    Renderer rend;
    bool iceDebuff = false;
    float iceTimer;
    bool pink;
    float pinktimer;
    public GameObject pinkEffect;

    private bool atEnd = false;
    public bool IsAtEnd() { return atEnd;  }

    public Image healthBar;

    public GameObject iceEffect;


    void Start()
    {
        thisEnemy = GameMaster.enemyList[GameMaster.enemyList.Count - 1];
        rend = GetComponent<Renderer>();

        waypointIndex = thisEnemy.waypointstart;

        target = GameMaster.waypointList[thisEnemy.waypointstart]; //target is the next waypoint, some enemies dont start at 0

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
            case "Grey":
                rend.material.color = Color.grey;
                speed = 17;
                break;
            default:
                break;
        }
        speed *= ((GameMaster.GetTotalPathLength() / 80)); //makes the game fairer by increasing the speed of the enemies on longer paths
        nonIceSpeed = speed;
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
        Vector3 dir = target.getPosition() - transform.position; 
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World); //normalized converts it to a unit vector
        //i need a unit vector here as we only want the direction, not the magitude. Unit vector means vector with magnitude 1

        if (Vector3.Distance(transform.position, target.getPosition()) <= 0.5f) //if we are very close to the waypoint 
            //Doing this instead of transform.position = target.getposition() is more robust 
        {
            GetNextWaypoint();
        }

    }

    void Orange()
    {
        speed *= 1.025f;
    }
    void GetNextWaypoint()
    {
        if (waypointIndex >= GameMaster.waypointList.Count - 1) //if we are at the end
        {
            GameMaster.enemyList.Remove(thisEnemy);
            atEnd = true;
            GameMaster.playerHealth -= thisEnemy.GetDamage();
            Destroy(gameObject); //despawn
            return;
        }
        waypointIndex++;
        target = GameMaster.waypointList[waypointIndex];
    }

    public void editHealthBar(float health, float maxhealth)
    {
        healthBar.fillAmount = health / maxhealth;
        healthBar.color = new Color(1 - (health / maxhealth), health / maxhealth, 0);
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


