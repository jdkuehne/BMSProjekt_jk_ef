using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Einfache Klasse für das Ablegen von Talisman-Slot-Informationen
[System.Serializable]
public class MenuThread
{
    public string slotName;
    //public TalismanEffect charmEffect;
    //public Texture2D image;
    public string talismanName;

    public MenuThread(string slotName, string talismanName)
    {
        this.slotName = slotName;
        this.talismanName = talismanName;
    }
}
