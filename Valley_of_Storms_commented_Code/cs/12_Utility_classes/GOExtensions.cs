using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOExtensions
{
    public static IEnumerator WaitForComponentOnGOWithTag<T>(string tag) where T : Component //Falls Erstellung eines GameObject abgewartet werden muss,
    //kann dies mit dieser Methode gemacht werden (Das GameObject benötigt aber einen Tag, damit die Methode das GameObject finden kann)
    {
        while (GameObject.FindWithTag(tag) == null)
        {
            yield return null;
        }
        GameObject go = GameObject.FindWithTag(tag);
        while(!go.TryGetComponent(out T output))
        {
            //Debug.Log(output.ToString());
            yield return null;
        }
    }    
}
