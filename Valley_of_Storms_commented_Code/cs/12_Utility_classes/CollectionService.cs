using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Diese Klasse sollte Zugriff auf Objekte per Name usw. ermöglichen => nur der Name wir gespeichert (weniger Speicherplatz)
//Das Objekt wird über die ID bzw. den Namen hier gefunden und zugewiesen, momentan nur Talismane
public class CollectionService : MonoBehaviour
{
    public List<Talisman> AllTalismans = new(); 
}
