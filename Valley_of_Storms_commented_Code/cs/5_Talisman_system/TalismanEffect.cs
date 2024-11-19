using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "TalismanEffect", menuName = "Talisman/Effects")]
public abstract class TalismanEffect : ScriptableObject //abstract class für Talismaneffekte
{
    public abstract void Effect(string applicationOrRemoval); //wird von Erweiterungen überschrieben
}

//1. When the player presses "E" for 0.5s, a Full Heal is done and Input is disabled (not really, the player simply enters a state where many controls are disabled), instead "I" now opens a Inventory, hitting "Q" will end CheckpointMode
//when Inventory is opened Player gets "Esc" in Player Control (The UI Actionmap is only for mouse-inputs on buttons..., to close the window again (so even another state) and the panel for Inventory is SetActive, So in State "Praying" there is a function that:
//contains something like FindWithTag("MenuCanvas").FindComponentsInChildren<CharmInventory>().gameObject.SetActive(); causing the CharmButtons to OnEnable where every CharmButton compares the "collected-List<>" with the own SO (depending on multiple factors, the charm will have
//a not collected image, be grayed out (not interactable) or have its image and be interactable (if slots are full a sound might play, we'll see), also the function Invokes a UnityEvent, that plays Reset() on all Stats and does the InitInventory in MyCharacterCtrl
