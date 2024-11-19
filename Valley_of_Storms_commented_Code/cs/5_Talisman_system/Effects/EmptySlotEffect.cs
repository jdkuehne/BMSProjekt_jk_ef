using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects/Empty")]
public class EmptySlotEffect : TalismanEffect
{
    public override void Effect(string applicationOrRemoval)
    {
        Debug.Log("Empty Slot!");
    }
}
