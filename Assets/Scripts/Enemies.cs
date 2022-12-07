using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy
{
    GameObject thisEnemyGO;
    float health, maxHealth;
    float damage = 4;
    int moneyDropped = 20;
    string colour;
    bool flying;
    bool boss;

    public bool dead = false;

    public int waypointstart = 0;
    public Enemy(GameObject thisEnemyGO, float xpos, float zpos, string Colour, int? waypoint) //nullable int bc boss minions spawn half way through the waypoints
    {
        this.thisEnemyGO = (GameObject)GameObject.Instantiate(thisEnemyGO, new Vector3(xpos, 0f, zpos), new Quaternion(0, 0, 0, 0));
        if (waypoint != null)
        {
            waypointstart = (int)waypoint;
        }

        if (thisEnemyGO.name == "flyingEnemy")
        {
            flying = true;
        }
        else
        {
            flying = false;
        }
        colour = Colour;
        boss = false;
        switch (colour)
        {
            case "Blue":
                health = 40;
                moneyDropped = 20;
                break;
            case "Yellow":
                health = 80;
                moneyDropped = 30;
                break;
            case "Green":
                health = 130;
                moneyDropped = 35;
                break;
            case "Red":
                health = 250;
                moneyDropped = 35;
                break;
            case "Pink":
                health = 400;
                moneyDropped = 50;
                break;
            case "Orange":
                health = 150;
                moneyDropped = 40;
                break;
            case "White":
                health = 700;
                moneyDropped = 400;
                boss = true;
                break;
            case "Black":
                health = 2000;
                moneyDropped = 500;
                boss = true;
                break;
            case "Grey":
                health = 50;
                moneyDropped = 10;
                break;
            default:
                break;
        }

        //adjusting the health and damage based on circumstances
        if (!boss)
        {
            switch (MainMenu.GetDifficulty())
            {
                case 0: //easy mode
                    health *= 0.8f;
                    damage = 3;
                    break;
                case 2: //hard mode
                    health *= 1.2f;
                    damage = 5;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (MainMenu.GetDifficulty())
            {
                case 0: //easy mode
                    damage = 12;
                    break;
                case 1: //normal mode
                    damage = 16;
                    break;
                case 2: //hard mode
                    damage = 20;
                    break;
                default:
                    break;
            }
        }

        health *= (1 + ((GameMaster.GetWaveNumber()) * 0.05f));
        //health *= 2; //d
        maxHealth = health;
        //System.Random r = new System.Random(); //d
        //float n = r.Next(3, 10) * 0.1f; //d
        //health *= n; //d
        thisEnemyGO.GetComponent<StandardEnemy>().editHealthBar(health, maxHealth);
    }

    public void Pink()
    {
        if (health + 30 <= maxHealth)
        {
            health += 30;
            thisEnemyGO.GetComponent<StandardEnemy>().editHealthBar(health, maxHealth);
            //pink enemies are now always path following 
        }
        
    }

    public float GetHealth()
    {
        return health;
    }
    public void SetBossStats(float hp, float minionSpawnRate) //boss enemies will have different health values on boss waves
    {
        if (!boss) //this is only for boss enemies
        {
            return;
        }
        health = hp;
        thisEnemyGO.GetComponent<BossEnemy>().SetMinionRate(minionSpawnRate);
        switch (MainMenu.GetDifficulty())
        {
            case 0: //easy mode
                health *= 0.8f;
                break;
            case 2: //hard mode
                health *= 1.2f;
                break;
            default:
                break;               
        }
        maxHealth = health;
    }
    public Vector3 GetPosition()
    {
        if (thisEnemyGO == null)
        {
            return new Vector3(0, 0, 0);
        }
        else
        {
            return thisEnemyGO.transform.position;
        }
    }
    public string GetColour()
    {
        return colour;
    }
    public float GetDamage()
    {
        return damage;
    }
    public GameObject GetScript()
    {
        return thisEnemyGO;
    }
    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if (flying)
        {
            thisEnemyGO.GetComponent<FlyingEnemy>().editHealthBar(health, maxHealth);
        }
        else if(boss)
        {
            thisEnemyGO.GetComponent<BossEnemy>().editHealthBar(health, maxHealth);
        }
        else
        {
            thisEnemyGO.GetComponent<StandardEnemy>().editHealthBar(health, maxHealth);
        }
        
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        thisEnemyGO.transform.position = new Vector3(0, -500f, 0); //DEAD ZONE
        //disables the enemy. Enemy scrips will check whether the enemy has been disabled each frame, and will destroy the gameobject if ti has been
        dead = true;
        //cannot destroy a gameobejct without inheriting from Monobehaviour, but i can set its position adn then get it to delete itself if it reaches that position
        GameMaster.playerBalance += moneyDropped;
        GameMaster.enemyList.Remove(this);
        GameMaster.enemiesKilled++;
    }

}