using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanHandler : MonoBehaviour
{
    [HideInInspector] public bool Sorted = false;
    public List<Talisman> Talismans = new(); //gesammelte Talismane
    public List<string> CarriedTalismanNames = new(); //ausgerüstete Talismane
    private CollectionService myCollections; //alle Talismane im Spiel
    private IEnumerator Start()
    {
        myCollections = GameObject.FindWithTag("SaveHandler").GetComponent<CollectionService>();
        Talismans.Clear();
        Talismans.AddRange(myCollections.AllTalismans.FindAll(x => SaveHandler.Instance.theSave.TalismanNames.Exists(y => y == x.TalismanName)));
        CarriedTalismanNames.Clear();
        CarriedTalismanNames.AddRange(SaveHandler.Instance.theSave.CarriedTalismanNames);
        TalismanSort();        
        yield return StartCoroutine(GOExtensions.WaitForComponentOnGOWithTag<MyCharacterCtrl>("Player"));
        Sorted = true;
        yield return null;
        MyCharacterCtrl.OnPickup += AddTalisman;
        
    }

    //Methode, die Talismane nach ihrer Sortiernummer ordnet
    public void TalismanSort()
    {
        Talismans.Sort(delegate(Talisman x, Talisman y)
        {
            if (y == null) return 1;
            else return x.SortNumber.CompareTo(y.SortNumber);
        });
    }

    //ausgelöst bei Pickup, fügt Talisman zu Liste hinzu und löst UI-Methode zur Erstellung des Elements aus
    public void AddTalisman(object obj, PickupEventArgs args)
    {
        if (args.ItemInformation.ItemType == MyCharacterCtrl.ItemType.Talisman) 
        {
            Talismans.Add(args.PickupItem as Talisman);
            TalismanSort();
            GameObject.FindWithTag("UI-Talisman").GetComponent<TalismanUIManager>().NewTalismanAdded();
        }
    }
}
