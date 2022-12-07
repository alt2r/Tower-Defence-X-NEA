using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using System;

public class Turret : MonoBehaviour
{
    protected float range;
    public float GetRange() { return range; } //putting getters and setters on one line makes it easier to recognise them
    protected float fireRate;
    public float GetFireRate() { return fireRate;  }
    protected float fireCountdown = 0f;
    protected string enemyTag = "Enemy";
    private GameObject bulletGO;
    protected bool targetAcquired = false;
    protected Transform target = null;
    private GameObject targetEnemy;
    protected float sellPrice;

    protected string Upgrade1Title, Upgrade1Desc, Upgrade2Title, Upgrade2Desc;
    public string GetUpgrade1Title() { return Upgrade1Title; }
    public string GetUpgrade1Desc() { return Upgrade1Desc; }
    public string GetUpgrade2Title() { return Upgrade2Title; }
    public string GetUpgrade2Desc() { return Upgrade2Desc; }
    protected string targetPriority = "first";

    protected int price;
    protected int upgrade1PriceL1, upgrade1PriceL2, upgrade2PriceL1, upgrade2PriceL2;
    public int GetUpgrade1PriceL1() { return upgrade1PriceL1;  }
    public int GetUpgrade1PriceL2() { return upgrade1PriceL2; }
    public int GetUpgrade2PriceL1() { return upgrade2PriceL1; }
    public int GetUpgrade2PriceL2() { return upgrade2PriceL2; }

    protected int UpgradePath = 0;
    public int GetUpgradePath() { return UpgradePath; }
    protected bool alreadyUpgraded = false;
    protected bool turretMaxed = false;
    public bool GetTurretMaxed() { return turretMaxed;  }
    public void SetTurretMaxed(bool x) { turretMaxed = x; }

    private List<Tile> tilesMarked = new List<Tile>();

    protected float damage;
    public Turret()
    {

    }

    protected void RefreshTilesMarked()
    {
        int len = tilesMarked.Count;
        for(int i = 0; i > len; i++) //empty tilesMarked
        {
            //could have used foreach here, but foreach loops crash the program if you modify the collection inside the loop, whereas for loops dont
            //now everything is reset.
            tilesMarked[0].DeIncrementDanger();
            tilesMarked.Remove(tilesMarked[0]);
        }
        foreach (Tile x in Grid.instance.gridArray)
        {
            if (Vector3.Distance(x.getPos(), transform.position) <= range + 3)
            {
                x.IncrementDanger();
                tilesMarked.Add(x);
            }
            //double mark the tiles if they are close to the tower. 
            if (Vector3.Distance(x.getPos(), transform.position) <= (range + 2) / 2)
            {
                x.IncrementDanger();
                tilesMarked.Add(x); //add them to the list a second time so that when the list is reset their danger level is deincremented twice
            }
        }

    }
    public int GetPrice()
    {
        return price;
    }
    public float GetSellPrice()
    {
        return sellPrice;
    }


    public void setTargetMode(string x)
    {
        targetPriority = x;
    }
    public virtual void Shoot(Transform firepoint, GameObject bulletToShoot, Transform target) //firepoint is a transform for the turrets that fire bullets that arent just spheres. I did it this way to make it easy to add actual bullet models later down the line
    {
        bulletGO = (GameObject)Instantiate(bulletToShoot, firepoint.position, firepoint.rotation);
        Bullet thisBullet = bulletGO.GetComponent<Bullet>();
        thisBullet.setDamage(damage);

        if (thisBullet != null)
        {
            thisBullet.SetTarget(target);
        }
    }
    public virtual int Upgrade1()
    {
        return 0;
    }

    public virtual int Upgrade2()
    {
        return 0;
    }
    public void Sell()
    {
        GameMaster.playerBalance += Convert.ToInt32(Mathf.Floor(sellPrice));
        GameMaster.DeIncrementActiveTowers();
        Destroy(gameObject);
    }

