using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//UI Manager für das RuntimeUI => Lebensbalken, Popups, Heilflasche
public class RuntimeUIManager : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht Anhängen an GameObject
{
    public UIDocument runtimeUI;
    //Diese 3 Elemente nutzen 9-slicing um gewisse Teile des Bildes aus dem Skalieren auszuschliessen, daher kann die Grösse jeweils direkt auf die Hp gesetzt werden, ohne das Rahmen verzerrt wird
    private VisualElement healthFrame; //Rahmen des Lebensbalken
    private VisualElement healthBack; //Hintergrund des Lebensbalken
    private VisualElement healthFill; //Füllung des Lebensbalken
    
    private Label flaskLabel; //Label für Anzahl übrige Heilflaschen
    private int _currentMaxHp;
    private const int maxPossibleHp = 3700; //Maximum, bei mehr gerät der Rahmen aus dem Bild
    private const int frameMin = 110; //Anfangsmass des Rahmen => bei Max-Lebenspunkten von 5Hp ist der Rahmen so gross, weniger Raum ist nicht möglich, weil er mitgeslict wird
    private const int addToFrame = 105; //theoretische 0Hp für Rahmen und Hintergrund, für Berechnung der Pixelgrösse
    private const int addToFill = 95; //0Hp für Füllung

    [SerializeField] private ScriptableObject Test1;
    [SerializeField] private ScriptableObject Test2;
    [SerializeField] private ScriptableObject Test3;    

    [SerializeField] VisualTreeAsset pickupCardTemplate; //Template für Popup   

    IEnumerator Start()
    {        
        runtimeUI = GetComponent<UIDocument>();
        healthFrame = runtimeUI.rootVisualElement.Q("HealthFrame"); //Suche Elemente nach Name in UI
        healthBack = runtimeUI.rootVisualElement.Q("HealthBack");
        healthFill = runtimeUI.rootVisualElement.Q("HealthFill");
        flaskLabel = runtimeUI.rootVisualElement.Q<Label>("NumberHeal");
        
        yield return StartCoroutine(GOExtensions.WaitForComponentOnGOWithTag<MyCharacterCtrl>("Player")); //Warte auf Spieler, da momentan in selber Scene nicht notwendig
        MyCharacterCtrl.OnPickup += CardPopUp; //Subscription der Popup-Methode an das OnPickup-Event im Charaktercontroller        
    }


    public void CardPopUp(object sender, PickupEventArgs args) //Erstellt Popup
    {
        //Erstelle Element wie in TalismanUIManager beschrieben    
        UIManager.AddToElementOfType<VisualElement, PickupCard, IItem>(runtimeUI, pickupCardTemplate, "ItemsWindow", args.ItemInformation, (element, source) => //values for element/ source are inserted in the Method (UIManager)
        {
            element.ImageElement.style.backgroundImage = source.Image; //Setze Bild des Popups
            element.NumberLabel.text = source.Number.ToString(); //Nummer-Label auf Menge (bei Talisman immer 1)
            element.DescriptionLabel.text = source.Name; //Text-Label auf Objektname
            element.style.width = 1000; //Setzt Grösse des äusseren Elements (Template)
            element.style.height = 250;
            StartCoroutine(RemoveCard(source)); //Coroutine für Zerstören des Popups
        });
    }
    private IEnumerator RemoveCard(IItem selection) //Zerstört Popup nach 5s
    {
        yield return new WaitForSeconds(5f);
        var view = runtimeUI.rootVisualElement.Q("ItemsWindow");
        var card = view.Query<PickupCard>().Where(x => x.DescriptionLabel.text == selection.Name);
        view.Remove(card);
    }

    public void SetMaxHealth(int maxHealth) //ausgelöst von MaxHpChanged Event in HealthV2, setzt Breite von Hintergrund und Rahmen wenn Max-Lebenspunkte sich verändern
    {
        healthFrame.style.width = Mathf.Clamp(maxHealth + addToFrame, frameMin, maxPossibleHp + addToFrame); 
        healthBack.style.width = Mathf.Clamp(maxHealth + addToFrame, frameMin, maxPossibleHp + addToFrame);
        _currentMaxHp = maxHealth;
    }

    public void SetHp(int hp) //ausgelöst von Heal/Damaged Unity Event in HealthV2, verändert Breite der Füllung des Lebensbalken anhand jetzigen Hp
    {
        int val = hp + addToFill;
        healthFill.style.width = val;        
    }

    public void SetFlask(int flask) //Ausgelöst von OnFlaskUse-C# Event in Flask.cs, reagiert anders als es der Name vermuten lässt auch, wenn die Flaschen wieder aufgefüllt werden
    {
        flaskLabel.text = flask.ToString();
    }
}
