using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//deaktiviert zu Beginn des Spiels das Controls-Fenster
public class ControlsUIManager : MonoBehaviour
{
    private UIDocument controlsUI;
    void Start()
    {
        controlsUI = GetComponent<UIDocument>();
        controlsUI.rootVisualElement.style.display = DisplayStyle.None;                
    }
}
