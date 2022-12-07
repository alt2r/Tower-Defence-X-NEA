using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fire : Turret
{
    public GameObject fireBulletGO;

    public Transform firepoint;
    private Vector3 endBlock;

    
    public Fire()
    {
        range = 9f;
        fireRate = 0.15f;
        Upgrade1Title = "Higher Temperature";
        Upgrade1Desc = "Increases damage";
        Upgrade2Title = "Shoot Further";
        Upgrade2Desc = "Increases Range";
        price = 500;
        sellPrice = price * 0.9f;
        upgrade1PriceL1 = 400;
        upgrade1PriceL2 = 600;
        upgrade2PriceL1 = 300;
        upgrade2PriceL2 = 450;
        damage = 7.5f;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.1f); //repeats the UpdateTarget() function every 0.1 seconds
        RefreshTilesMarked();
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > range || Vector3.Distance(target.transform.position, endBlock) <= 1f)
        {
            target = null; //if target is out of range stop shooting it
            targetAcquired = false;
        }
        if (target == null)
        {
            return;
        }

        if (fireCountdown <= 0f)
        {

            Shoot(firepoint, fireBulletGO, target);
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }
    public override int Upgrade1() //increase range
    {
        if (alreadyUpgraded)
        {
            damage = 10.5f;
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            return -1;
        }
        damage = 9f;
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        return upgrade1PriceL2;
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            range = 11.5f;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            RefreshTilesMarked();
            return -1;
        }
        range = 10.25f;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        RefreshTilesMarked();
        return upgrade2PriceL2;
    }
}
