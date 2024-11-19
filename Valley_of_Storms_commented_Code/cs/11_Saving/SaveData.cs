using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In diesem Datentyp werden zu speichernde Daten eingetragen, alle Daten werden ohne Referenz, meist als struct und sonst über Umwege gespeichert (Daten sind immutable)
//Das kann auch anders gelöst werden, man muss aber vorsichtig sein, dass Einträge sich dann nicht doppeln usw. (ist fehleranfälliger wenn mit Referenz zu Objekten)
[Serializable]
public struct SaveData
{
    public bool firstSessionOnThisSave;
    public string StartSceneName;
    public (float x, float y, float z) StartShrinePosition;
    public List<string> pickedUpItemNames;
    public List<string> killedBigEnemyNames;
    public List<string> killedBossEnemyNames;
    public List<MenuThread> TalismanSlotData;
    public List<string> TalismanNames;
    public List<string> CarriedTalismanNames;
    public Settings savedSettings; //this data type is easily serializable
}
