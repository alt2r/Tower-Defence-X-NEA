using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Lightning : Turret
{
    public GameObject LightningBulletGO;

    public Transform firepoint;
    private Vector3 endBlock;

    public GameObject shootEffect;

    public Lightning()
    {
        range = 18f;
        fireRate = 1.8f;
        Upgrade1Title = "Continental range";
        Upgrade1Desc = "Increases range dramatically but reduces damage";
        Upgrade2Title = "Smite";
        Upgrade2Desc = "Increases damage per shot";
        price = 1000;
        sellPrice = price * 0.9f;
        upgrade1PriceL1 = 900;
        upgrade1PriceL2 = 1200;
        upgrade2PriceL1 = 900;
        upgrade2PriceL2 = 1000;
        damage = 125;
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.1f); //repeats the UpdateTarget() function every 0.1 seconds
        transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y - 2, transform.position.z); //they were floating. need to lower them
        RefreshTilesMarked();
    }

    public override void Shoot(Transform firepoint, GameObject bulletToShoot, Transform target)
    {
        GameObject effectIns = (GameObject)Instantiate(shootEffect, transform.position + new Vector3(0f, 6f, 0f), transform.rotation);
        Destroy(effectIns, 2f);
        GameObject bulletGO;
        bulletGO = (GameObject)Instantiate(bulletToShoot, firepoint.position, firepoint.rotation);
        Bullet thisBullet = bulletGO.GetComponent<Bullet>();
        thisBullet.setDamage(damage);

        if (thisBullet != null)
        {
            thisBullet.SetTarget(target);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            fireCountdown -= Time.deltaTime;
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

        if (fireCountdown <= 0f)
        {
            Shoot(firepoint, LightningBulletGO, target);
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    public override int Upgrade1() //increase range
    {
        if (alreadyUpgraded)
        {
            range = 26f;
            damage = 115;
            RefreshTilesMarked();
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            return -1;
        }
        range = 22f;
        damage = 120;
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        RefreshTilesMarked();
        return upgrade1PriceL2;
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            damage = 165;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            return -1;
        }
        damage = 140f;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        return upgrade2PriceL2;
    }
}

