using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunpowderBullet : Bullet
{
    // Start is called before the first frame update
    private float radius, secondarydmg;
    public GameObject radiusID;
    private GameObject radiusIDGO;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = SPEED * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget(damage);
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void SetExplosionRadiusAndDamage(float r, float d)
    {
        radius = r;
        secondarydmg = d;
    }
    protected override void HitTarget(float dmg)
    {
        radiusIDGO = GameObject.Instantiate(radiusID,new Vector3(transform.position.x, 1, transform.position.z), new Quaternion(0, 0, 0, 0));
        radiusIDGO.transform.localScale = new Vector3(radius * 2, 1,radius * 2);
        Destroy(radiusIDGO, 0.1f);

        //finds the closest enemy to this bullet
        List<Enemy> enemies = GameMaster.enemyList;
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;
        float distanceToEnemy;
        foreach (Enemy x in enemies)
        {
            distanceToEnemy = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(x.GetPosition().x, 0, x.GetPosition().z));
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = x;
            }
        }

        nearestEnemy.TakeDamage(dmg);

        Enemy nearbyEnemy = null;
        for(int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(enemies[i].GetPosition().x, 0, enemies[i].GetPosition().z)) <= radius && enemies[i] != nearestEnemy)
            {
                nearbyEnemy = enemies[i];
                nearbyEnemy.TakeDamage(secondarydmg);
            }
        }

        Destroy(gameObject);
    }
}

