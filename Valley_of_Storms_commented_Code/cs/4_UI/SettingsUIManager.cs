using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using Unity.VisualScripting;

//UI Manager des Fensters für technische Einstellungen
public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument settingsUI;
    [SerializeField] private AudioMixer audioMixer;
    //[SerializeField] private SettingsContainer container;
    private Toggle screenSetting;
    private SliderInt masterVol;
    private DropdownField res;
    private Label applyButton;
    private Toggle vsyncToggle;
    
    Settings currentSettings;

    //Diese Funcs setzen gespeicherte Werte in den für Unity Settings Methoden benötigte Datentypen um
    public static readonly Func<bool, FullScreenMode> boolToMode = (x) =>
    {
        if (x) return FullScreenMode.FullScreenWindow;
        else return FullScreenMode.Windowed;
    };
    public static readonly Func<FullScreenMode, bool> modeToBool = (x) =>
    {
        if (x == FullScreenMode.FullScreenWindow) return true;
        else return false;
    };
    public static readonly Func<bool, int> boolToVsync = (x) =>
    {
        if (x) return 1;
        else return 0;
    };
    public static readonly Func<int, bool> vsyncToBool = (x) =>
    {
        if (x == 1) return true;
        else return false;
    };

    //Liste aller vom Bildschirm unterstützten Auflösungen
    private List<(int width, int height)> resolutions = new();

    void Start()
    {        
        settingsUI = GetComponent<UIDocument>();
        screenSetting = settingsUI.rootVisualElement.Q<Toggle>("screenSetting");
        masterVol = settingsUI.rootVisualElement.Q<SliderInt>("masterVol");
        res = settingsUI.rootVisualElement.Q<DropdownField>("res");
        applyButton = settingsUI.rootVisualElement.Q<Label>("applyButton");
        vsyncToggle = settingsUI.rootVisualElement.Q<Toggle>("vsync");
        
        //Finden unterstützter Auflösungen
        foreach(Resolution res in Screen.resolutions)
        {
            (int width, int height) resTuple = new() { width = res.width, height = res.height };            
            resolutions.Add(resTuple);
        }

        //Der Dropdownliste für jede Auflösung eine Option hinzufügen
        foreach ((int width, int height) resolution in resolutions)
        {
            res.choices.Add(resolution.width + "x" + resolution.height);
        }
        if (SaveHandler.Instance.theSave.firstSessionOnThisSave && !SaveHandler.Instance.SettingsLoaded)
        {
            SaveHandler.Instance.theSave.savedSettings.indexRes = resolutions.Count - 1;
            SaveHandler.Instance.SettingsLoaded = true;
        }        
        currentSettings = new Settings
        {
            indexRes = SaveHandler.Instance.theSave.savedSettings.indexRes,
            fullscreen = SaveHandler.Instance.theSave.savedSettings.fullscreen,
            masterVolume = SaveHandler.Instance.theSave.savedSettings.masterVolume,
            vsyncBool = SaveHandler.Instance.theSave.savedSettings.vsyncBool,
        };
        
        screenSetting.value = currentSettings.fullscreen; //bool
        masterVol.value = currentSettings.masterVolume;
        res.index = currentSettings.indexRes;
        vsyncToggle.value = currentSettings.vsyncBool;

        //*************************************************************************************************************************       
        
        ApplySettings();

        res.RegisterValueChangedCallback(SetRes); //Registrierung der Callbacks der UI Elemente
        screenSetting.RegisterValueChangedCallback(SetFullScreenMode);
        masterVol.RegisterValueChangedCallback(SetMasterVol);
        applyButton.RegisterCallback<MouseDownEvent>(InvokeApply);
        vsyncToggle.RegisterValueChangedCallback(SetVsync);
        
        settingsUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    //Methoden, welche das Settingsobjekt jeweils auf den richtigen Wert updaten, jedoch wird noch nichts ausgeführt ohne ApplySettings()
    private void SetRes(ChangeEvent<string> evt)
    {
        currentSettings.indexRes = res.index;
    }

    private void SetFullScreenMode(ChangeEvent<bool> evt)
    {
        currentSettings.fullscreen = evt.newValue;
    }

    private void SetMasterVol(ChangeEvent<int> evt)
    {
        currentSettings.masterVolume = evt.newValue;
    }

    private void SetVsync(ChangeEvent<bool> evt)
    {
        currentSettings.vsyncBool = evt.newValue;
    }
    
    private void InvokeApply(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            ApplySettings();
        }
    }
    
    //Methode zum Anwenden der technischen Einstellungen, bei Drücken von "Apply" Button
    private void ApplySettings()
    {
        int width = int.Parse(res.value.Substring(0, res.value.IndexOf('x')));
        int height = int.Parse(res.value.Substring(res.value.IndexOf('x') + 1));
        Screen.SetResolution(width, height, boolToMode(currentSettings.fullscreen));
        audioMixer.SetFloat("MasterVolume", currentSettings.masterVolume - 25f);
        QualitySettings.vSyncCount = boolToVsync(currentSettings.vsyncBool);
        SaveHandler.Instance.theSave.savedSettings = currentSettings;
    }
    
    //Methode zum Zurücksetzen der Einstellungen, wenn das Fenster, ohne "Apply" zu drücken, geschlossen wird
    public void SetUIToLastApplied()
    {
        screenSetting.value = SaveHandler.Instance.theSave.savedSettings.fullscreen;
        masterVol.value = SaveHandler.Instance.theSave.savedSettings.masterVolume;        
        res.index = SaveHandler.Instance.theSave.savedSettings.indexRes;
        vsyncToggle.value = SaveHandler.Instance.theSave.savedSettings.vsyncBool;
    }
}

//struct mit Settings in JSON-serialisierbarem Format
[System.Serializable]
public struct Settings
{
    public int indexRes;
    public bool fullscreen;
    public int masterVolume;
    public bool vsyncBool;
}
