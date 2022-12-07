using UnityEngine;
using System;

public class tileScript : MonoBehaviour
{
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;
    private float turretRange = 0f;
    private float rangeShown = 0f;
    private GameObject rangeIDGO;
    private bool loaded = false;

    BuildManager bm;
    bool seeRangeBeforePlaced = false;

    private GameObject turretGO;

    public GameObject rangeIdentifer;

    private Turret thisTurret;

    GameObject thisTurretMenu;
    TurretMenu turretMenu;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        bm = BuildManager.instance;
        loaded = true;
    }
    private void Update()
    {
        if (seeRangeBeforePlaced && turretGO != null)
        {
            seeRangeBeforePlaced = false;
            GameObject.Destroy(rangeIDGO);
            rangeIDGO = null;
        }
        if (rangeIDGO == null || seeRangeBeforePlaced)
        {
            return;
        }
        if (!GameMaster.activeTurretMenu)
        {
            GameObject.Destroy(rangeIDGO);
            rangeIDGO = null;
        }
        turretRange = thisTurret.GetRange(); //refresh the range incase it has been changed through upgrades
        if (turretRange != rangeShown)
        {
            GameObject.Destroy(rangeIDGO);
            rangeIDGO = GameObject.Instantiate(rangeIdentifer, this.transform.position + new Vector3(0, 2f, 0), new Quaternion(0, 0, 0, 0));
            rangeIDGO.transform.localScale = new Vector3(turretRange * 2, 1, turretRange * 2);
            rangeShown = turretRange;
        }
        
    }

    void OnMouseDown()
    {
        if (GameMaster.activeTurretMenu)
        {
            return;
        }
        if (turretGO != null)
        { //show the turret menu
            //show the range
            rangeIDGO = GameObject.Instantiate(rangeIdentifer, this.transform.position + new Vector3(0, 2f, 0), new Quaternion(0, 0, 0, 0));
            rangeIDGO.transform.localScale = new Vector3(turretRange * 2, 1, turretRange * 2);
            rangeShown = turretRange;
            //shows a circle around the turret showing the range. range * 2 bc range is a radius and scale is a diameter

            //close the shop
            GameMaster.shopPanel.SetActive(false);

            GameObject menuTemplate = GameMaster.turretMenu;
            GameMaster.activeTurretMenu = true;
        //show a turret menu
        thisTurretMenu = GameObject.Instantiate(menuTemplate);
        turretMenu = thisTurretMenu.GetComponent<TurretMenu>();
        turretMenu.setUp(thisTurret); //add all the correct turret information to the turret menu
            return;
        }
        if (bm.GetTurretToBuild() == null)
        {
            return;
        }
        //build a tower
        GameObject turretToBuild = bm.GetTurretToBuild();
        turretGO = (GameObject)Instantiate(turretToBuild, transform.position, transform.rotation);
        switch (turretToBuild.name) //making sure each model is in the middle of the tile, on the ground
        {
            case ("Ballista"):
                turretGO.transform.position += new Vector3(0, 1, 0);
                break;
            case ("Fire Tower"):
                turretGO.transform.position += new Vector3(0, 0.5f, 0.2f);
                break;
            case ("Lightning Tower"):
                turretGO.transform.position += new Vector3(0f, 0.5f, 0);
                break;
            default:
                turretGO.transform.position += new Vector3(0, 0.52f, 0); //the other towers all need this same correction
                break;
        }
        turretGO.transform.rotation *= new Quaternion(0, 1, 0, 0);
        GameMaster.IncrementActiveTowers();

        thisTurret = turretGO.GetComponent<Turret>(); //gets the turret object of the script attatched to the gameobject
        GameMaster.playerBalance -= thisTurret.GetPrice();
        if (GameMaster.playerBalance < thisTurret.GetPrice() || !bm.GetMultiBuild())
        {
            bm.SetTurretToBuild(null);
        }
        rend.material.color = hoverColor;
    }


    void OnMouseEnter() //enable highlight
    {
        if (!loaded || GameMaster.activeTurretMenu)
        {
            return;
        }
        if (bm.GetTurretToBuild() == null || turretGO != null)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = Color.green;

            rangeIDGO = GameObject.Instantiate(rangeIdentifer, this.transform.position + new Vector3(0, 2f, 0), new Quaternion(0, 0, 0, 0));
            rangeIDGO.transform.localScale = new Vector3(bm.GetTurretToBuild().GetComponent<Turret>().GetRange() * 2, 1, bm.GetTurretToBuild().GetComponent<Turret>().GetRange() * 2);
            rangeShown = turretRange;
            seeRangeBeforePlaced = true;

            //Debug.Log(Grid.instance.GetTileAt(Convert.ToInt32(this.transform.position.x / 4.5f), Convert.ToInt32(this.transform.position.x / 4.5f)).GetDanger());
        }
    }
    void OnMouseExit() //disable highlight
    {
        rend.material.color = startColor;
        if (seeRangeBeforePlaced)
        {
            seeRangeBeforePlaced = false;
            GameObject.Destroy(rangeIDGO);
            rangeIDGO = null;
        }
    }
}
