using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Ballista : Turret
{
   
    public Transform partToRotate;
    private float turnspeed = 10f;
    public GameObject ballistaBulletGO;

    public Transform firepoint;
    private Vector3 endBlock;

    public Ballista()
    {
        range = 15;
        fireRate = 0.5f;
        Upgrade1Title = "Shoot further";
        Upgrade1Desc = "Increases range";
        Upgrade2Title = "Shoot faster";
        Upgrade2Desc = "Increases fire rate";
        price = 100;
        sellPrice = price * 0.9f;
        upgrade1PriceL1 = 100;
        upgrade1PriceL2 = 200;
        upgrade2PriceL1 = 100;
        upgrade2PriceL2 = 200;
        damage = 10.5f;
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
        //target lock on 
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnspeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        firepoint = partToRotate.transform; //firepoint is the point at which the bullet starts its movement

        if (fireCountdown <= 0f)
        {
            Shoot(firepoint, ballistaBulletGO, target);
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    public override int Upgrade1() //increase range
    {
        if (alreadyUpgraded)
        {
            range = 20f;
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            RefreshTilesMarked();
            return -1;
        }
        range = 17.5f;
        RefreshTilesMarked();
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        return upgrade1PriceL2;
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            fireRate = 0.33f;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            return -1;
        }
        fireRate = 0.42f;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        return upgrade2PriceL2;
    }
}
