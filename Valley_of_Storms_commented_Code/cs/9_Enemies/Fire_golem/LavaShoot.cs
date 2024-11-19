using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Behaviour für Feuerspeier/-golems, erstellt regelmässig Feuerbälle
public class LavaShoot : MonoBehaviour
{
    [field: SerializeField] public GameObject Prefab { get; private set; } //Feuerball
    [field: SerializeField] public Transform StartPoint { get; private set; }
    [SerializeField] private ShootParameters LavaShot;
    public bool isInRange; //static vars aren't Threadsafe
    IEnumerator Start()
    {
        //isInRange = false;
        //yield return new WaitUntil(() => isInRange);
        yield return null;
        while (true)
        {
            GameObject prefabInstance = Instantiate(Prefab, StartPoint.position,transform.rotation, transform); //Erstelle/Instanziere Feuerball
            prefabInstance.GetComponent<Rigidbody2D>().velocity = LavaShot.Speed; //Setze Geschwindigkeit (keine Gravitation bei Feuerbällen, da dies Ausweichen schwierig machen würde)
            //yield return new WaitUntil(() => isInRange);
            yield return new WaitForSeconds(LavaShot.Period);
        }
    }    
}
