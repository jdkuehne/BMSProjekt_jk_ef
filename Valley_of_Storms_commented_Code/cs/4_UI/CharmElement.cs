using UnityEngine.UIElements;
using UnityEngine;

//Diese Klasse konfiguriert den CharmElement Typ als VisualElement-Template, das Element teilt alle Eigenschaften mit Visual Element (Inheritance)
public class CharmElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<CharmElement> { } //erstellt den CharmElement Typ für UXML

    //Finde per default diese Elemente des Templates für Init
    private VisualElement Portraitimage => this.Q(className: "talismanmain");
    //private VisualElement Badge => this.Q("badge");
    private Label NoSlots => this.Q<Label>("slots");

    public void Init(Texture2D image, int slots, string talismanname) //Initialisierung des Elements mit Hintergrund, Sortiernummer und Name
    {
        Portraitimage.style.backgroundImage = image; 
        NoSlots.text = slots.ToString();
        Portraitimage.name = talismanname;
    }

    public CharmElement() { } //Constructor
}
