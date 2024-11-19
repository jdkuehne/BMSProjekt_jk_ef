using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es können bei Scene Gates jeweils dieselben Instanzen dieser ScriptableObject-Klasse verwendet werde, um eine Verbindung zwischen diesen zwei Toren darzustellen.
[CreateAssetMenu(menuName = "SceneManagement/Connection")]
public class LevelConnection : ScriptableObject
{
    public static LevelConnection ActiveConnection { get; set; }
}
