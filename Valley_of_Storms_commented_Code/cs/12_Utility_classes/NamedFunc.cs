using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Delegat mit Name z.B. für Talisman-Effekt, der Effekt kann in Befehlsliste wieder anhand des Namen gefunden und entfernt werden (Func -> mit Rückgabewert)
[Serializable]
public class NamedFunc<T, TResult>
{
    [field: SerializeField] public string Name { get; set; }
    public Func<T, TResult> Method;

    public NamedFunc(string name, Func<T, TResult> func)
    {
        Name = name;
        Method = func;
    }
}
