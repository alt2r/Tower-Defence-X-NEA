using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TurretMenu : MonoBehaviour
{
    public Text upgrade1Title, upgrade1Desc, upgrade2Title, upgrade2Desc, sellPriceText, upgrade1Cost, upgrade2Cost;
    Turret thisTurret;
    public GameObject upgr1Button, upgr2Button;
    private bool button1Destroyed = false, button2Destroyed = false;
    private int currentCost1, currentCost2;
    public void ClickOff()
    {
        GameMaster.shopPanel.SetActive(true);
        GameMaster.activeTurretMenu = false;
        Destroy(gameObject);
        return;
    }

    private void Update()
    {
        if (!button1Destroyed)
        {
            upgr1Button.GetComponent<Button>().interactable = true;
            if (GameMaster.playerBalance - currentCost1 < 0)
            {
                upgr1Button.GetComponent<Button>().interactable = false;
            }
        }
        if (!button2Destroyed)
        {
            upgr2Button.GetComponent<Button>().interactable = true;
            if (GameMaster.playerBalance - currentCost2 < 0)
            {
                upgr2Button.GetComponent<Button>().interactable = false;
            }
        }

    }

    public void setUp(Turret t)
    {
        upgrade1Title.text = t.GetUpgrade1Title();
        upgrade1Desc.text = t.GetUpgrade1Desc();
        upgrade2Title.text = t.GetUpgrade2Title();
        upgrade2Desc.text = t.GetUpgrade2Desc();
        sellPriceText.text = $"{Mathf.Floor(t.GetSellPrice())}g";
        thisTurret = t;

        switch (t.GetUpgradePath())
        {
            case 1:
                Destroy(upgr2Button);
                button2Destroyed = true;
                if (t.GetTurretMaxed())
                {
                    upgrade1Cost.text = "MAX";
                    currentCost1 = 9999999; //large number that will never be exceeded 
                }
                else
                {
                    upgrade1Cost.text = $"{t.GetUpgrade1PriceL2()}g";
                    currentCost1 = t.GetUpgrade1PriceL2();
                }
                break;
            case 2:
                Destroy(upgr1Button);
                button1Destroyed = true;
                if (t.GetTurretMaxed())
                {
                    upgrade2Cost.text = $"MAX";
                    currentCost2 = 9999999;
                }
                else
                {
                    upgrade2Cost.text = $"{t.GetUpgrade2PriceL2()}g";
                    currentCost2 = t.GetUpgrade2PriceL2();
                }
                break;
            default:
                upgrade1Cost.text = $"{t.GetUpgrade1PriceL1()}g";
                upgrade2Cost.text = $"{t.GetUpgrade2PriceL1()}g";
                currentCost1 = t.GetUpgrade1PriceL1();
                currentCost2 = t.GetUpgrade2PriceL1();
                break;
        }
    }

    public void Sell()
    {
        thisTurret.Sell();
        ClickOff();
    }
    
    public void Upgrade1()
    {
        GameMaster.playerBalance -= currentCost1;
        int newPrice = thisTurret.Upgrade1();
        if (newPrice == -1)
        {
            upgrade1Cost.text = $"MAX";
            currentCost1 = 999999999; //big number to ensure the plaeyrs balance never exceeds it
            thisTurret.SetTurretMaxed(true);
        }
        else
        {
            upgrade1Cost.text = $"{newPrice}g";
            currentCost1 = newPrice;
        }
        sellPriceText.text = $"{Mathf.Floor(thisTurret.GetSellPrice())}g";
        Destroy(upgr2Button);
        button2Destroyed = true;
    }

    public void Upgrade2()
    {
        GameMaster.playerBalance -= currentCost2;
        int newPrice = thisTurret.Upgrade2();
        if (newPrice == -1)
        {
            upgrade2Cost.text = $"MAX";
            currentCost2 = 999999999; //big number to ensure the plaeyrs balance never exceeds it
            thisTurret.SetTurretMaxed(true);
        }
        else
        {
            upgrade2Cost.text = $"{newPrice}g";
            currentCost2 = newPrice;
        }
        sellPriceText.text = $"{Mathf.Floor(thisTurret.GetSellPrice())}g";
        Destroy(upgr1Button);
        button1Destroyed = true;
    }

    public void TargetPriorityFirst()
    {
        thisTurret.setTargetMode("first");
    }
    public void TargetPriorityWeakest()
    {
        thisTurret.setTargetMode("weakest");
    }

    public void TargetPriorityLast()
    {
        thisTurret.setTargetMode("last");
    }

    public void TargetPriorityStrongest()
    {
        thisTurret.setTargetMode("strongest");
    }

}
