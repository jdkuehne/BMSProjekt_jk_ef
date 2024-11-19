using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EventArgs Klasse für Item-Pickup Handling, wird bei auslösen des Event mitgegeben
public class PickupEventArgs
{
    public ScriptableObject PickupItem { get; set; }
    public IItem ItemInformation { get; set; }
    public PickupEventArgs(ScriptableObject pickupItem, IItem popUp)
    {
        PickupItem = pickupItem;
        ItemInformation = popUp;
    }
}
