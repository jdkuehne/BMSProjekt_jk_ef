using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface für Speichermethoden
public interface IDataService
{
    bool SaveData(string relativePath, SaveData data, bool encrypted);
    SaveData LoadData(string relativePath, bool encrypted);
}
