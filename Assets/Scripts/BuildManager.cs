using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager
{
    public GameObject ballistaTurret;
    public GameObject gunpowderTurret;
    public GameObject iceTurret;
    public GameObject fireTurret;
    public GameObject lightningTurret;
    public GameObject powerTurret;

    public static BuildManager instance;
    private GameObject turretToBuild;

    private bool multibuild = false;
    public BuildManager(GameObject ballistaGO, GameObject gunpowderGO, GameObject iceGO, GameObject fireGO, GameObject lightningGO, GameObject powerGO)
    {
        
        if (instance != null)
        {
            Debug.LogError("More than one instance");
            return;
        }
        instance = this;
        ballistaTurret = ballistaGO;
        gunpowderTurret = gunpowderGO;
        iceTurret = iceGO;
        fireTurret = fireGO;
        lightningTurret = lightningGO;
        powerTurret = powerGO;

    }
    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }
    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
        
    }

    public void DestroyBM()
    {
        instance = null;
    }

    public void ToggleMultiBuild()
    {
        multibuild = !multibuild;
    }

    public bool GetMultiBuild()
    {
        return multibuild;
    }
}