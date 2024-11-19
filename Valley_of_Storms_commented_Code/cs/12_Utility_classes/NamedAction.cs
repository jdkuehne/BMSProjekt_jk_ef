using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Delegat mit Name z.B. für Talisman-Effekt, der Effekt kann in Befehlsliste wieder anhand des Namen gefunden und entfernt werden (Action -> keine Rückgabewert)
[Serializable]
public class NamedAction<T>
{
    [field: SerializeField] public string Name { get; set; }
    public Action<T> Method;

    public NamedAction(string name, Action<T> func)
    {
        Name = name;
        Method = func;
    }
}
