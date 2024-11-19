using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//Wird jeder Klinge des Bosses angefügt, signalisiert Animationsende
public class BladesEvent : MonoBehaviour
{
    public bool BladeAnimDone = false;
    public void BladesEventFunc()
    {
        BladeAnimDone = true;
    }

    public void EnableColliders() //aktiviert beide Collider dieser Klinge
    {
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = true;
        }
        GetComponent<Light2D>().enabled = true;
    }

    public void DisableColliders() //deaktiviert
    {
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        GetComponent<Light2D>().enabled = false;
    }

}
