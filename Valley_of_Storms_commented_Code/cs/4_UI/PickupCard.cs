using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

//Template für Popus bei Aufsammeln von Items
public class PickupCard : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PickupCard> { }
    public VisualElement ImageElement => this.Q("Image");
    public Label NumberLabel => this.Q<Label>("Number");
    public Label DescriptionLabel => this.Q<Label>("Description");
    public PickupCard() { }
}
