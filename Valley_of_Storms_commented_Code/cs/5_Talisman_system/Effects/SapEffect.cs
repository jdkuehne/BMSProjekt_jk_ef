using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEnums;

[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Sap")]
public class SapEffect : TalismanEffect
{
    public override void Effect(string applicationOrRemoval)
    {
        if (applicationOrRemoval == EffectState.Application.ToString()) GameObject.FindWithTag("Player").GetComponent<Flask>().IncreaseMax(1); //Erhöhe Heilflaschen-Anzahl um 1
        if (applicationOrRemoval == EffectState.Removal.ToString()) GameObject.FindWithTag("Player").GetComponent<Flask>().DecreaseMax(1);
    }

}
