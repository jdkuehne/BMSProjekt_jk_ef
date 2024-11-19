using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UIEnums;

//UI Manager für das Inventar-Fenster
public class InventoryUIManager : MonoBehaviour
{
    public UIDocument inventory;
    [SerializeField] GameObject talismanSelect;
    public List<MenuThread> TalismanSlots = new();
    public MenuThread currentThread;
    private CollectionService myCollections;
    
    //public TalismanEffect emptyEffect;

    [Header("Slot Names")]
    public string FirstTalismanSlot;
    public string SecondTalismanSlot;
    public string ThirdTalismanSlot;

    IEnumerator Start()
    {
        inventory = GetComponent<UIDocument>();
        myCollections = GameObject.FindWithTag("SaveHandler").GetComponent<CollectionService>();
        //talismanSelect = GameObject.FindWithTag("UI-Talisman");
        yield return null;
        if (SaveHandler.Instance.theSave.firstSessionOnThisSave)
        {
            TalismanSlots.Clear();
            TalismanSlots.Add(new MenuThread(FirstTalismanSlot, string.Empty)); //Anlegen von drei "Talisman Slot"-Datenobjekten
            TalismanSlots.Add(new MenuThread(SecondTalismanSlot, string.Empty));
            TalismanSlots.Add(new MenuThread(ThirdTalismanSlot, string.Empty));
        }
        else
        {
            TalismanSlots.Clear();
            TalismanSlots.AddRange(SaveHandler.Instance.theSave.TalismanSlotData);
            //schon bei vorherigen Sessions ausgestattete Talisman-Konfiguration wiederherstellen:            
            foreach (var slot in TalismanSlots) 
            {
                if (slot.talismanName != string.Empty)
                {
                    inventory.rootVisualElement.Q(slot.slotName).style.backgroundImage = myCollections.AllTalismans.Find(x => x.TalismanName == slot.talismanName).Image;
                    myCollections.AllTalismans.Find(x => x.TalismanName == slot.talismanName).effect.Effect(EffectState.Application.ToString());
                }
            }
        }
        GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().OnPray.Invoke();
        inventory.rootVisualElement.Q(FirstTalismanSlot).RegisterCallback<MouseDownEvent>(OnSlotClick); //Event Registrierung bei Slots
        inventory.rootVisualElement.Q(SecondTalismanSlot).RegisterCallback<MouseDownEvent>(OnSlotClick);
        inventory.rootVisualElement.Q(ThirdTalismanSlot).RegisterCallback<MouseDownEvent>(OnSlotClick);
        inventory.rootVisualElement.style.display = DisplayStyle.None;
    }
    
    //Diese Methode öffnet den Talisman Selektor und macht Informationen zum Slot für den TalismanUIManager verfügbar
    private void OnSlotClick(MouseDownEvent evt)
    {
        GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().OnTalismanSelectorOpen();
        inventory.rootVisualElement.style.display = DisplayStyle.None;
        currentThread = new MenuThread((evt.target as VisualElement).name, TalismanSlots.Find(x => x.slotName == (evt.target as VisualElement).name).talismanName);
        //Debug.Log((evt.target as VisualElement).name);
        talismanSelect.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }
}
