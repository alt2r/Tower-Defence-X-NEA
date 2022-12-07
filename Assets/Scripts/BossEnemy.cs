using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
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
    

    public GameObject minionGO;

    private bool atEnd = false;
    public bool IsAtEnd() { return atEnd; }

    public Image healthBar;

    public GameObject iceEffect;

    void Start()
    {
        thisEnemy = GameMaster.enemyList[GameMaster.enemyList.Count - 1];

        rend = GetComponent<Renderer>();

        target = GameMaster.waypointList[0]; //target is the next waypoint
        colour = thisEnemy.GetColour();
        switch (colour)
        {
            case "Black":
                rend.material.color = Color.black;
                speed = 9;
                break;
            case "White":
                rend.material.color = Color.white;
                speed = 11;
                InvokeRepeating("SpawnMinions", 1, 5); // this is always the same for white enemies so might as well set it here
                break;
            default:
                break;
        }
        speed *= (GameMaster.GetTotalPathLength() / 65); //makes the game fairer by increasing the speed of the enemies on longer paths
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
            effectIns.transform.localScale = new Vector3(2, 2, 2);
            Destroy(effectIns, 0.25f);
        }

        if (this.transform.position.y < -400) //if the enemy is in the dead zone(where enemies are moved when tthey die)
        {
            Destroy(gameObject);
        }
        Vector3 dir = target.getPosition() - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World); //normalized converts it to a unit vector
        //i need a unit vector here as we only want the direction, not the magitude. Unit vector means vector with magnitude 1

        if (Vector3.Distance(transform.position, target.getPosition()) <= 0.5f) //if we are very close to the waypoint 
        //Doing this instead of transform.position = target.getposition() is more robust and causes the enemies to not spazz out when stuff starts getting crazy
        {
            GetNextWaypoint();
        }
    }
    public void SetMinionRate(float minionSpawnTimer)
    {
        InvokeRepeating("SpawnMinions", 1, minionSpawnTimer);
    }

    private void SpawnMinions()
    {
        System.Random r = new System.Random();
        for (int i = 0; i < 2; i++)
        {
            Enemy x = new Enemy(minionGO, this.transform.position.x + ((float)r.NextDouble() * 2), this.transform.position.z + ((float)r.NextDouble() * 2), "Grey", waypointIndex);

            GameMaster.enemyList.Add(x);
        }
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= GameMaster.waypointList.Count - 1) //if we are at the end
        {
            GameMaster.enemyList.Remove(thisEnemy);
            atEnd = true;
            GameMaster.playerHealth -= thisEnemy.GetDamage();
            GameMaster.enemyList.Clear(); //softlock issues
            Destroy(gameObject); //despawn
            return;
        }
        waypointIndex++;
        target = GameMaster.waypointList[waypointIndex];
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
    public void editHealthBar(float health, float maxhealth)
    {
        healthBar.fillAmount = health / maxhealth;
        healthBar.color = new Color(1 - (health / maxhealth), health / maxhealth, 0);
    }
}
