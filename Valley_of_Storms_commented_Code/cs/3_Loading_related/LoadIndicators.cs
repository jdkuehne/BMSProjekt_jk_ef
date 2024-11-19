using UnityEditor;
using UnityEngine;

//Einfacher Enum der beim Laden Auskunft darüber liefert, ob der Spieler gestorben (Checkpoint) oder durch ein Tor gegangen (Gate) gegangen ist.
public static class LoadIndicators
{
    public enum LoadOption
    {
        Checkpoint,
        Gate,
        WaitingForLoad
    }
    public static string LastLoadOption = LoadOption.WaitingForLoad.ToString();   
}
