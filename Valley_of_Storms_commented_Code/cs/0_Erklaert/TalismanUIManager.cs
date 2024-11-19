using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UIEnums;

//Klasse, die Erstellung und Interaktion des Talisman-Selektor-Fensters implementiert
public class TalismanUIManager : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht Anhängen an GameObject
{
    [SerializeField] VisualTreeAsset charmTemplate; //Template Uxml File    
    [SerializeField] StyleSheet charmStyle; //Stylesheet für Talisman Elemente in UI
    
    [SerializeField] private TalismanHandler talismanHandler;
    [SerializeField] private CollectionService myCollections; //stellt eine Liste aller Talismane zur Verfügung (Talismane können so per Name gesucht werden mit List.Find(Predicate<T>);)
    
    private UIDocument talismanDoc; //Das UI Document mit dem Talisman Selektor
    [SerializeField] private InventoryUIManager inventoryUI; //Manager Script für Inventar UI
    
    IEnumerator Start()
    {
        talismanDoc = GetComponent<UIDocument>(); 
        myCollections = GameObject.FindWithTag("SaveHandler").GetComponent<CollectionService>();

        yield return new WaitUntil(() => talismanHandler.Sorted); //Wartet bis Talismane in TalismanHandler aus dem Speicher gelesen und sortiert sind       
        
        foreach (var talisman in talismanHandler.Talismans) //Für jeden Talisman in der Liste wird folgendes ausgeführt:
        {
            var templateContainer = charmTemplate.Instantiate(); //Talisman Template Container wird erstellt, der Container ist ein temporärer "Behälter" für die Daten des UXML
            var charmElement = templateContainer.Q<CharmElement>(); //Container wird nach Element des Typs: CharmElement durchsucht, dieser ist eine C# Klasse die im UXML referenziert wird
            
            var view = talismanDoc.rootVisualElement.Q<ScrollView>("Charms"); //Die Scrollliste (wo die Talismane hinkommen) wird einer Variable zugewiesen
            view.Add(charmElement); //Das Element aus dem Container wird der Liste hinzugefügt       
            
            charmElement.Init(talisman.Image, talisman.SortNumber, talisman.TalismanName); //Initialisierungcode aus der CharmElement Klasse wird ausgeführt, wodurch Name, Nummer und Bild des Elements gesetzt werden
            //Debug.Log(charmElement.Q(className: "talismanmain"));
            charmElement.styleSheets.Add(charmStyle); //Fügt style (Dimensionen, Farbe/Rand/Radius des Nummern-Elements) hinzu, ausserdem ermöglicht mit hover: abdunkeln wenn mit Maus darübergefahren wird
            charmElement.style.alignSelf = Align.Center; //zentriert das Element, muss direkt am äussersten Element gemacht werden, was wir im USS nicht hingekriegt haben
            charmElement.Q(className: "talismanmain").RegisterCallback<MouseDownEvent>(CharmInteraction); //Element als Button konfigurieren
        }
        talismanDoc.rootVisualElement.style.display = DisplayStyle.None; //UI vorerst ausblenden
    }
    
    public void NewTalismanAdded() //Diese Methode führt den Prozess der CharmElement-Instanzierung ähnlich Start() aus, liest aber dabei Daten des aufgenommenen Objektes (ausgelöst bei Item-Pickup)
    {
        var view = talismanDoc.rootVisualElement.Q<ScrollView>("Charms");
        view.Clear(); //leeren
        foreach(var talisman in talismanHandler.Talismans) //wieder neu auffüllen mit neuer Reihenfolge
        {
            UIManager.AddToElementOfType<ScrollView, CharmElement, IItem>(talismanDoc, charmTemplate, "Charms", talisman as IItem, (element, source) =>  //Methode zum hinzufügen von Template zu Element/Scrollliste
            { //delegat => kann bei jeder Ausführung anders sein
                element.Init(talisman.Image, talisman.SortNumber, talisman.TalismanName);
                element.styleSheets.Add(charmStyle);
                element.style.alignSelf = Align.Center;
                element.Q(className: "talismanmain").RegisterCallback<MouseDownEvent>(CharmInteraction);
            });
        }
    }
    
