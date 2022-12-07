using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{

    BuildManager bm;

    public Button ballistaButton;
    public Button gunpowderButton;
    public Button iceButton;
    public Button fireButton;
    public Button lightningButton;
    public Button powerButton;

    public GameObject UnlocktextB;
    public GameObject UnlocktextG;
    public GameObject UnlocktextI;
    public GameObject UnlocktextF;
    public GameObject UnlocktextL;
    public GameObject UnlocktextP;

    public Text selectedTitle, selectedDesc;
    public GameObject textBG;

    List<Button> shopButtons = new List<Button>();
    List<GameObject> unlockTexts = new List<GameObject>();

    int[] prices = { 100, 300, 200, 500, 1000, 1500}; //ballista = 100, gunpowder = 300, ice = 200, fire = 500, lightning = 1000, power = 1500
    int[] unlockWaves = {0, 2, 4, 7, 11, 15};
    //int[] unlockWaves = { 0, 0, 0, 0, 0, 0 };
    private void Start()
    {
        bm = BuildManager.instance;

        shopButtons.Add(ballistaButton);
        shopButtons.Add(gunpowderButton);
        shopButtons.Add(iceButton);
        shopButtons.Add(fireButton);
        shopButtons.Add(lightningButton);
        shopButtons.Add(powerButton);

        unlockTexts.Add(UnlocktextB);
        unlockTexts.Add(UnlocktextG);
        unlockTexts.Add(UnlocktextI);
        unlockTexts.Add(UnlocktextF);
        unlockTexts.Add(UnlocktextL);
        unlockTexts.Add(UnlocktextP);

        textBG.SetActive(false);
    }
    private void Update()
    {
        for (int i = 0; i < shopButtons.Count; i++)
        {
            shopButtons[i].interactable = true;
            unlockTexts[i].SetActive(false);
            if (GameMaster.playerBalance < prices[i] || GameMaster.GetWaveNumber() < unlockWaves[i])
            {
                shopButtons[i].interactable = false;
                if (GameMaster.GetWaveNumber() < unlockWaves[i])
                {
                    unlockTexts[i].SetActive(true);
                }
                
            }
        }
        if (bm.GetTurretToBuild() == null)
        {
            selectedTitle.text = "";
            selectedDesc.text = "";
            textBG.SetActive(false);
            return;
        }
        textBG.SetActive(true);
        switch (bm.GetTurretToBuild().name)
        {
            case "Ballista":
                selectedTitle.text = "Ballista";
                selectedDesc.text = "A slow firing turret that fires high velocity arrows at its target";
                break;
            case "Ice Tower":
                selectedTitle.text = "Ice";
                selectedDesc.text = "A tower that deals 0 damage, but slows down all enemies within its range for a short time";
                break;
            case "Gunpowder Turret":
                selectedTitle.text = "Gunpowder";
                selectedDesc.text = "A tower that fires explosive rounds, capable of dealing damage to multiple targets";
                break;
            case "Fire Tower":
                selectedTitle.text = "Fire";
                selectedDesc.text = "A rapid fire turret that shoots fire at targets. deals high damage in a short range";
                break;
            case "Lightning Tower":
                selectedTitle.text = "Lightning";
                selectedDesc.text = "A slow firing turret that deals huge damage to single targets";
                break;
            case "Power Tower":
                selectedTitle.text = "Power";
                selectedDesc.text = "A turret that increases its damage per shot each time it hits an enemy. Resets each round";
                break;
            default:
                break;
        }

    }

    public void PurchaseBallistaTurret()
    {
        bm.SetTurretToBuild(bm.ballistaTurret);
    }
    public void PurchaseGunpowderTurret()
    {
       bm.SetTurretToBuild(bm.gunpowderTurret);
    }

    public void PurchaseIceTurret()
    {
        bm.SetTurretToBuild(bm.iceTurret);
    }

    public void PurchaseFireTurret()
    {
        bm.SetTurretToBuild(bm.fireTurret);
    }

    public void PurchaseLightningTurret()
    {
        bm.SetTurretToBuild(bm.lightningTurret);
    }
    public void PurchasePowerTurret()
    {
        bm.SetTurretToBuild(bm.powerTurret);
    }
    public void CancelBuy()
    {
        bm.SetTurretToBuild(null);
    }
    public void ToggleMultiBuild()
    {
        bm.ToggleMultiBuild();
    }

}
