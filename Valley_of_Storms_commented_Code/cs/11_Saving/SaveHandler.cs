using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class SaveHandler : MonoBehaviour
{
    public SaveData theSave;
    public SaveData defaultSave;
    [SerializeField] Transform defaultSpawnPos;
    [SerializeField] TextMeshProUGUI jsonLog;
    [SerializeField] TextMeshProUGUI saveTimeLog;

    public bool SettingsLoaded = false;

    public const string path = @"/valley_of_storms_data.json"; //unser relativer Pfad, const, damit von anderen Scripts darauf zugegriffen werden kann und keine Änderungen geschehen

    private bool _encryptionEnabled = false;
    private IDataService dataService = new JsonDataService(); //Enthält Speichermethoden
    private long _saveTime; //obsolet
    
    public static SaveHandler Instance { get; private set; } //Singleton-Instanz für einfacheren externen Zugriff, kein FindWithTag.GetComponent...

    private void Awake() //Singleton
    {
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (SaveMode.Saving)
        {
            theSave = dataService.LoadData(path, _encryptionEnabled);
            if(theSave.StartSceneName == "defaultFlag") { WriteDefault(); }
        }
        else { WriteDefault(); }
    }

    public void ToggleEncryption(bool encryptionEnabled)
    {
        _encryptionEnabled = encryptionEnabled;
    }
    
    public static void WriteDefault() //Diese Daten werden so geschrieben, dass keine Referenzen zur Scene bestehen => Listen mit Clear und AddRange statt gleichsetzen 
    {
        Instance.theSave.firstSessionOnThisSave = Instance.defaultSave.firstSessionOnThisSave;
        Instance.theSave.StartSceneName = Instance.defaultSave.StartSceneName;
        Instance.theSave.StartShrinePosition = Instance.defaultSave.StartShrinePosition;
        Instance.theSave.pickedUpItemNames.Clear();
        Instance.theSave.killedBigEnemyNames.Clear();
        Instance.theSave.killedBossEnemyNames.Clear();
        Instance.theSave.pickedUpItemNames.AddRange(Instance.defaultSave.pickedUpItemNames);
        Instance.theSave.killedBigEnemyNames.AddRange(Instance.defaultSave.killedBigEnemyNames);
        Instance.theSave.killedBossEnemyNames.AddRange(Instance.defaultSave.killedBossEnemyNames);
        Instance.theSave.TalismanSlotData.Clear();
        Instance.theSave.TalismanNames.Clear();
        Instance.theSave.CarriedTalismanNames.Clear();
        //Instance.theSave.TalismanSlotData.AddRange();
        Instance.defaultSave.TalismanSlotData.ForEach(x => Instance.theSave.TalismanSlotData.Add(new MenuThread(x.slotName, x.talismanName)));
        Instance.theSave.TalismanNames.AddRange(Instance.defaultSave.TalismanNames);
        Instance.theSave.CarriedTalismanNames.AddRange(Instance.defaultSave.CarriedTalismanNames);
        Instance.theSave.savedSettings = Instance.defaultSave.savedSettings;        
    }

    public IEnumerator Save() //Methode für einfachen Speichervorgang
    {
        //shrine is set on collision
        /*theSave.pickedUpItemNames = */
        theSave.pickedUpItemNames.Clear();
        theSave.killedBigEnemyNames.Clear();
        theSave.killedBossEnemyNames.Clear();
        theSave.pickedUpItemNames.AddRange(SpawningDataHandler.Instance.pickedUpItems);
        theSave.killedBigEnemyNames.AddRange(SpawningDataHandler.Instance.killedBigEnemies);
        theSave.killedBossEnemyNames.AddRange(SpawningDataHandler.Instance.killedBossEnemies);              
        theSave.TalismanSlotData.Clear();
        theSave.TalismanSlotData.AddRange(GameObject.FindWithTag("UI-Inventory").GetComponent<InventoryUIManager>().TalismanSlots);
        theSave.TalismanNames.Clear();
        GameObject.FindWithTag("DataHandler").GetComponent<TalismanHandler>().Talismans.ForEach(x => { theSave.TalismanNames.Add(x.TalismanName); });
        theSave.CarriedTalismanNames.Clear();
        theSave.CarriedTalismanNames.AddRange(GameObject.FindWithTag("DataHandler").GetComponent<TalismanHandler>().CarriedTalismanNames);
        //Settings are set in the SettingsUIManager class
        theSave.firstSessionOnThisSave = false;
        dataService.SaveData(path, theSave, _encryptionEnabled); //Schreibe Daten in File
        yield return null;
    }
}
