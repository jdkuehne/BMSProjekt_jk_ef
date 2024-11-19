using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Selbstzerstörungstimer für GameObjects wie den "besiegten" Troll, der aus dem Bild fliegt und anschliessend zerstört werden sollte
public class DestroyTimer : MonoBehaviour
{
    [SerializeField] float timeUntilDestroy;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(timeUntilDestroy);
        Destroy(gameObject);
    }    
}
