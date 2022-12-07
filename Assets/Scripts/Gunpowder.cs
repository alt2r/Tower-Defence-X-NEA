using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Gunpowder : Turret
{

    public Transform partToRotate;
    const float TURNSPEED = 6f;
    public GameObject gunpowderBulletGO;

    public Transform firepoint;
    private Vector3 endBlock;

    private float explosionRadius = 4;

    public Gunpowder()
    {
        range = 11.5f;
        fireRate = 1.15f;
        Upgrade1Title = "Heavy Artillery";
        Upgrade1Desc = "Increases explosion radius";
        Upgrade2Title = "Light Artillery";
        Upgrade2Desc = "Increases damage and fire rate";
        price = 300;
        sellPrice = price * 0.9f;
        upgrade1PriceL1 = 250;
        upgrade1PriceL2 = 400;
        upgrade2PriceL1 = 250;
        upgrade2PriceL2 = 400;
        damage = 15f;
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
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * TURNSPEED).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (fireCountdown <= 0f)
        {
            Shoot(firepoint, gunpowderBulletGO, target);
            fireCountdown = fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    public override int Upgrade1() //increase range
    {
        if (alreadyUpgraded)
        {
            explosionRadius = 6;
            sellPrice += Convert.ToInt32(upgrade1PriceL2 * 0.9f);
            return -1;
        }
        explosionRadius = 5;
        alreadyUpgraded = true;
        UpgradePath = 1;
        sellPrice += Convert.ToInt32(upgrade1PriceL1 * 0.9f);
        return upgrade1PriceL2;
    }

    public override void Shoot(Transform firepoint, GameObject bulletToShoot, Transform target)
    {
        GameObject bulletGO;
        bulletGO = (GameObject)Instantiate(bulletToShoot, firepoint.position, firepoint.rotation);
        GunpowderBullet thisBullet = bulletGO.GetComponent<GunpowderBullet>();
        thisBullet.setDamage(damage);
        thisBullet.SetExplosionRadiusAndDamage(explosionRadius, damage);

        if (thisBullet != null)
        {
            thisBullet.SetTarget(target);
        }
    }

    public override int Upgrade2() //increase fire rate
    {
        if (alreadyUpgraded)
        {
            fireRate = 1.05f;
            damage = 23f;
            sellPrice += Convert.ToInt32(upgrade2PriceL2 * 0.9f);
            return -1;
        }
        fireRate = 0.95f;
        damage = 19f;
        alreadyUpgraded = true;
        UpgradePath = 2;
        sellPrice += Convert.ToInt32(upgrade2PriceL1 * 0.9f);
        return upgrade2PriceL2;
    }
}

