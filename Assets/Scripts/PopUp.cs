using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public Text title;
    public Text desc;

    private GameObject thisPopUp;

    public void Setup(string titletext, string desctext, GameObject thisGO)
    {
        title.text = titletext;
        desc.text = desctext;
        thisPopUp = thisGO;
    }

    public void Close()
    {
        Destroy(thisPopUp);
    }
}
