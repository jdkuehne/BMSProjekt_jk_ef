using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; //Die Library für Speichern bzw. JSON-Serialisierung

//Klasse für JSON-Methoden, grösstenteils übernommen von Video: https://youtu.be/mntS45g8OK4
public class JsonDataService : IDataService //Interface für Speicherklassen
{
    public bool SaveData(string relativePath, SaveData data, bool encrypted) //bool ermöglicht einfache Bestätigung: Funktion in if-Statement
    {
        string path = Application.persistentDataPath + relativePath; //Erstelle Pfad aus Unity Standard Pfad und unserem Dateinamen
        try
        {
            Debug.Log(path); //Pfad an Console zum Überprufen
            if (File.Exists(path)) //Wenn File schon existiert:
            {
                Debug.Log("Data exists. Deleting old File and writing new.");
                File.Delete(path); //entferne dieses File
            }
            else
            {
                Debug.Log("Creating new json file!");
            }
            using FileStream stream = File.Create(path); //Erstelle Datei
            stream.Close(); //Beende Schreibprozess
            File.WriteAllText(path, JsonConvert.SerializeObject(data)); //Lies Datenobjekt, wandle in JSON Format, schreibe JSON-Text in erstelltes File
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}"); //Error Message
            return false;
        }        
    }

    public SaveData LoadData(string relativePath, bool encrypted) //Lade Daten wieder
    {
        string path = Application.persistentDataPath + relativePath;//"

        if (File.Exists(path)) //wenn existiert:
        {
            try
            {
                SaveData data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path)); //Schreibe Inhalt in Objekt
                Debug.Log(path);
                return data; //gib Objekt zurück
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load due to: {e.Message} {e.StackTrace}");
                throw e;
            }

        }
        else
        {
            Debug.Log("No File in this directory, data set to default");
            return new SaveData //sonst erstelle default Objekt und gib dieses zurück
            {
                firstSessionOnThisSave = false,
                StartSceneName = "defaultFlag",
                StartShrinePosition = new(0, 0, 0),
                pickedUpItemNames = new List<string>(),
                killedBigEnemyNames = new List<string>(),
                killedBossEnemyNames = new List<string>(),
                TalismanSlotData = new List<MenuThread>(),
                TalismanNames = new List<string>(),
                CarriedTalismanNames = new List<string>(),
                savedSettings = new Settings()
            };   
        }            
    }    
}
