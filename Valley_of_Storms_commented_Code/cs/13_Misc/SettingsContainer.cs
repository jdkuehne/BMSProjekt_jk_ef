using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//ScriptableObject um Settings zwischen Scene und Startmenu beizubehalten
[CreateAssetMenu(fileName = "SettingsCont", menuName = "Settings")]
public class SettingsContainer : ScriptableObject
{
    public Settings savedSettings = new()
    {
        indexRes = 0,
        fullscreen = true,
        masterVolume = 7,
        vsyncBool = true
    };

    public bool firstSet = true; //So the UI-Code can set the index value to the number of resolutions
}
