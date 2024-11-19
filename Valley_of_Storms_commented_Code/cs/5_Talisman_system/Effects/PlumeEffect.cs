using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEnums;

[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Plume")]
public class PlumeEffect : TalismanEffect
{
    [SerializeField] private string _modifierName;
    [SerializeField] private float _rtsrAttackMod; //Schadensfaktor, Effekt könnte auch mit verschiedenen Upgrade Stufen erstellt werden, da ScriptableObject
    public override void Effect(string applicationOrRemoval)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (applicationOrRemoval == EffectState.Application.ToString()) 
        { 
            //Füge der Berechnungsliste eine Funktionsvariable an
            player.GetComponent<Damage>().Modifiers.Add(new NamedFunc<float, float>(_modifierName, (x) =>
            {
                if (player.GetComponent<HealthV2>().Hp < (int)(0.3 * (float)player.GetComponent<HealthV2>().MaxHp)) //Wenn Hp unter 30%
                {
                    return x * _rtsrAttackMod; //Gebe den Schadenswert multipliziert mit Boostfaktor zurück
                }
                else
                {
                    return x; //Sonst unverändert
                }
            })); 
        }
        if (applicationOrRemoval == EffectState.Removal.ToString())
        {
            player.GetComponent<Damage>().Modifiers.RemoveAll(x => x.Name == _modifierName); //Finde und entferne Berechnung aus Liste
        }     
    }
}