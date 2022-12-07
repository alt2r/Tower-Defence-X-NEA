using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBullet : Bullet
{
    // Start is called before the first frame update

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
}
