using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Ice : Turret
{

    public GameObject shootEffect;

    private float duration = 7;
    private float speedReduction = 0.7f;

    public Ice()
    {
        range = 8f;
        fireRate = 1f;
        Upgrade1Title = "Lingering Cold";
        Upgrade1Desc = "Increases duration of the slowness debuff";
        Upgrade2Title = "Sheer Cold";
        Upgrade2Desc = "Further reduces enemy speed";
        price = 200;
        sellPrice = 180;
        upgrade1PriceL1 = 300;
        upgrade1PriceL2 = 400;
        upgrade2PriceL1 = 300;
        upgrade2PriceL2 = 400;
       
    }
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z); 
        RefreshTilesMarked();
    }

    public void Shoot(List<GameObject> enemiesToHit)
    {
        GameObject effectIns = (GameObject)Instantiate(shootEffect, transform.position + new Vector3(0f, 6f, 0f), transform.rotation);
        Destroy(effectIns, 2f);
        foreach (GameObject x in enemiesToHit)
        {
            if (x.name == "Enemy(Clone)")
            {
                x.GetComponent<StandardEnemy>().IceDebuff(speedReduction, duration);
            }
            else if(x.name == "flyingEnemy(Clone)")
            {
                x.GetComponent<FlyingEnemy>().IceDebuff(speedReduction, duration);
            }
            else
            {
                x.GetComponent<BossEnemy>().IceDebuff(speedReduction, duration);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
        if (enemiesInRange.Count <= 0)
        {
            fireCountdown -= Time.deltaTime;
            return;
        }
        if (fireCountdown <= 0f)
        {
            Shoot(enemiesInRange);
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    public override int Upgrade1() //increase range
    {
        if (alreadyUpgraded)
        {
            duration = 11;
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            return -1;
        }
        duration = 9;
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        return upgrade1PriceL2;
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            speedReduction = 0.55f;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            return -1;
        }
        speedReduction = 0.625f;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        return upgrade2PriceL2;
    }
}