    void UpdateTarget()
    {
        if (target != null)
        {
            if (targetEnemy.name == "Enemy")
            {
                if (targetEnemy.GetComponent<StandardEnemy>().IsAtEnd())
                {
                    target = null;
                }
            }
            else if (targetEnemy.name == "flyingEnemy")
            {
                if (targetEnemy.GetComponent<FlyingEnemy>().IsAtEnd())
                {
                    target = null;
                }
            }
            else if(targetEnemy.name == "BossEnemy")
            {
                if (targetEnemy.GetComponent<BossEnemy>().IsAtEnd())
                {
                    target = null;
                }
            }
                           
        }
        if (target != null && (targetPriority == "first" || targetPriority == "last"))
        {
            return;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        List<GameObject> enemiesInRange = new List<GameObject>();
        float distanceToEnemy;
        foreach (GameObject enemy in enemies)
        {
            distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < range)
            {
                enemiesInRange.Add(enemy);
            }
        }
        if (enemiesInRange.Count == 0)
        {
            return;
        }
        switch (targetPriority)
        {
            case "first":
                float shortestDistance = Mathf.Infinity; //shortest distance is infinity as there are no enemys found at this stage
                GameObject nearestEnemy = null;

                foreach (GameObject enemy in enemiesInRange)
                {
                    distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distanceToEnemy < shortestDistance) //if this current enemy is closer than the closest enemy
                    {
                        shortestDistance = distanceToEnemy; 
                        nearestEnemy = enemy; //this enemy is now the closest enemy
                    }
                }
                if (nearestEnemy != null && shortestDistance <= range)
                {
                    target = nearestEnemy.transform; //closest enemy is now the target
                    targetEnemy = nearestEnemy;
                    targetAcquired = true; //we have a target
                }

                break;
            case "weakest":
                GameObject weakest = null;
                float healthOfWeakest = 99999; //smallest health of target is large number 
                foreach (GameObject x in enemiesInRange)
                {
                    if (x.name == "Enemy(Clone)") //if enemy is not flying
                    {
                        StandardEnemy scriptOfTarget = x.GetComponent<StandardEnemy>(); //this standard enemy
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return; //fixing a crash
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() < healthOfWeakest) //if this enemy has less health than the lowest so far
                        {
                            weakest = x; //this enemy is now the weakest
                            healthOfWeakest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }
                    else if(x.name == "flyingEnemy(Clone)") //if enemy is flying
                    {
                        FlyingEnemy scriptOfTarget = x.GetComponent<FlyingEnemy>(); //this flying enemy
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return;
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() < healthOfWeakest)//if this enemy has less health than the lowest so far
                        {
                            weakest = x; //this enemy is now the weakest
                            healthOfWeakest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }
                    else if (x.name == "BossEnemy(Clone)")
                    {
                        BossEnemy scriptOfTarget = x.GetComponent<BossEnemy>();
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return;
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() < healthOfWeakest)//if this enemy has less health than the lowest so far
                        {
                            weakest = x; //this enemy is now the weakest
                            healthOfWeakest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }

                }
                if (weakest != null)
                {
                    target = weakest.transform; //target is the weakest enemy
                    targetEnemy = weakest;
                    targetAcquired = true;
                }

                break;
            case "strongest":
                GameObject strongest = null;
                float healthOfStrongest = 0;
                foreach (GameObject x in enemiesInRange)
                {
                    if (x.name == "Enemy(Clone)")
                    {
                        StandardEnemy scriptOfTarget = x.GetComponent<StandardEnemy>();
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return; //was an error where we get here before thisEnemy was asigned 
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() > healthOfStrongest)
                        {
                            strongest = x;
                            healthOfStrongest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }
                    else if (x.name == "flyingEnemy(Clone)")
                    {
                        FlyingEnemy scriptOfTarget = x.GetComponent<FlyingEnemy>();
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return; //was an error where we get here before thisEnemy was asigned 
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() > healthOfStrongest)
                        {
                            strongest = x;
                            healthOfStrongest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }
                    else if (x.name == "BossEnemy(Clone)")
                    {
                        BossEnemy scriptOfTarget = x.GetComponent<BossEnemy>();
                        if (scriptOfTarget.thisEnemy == null)
                        {
                            return; //was an error where we get here before thisEnemy was asigned 
                        }
                        if (scriptOfTarget.thisEnemy.GetHealth() > healthOfStrongest)
                        {
                            strongest = x;
                            healthOfStrongest = scriptOfTarget.thisEnemy.GetHealth();
                        }
                    }
                }
                if (strongest != null)
                {
                    target = strongest.transform;
                    targetEnemy = strongest;
                    targetAcquired = true;
                }

                break;
            case "last":
                GameObject furthest = null;
                float distanceOfFurthest = 0;
                foreach (GameObject x in enemiesInRange)
                {
                    if (Vector3.Distance(transform.position, x.transform.position) > distanceOfFurthest)
                    {
                        furthest = x;
                        distanceOfFurthest = Vector3.Distance(transform.position, furthest.transform.position);
                    }
                }
                if (furthest != null)
                {
                    target = furthest.transform;
                    targetEnemy = furthest;
                    targetAcquired = true;
                }

                break;
            default:
                break;
        }
    }


}
