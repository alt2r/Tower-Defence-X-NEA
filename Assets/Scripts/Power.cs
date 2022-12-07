using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Power : Turret
{
    public GameObject fireBulletGO;

    public Transform firepoint;
    private Vector3 endBlock;

    private float damageGain = 1.04f;

    private int currentWave = -1;

    public Power()
    {
        range = 13.5f;
        fireRate = 1f;
        Upgrade1Title = "Increased Gains";
        Upgrade1Desc = "Increases damage gained from each hit";
        Upgrade2Title = "Power Up";
        Upgrade2Desc = "Increases Range and damage";
        price = 1000;
        sellPrice = 900;
        upgrade1PriceL1 = 750;
        upgrade1PriceL2 = 1000;
        upgrade2PriceL1 = 600;
        upgrade2PriceL2 = 900;
        damage = 28;
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
        if (currentWave < GameMaster.GetWaveNumber())
        {
            currentWave = GameMaster.GetWaveNumber();
            damage = 28;
        }
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
            damage *= damageGain;
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }
    public override int Upgrade1()
    {
        if (alreadyUpgraded)
        {
            damageGain = 1.07f;
            damage = 28;
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            return -1;
        }
        damageGain = 1.055f;
        damage = 28;
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        return upgrade1PriceL2;
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            range = 17.5f;
            damage = 28;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            RefreshTilesMarked();
            return -1;
        }
        range = 15;
        damage = 28;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        RefreshTilesMarked();
        return upgrade2PriceL2;
    }
}
