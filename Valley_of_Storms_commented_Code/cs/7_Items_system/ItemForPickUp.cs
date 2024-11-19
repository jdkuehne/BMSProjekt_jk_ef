using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemForPickUp : MonoBehaviour
{
    public IItem item;
    public ScriptableObject pickup; //Daten-Objekt, das bei Aufnehmen des Item erhalten wird    

    private void Start()
    {
        item = pickup as IItem;
    }

    private void OnCollisionEnter2D(Collision2D collision) //Fügt Objekt zu Liste von "aufnehmbaren" Objekten hinzu
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<MyCharacterCtrl>().PickupObjects.Add(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Player")) //Entfernt wieder
        {
            collision.gameObject.GetComponent<MyCharacterCtrl>().PickupObjects.Remove(gameObject);
        }

    }

}
