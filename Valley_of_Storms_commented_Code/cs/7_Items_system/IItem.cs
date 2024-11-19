using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyCharacterCtrl;

//Interface für Popup Messages
public abstract class IItem : ScriptableObject
{
    public abstract Texture2D Image { get; set; }
    public abstract string Name { get; set; }

    public abstract int Number {  get; set; } 
    public abstract ItemType ItemType { get; set; }
}