    private void CharmInteraction(MouseDownEvent evt) //ausgelöst bei Interaktion mit Talisman
    {
        if (evt.button == 0) //wenn Linksklick:
        { 
            Debug.Log((evt.target as VisualElement).name.ToString());            
            Talisman clickedTalisman = myCollections.AllTalismans.Find(x => x.TalismanName == (evt.target as VisualElement).Q(className: "talismanmain").name); //Finde Element mit Name des UI Elements
            if (talismanHandler.CarriedTalismanNames.Exists(x => x == clickedTalisman.TalismanName)) //Wenn der Talisman schon in einem andern Slot ist:
            {
                MenuThread slotWithCarriedTalisman = inventoryUI.TalismanSlots.Find(x => x.talismanName == clickedTalisman.TalismanName); //Slot finden                
                Talisman talismanOldSlot =  myCollections.AllTalismans.Find(x => x.TalismanName == slotWithCarriedTalisman.talismanName); //Finde Talisman (redundant, clickedTalisman sollte dasselbe sein)
                talismanOldSlot.effect.Effect(EffectState.Removal.ToString()); //mache Effekt des Talismans im alten Slot rückgängig damit der Effekt nicht gedoppelt wird
                inventoryUI.inventory.rootVisualElement.Q(slotWithCarriedTalisman.slotName).style.backgroundImage = null; //Reset von Slot auf leeres Bild
                slotWithCarriedTalisman.talismanName = string.Empty; //Reset des Talisman-Namen im MenuThread des alten Slots               
            }
            else
            {
                talismanHandler.CarriedTalismanNames.Add(clickedTalisman.TalismanName); //wenn nicht wird der Talisman der Liste der getragenen Talismane schon hinzugefügt
            }            
            MenuThread currentSlot = inventoryUI.TalismanSlots.Find(x => x.slotName == inventoryUI.currentThread.slotName); //currentThread ist eine Kopie des ausgewählten Slots im InventoryUIManager
            //dies erlaubt uns den richtigen Slot zu finden
            Debug.Log("The Slot name is: " + currentSlot.slotName); //kontrolle des Namen
            if(currentSlot.talismanName != string.Empty) //Wenn nicht leer
            {
                Talisman formerTalisman = myCollections.AllTalismans.Find(x => x.TalismanName == currentSlot.talismanName); //Finde den Talisman der in der geklickten Slot bisher war
                formerTalisman.effect.Effect(EffectState.Removal.ToString()); //Mache Effekt rückgängig
                talismanHandler.CarriedTalismanNames.RemoveAll(x => x == formerTalisman.TalismanName); //Entferne aus getragenen alle mit dem Namen dieses Talismans (also nur diesen)
            }
            else
            {
                Debug.Log("Slot was empty!");
            }
              
            currentSlot.talismanName = clickedTalisman.TalismanName; //setze den geklickten Slot auf den neuen Namen
            inventoryUI.inventory.rootVisualElement.Q(currentSlot.slotName).style.backgroundImage = clickedTalisman.Image; //setze das Bild des Slots           
            clickedTalisman.effect.Effect(EffectState.Application.ToString()); //Löse den Effekt aus
            talismanDoc.rootVisualElement.style.display = DisplayStyle.None; //schliesse Selektor
            GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().OnTalismanSelectorClose(); //erlaubt schliessen des Inventars
            inventoryUI.inventory.rootVisualElement.style.display = DisplayStyle.Flex; //blende Inventar wieder ein
        }
        if (evt.button == 1) //wenn Rechtklick:
        {
            UIDocument document = GetComponent<UIDocument>(); //redundant da dasselbe wie talismanDoc, aber natürlich nicht falsch          
            Talisman talismanToRead = talismanHandler.Talismans.Find(x => x.TalismanName == (evt.target as VisualElement).Q(className: "talismanmain").name); //Finde Talisman mit Namen des Button
            document.rootVisualElement.Q<Label>("TalismanName").text = talismanToRead.TalismanWindowTitle; //Setze Titel des Erklärungsfeldes auf TalismanWindowTitle des Buttons
            document.rootVisualElement.Q<Label>("ItemDescription").text = talismanToRead.ItemDescription; //Setze Beschreibung
            document.rootVisualElement.Q("CloseView").style.backgroundImage = talismanToRead.Image; //Setze Bild der Detailansicht
        }
        
    }
}
