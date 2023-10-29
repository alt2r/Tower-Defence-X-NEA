using UnityEngine;
using System.Collections.Generic;
public class Bullet : MonoBehaviour
{
    protected Transform target = null;
    protected const float SPEED = 150f;
    protected Vector3 endBlock = new Vector3(37, -60, 55);
    protected string enemyTag = "Enemy";
    protected float damage;
    public void SetTarget(Transform t)
    {
        target = t;

    }

    public void setDamage(float x)
    {
        damage = x;
    }

    protected virtual void HitTarget(float dmg)
    {
        //finds the closest enemy to this bullet
        List<Enemy> enemies = GameMaster.enemyList;
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;
        float distanceToEnemy;
        foreach (Enemy enemy in enemies)
        {
            //this function is called when an enemy is hit
            //loops through all enemies to find the closest one to where the bullet landed.
            distanceToEnemy = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        try //there was an issue when two bullets hit an enemy at the same time and the first bullet to run this code killed the enemy, meaning nearest enemy became null. this caused the game to crash. this stops this from happening
        {
            nearestEnemy.TakeDamage(dmg);
        }
        catch (System.Exception)
        {
            
        }


        Destroy(gameObject);
    }
}
