using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEnums;

[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Bear")]
public class BearEffect : TalismanEffect
{
    public override void Effect(string applicationOrRemoval)
    {
        Debug.Log("Bear's Vigour was applied!");
        if( applicationOrRemoval == EffectState.Application.ToString()) GameObject.FindWithTag("Player").GetComponent<HealthV2>().IncreaseMax(200); //Erhöhung Maximale Lebenspunkte
        if( applicationOrRemoval == EffectState.Removal.ToString()) GameObject.FindWithTag("Player").GetComponent<HealthV2>().DecreaseMax(200);
    }
}
