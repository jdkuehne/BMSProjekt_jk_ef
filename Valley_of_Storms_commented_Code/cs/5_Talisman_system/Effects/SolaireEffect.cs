using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIEnums;


[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Solaire")]
public class SolaireEffect : TalismanEffect
{
    [SerializeField] private string _actionName;
    [SerializeField] private float _repeatedHealMultiplier;
    public override void Effect(string applicationOrRemoval)
    {
        HealthV2 playerHealth = GameObject.FindWithTag("Player").GetComponent<HealthV2>();        
        if (applicationOrRemoval == EffectState.Application.ToString())
        {
            //Füge dem Healthobjekt des Spielers in der Coroutine, die jede Sekunde abläuft, einen geringen Heileffekt hinzu (Heilung über Zeit)
            playerHealth.TalismanEverySecond.Add(new NamedAction<int>(_actionName, (x) =>
            {
                playerHealth.Heal((int)((float)x * _repeatedHealMultiplier));
            }));
        }
        if (applicationOrRemoval == EffectState.Removal.ToString())
        {
            playerHealth.TalismanEverySecond.RemoveAll(x => x.Name == _actionName);
        }
    }
}
