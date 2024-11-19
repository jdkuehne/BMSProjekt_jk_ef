using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Die Erstellung eines GameObjects mit diesem Behaviour wird in gewissen Scripts benötigt, um sicherzugehen, dass die Scene vollständig geladen ist
//Dies könnte aber eleganter gelöst werden, wir haben im Moment eine Lösung wo viele Handler erst nach der ersten Kammer geladen werden
public class LoadCompleteEventTrigger : MonoBehaviour
{    
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Initializer.InitInstance.NeverDestroyLoaded);
        ShrineHandler.shrinehandler.TriggerLoadEvent = true;
    }    
}
