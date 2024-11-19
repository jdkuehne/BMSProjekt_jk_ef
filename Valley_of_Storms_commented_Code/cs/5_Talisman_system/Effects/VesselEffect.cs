using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEnums;

[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Vessel")]
public class VesselEffect : TalismanEffect
{
    [SerializeField] private string _actionName;
    [SerializeField] private float _damageMultiplierForHeal;
    public override void Effect(string applicationOrRemoval)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject sword = GameObject.FindWithTag("Player-Weapon");
        if (applicationOrRemoval == EffectState.Application.ToString())
        {
            //Heilung bei Treffer wird hinzugefügt
            sword.GetComponent<Sword>().TalismanAttackEffects.Add(new NamedAction<int>(_actionName, (x) =>
            {
                player.GetComponent<HealthV2>().Heal((int)((float)x * _damageMultiplierForHeal));
            }));
        }
        if (applicationOrRemoval == EffectState.Removal.ToString())
        {
            sword.GetComponent<Sword>().TalismanAttackEffects.RemoveAll(x => x.Name == _actionName);
        }
    }
}
