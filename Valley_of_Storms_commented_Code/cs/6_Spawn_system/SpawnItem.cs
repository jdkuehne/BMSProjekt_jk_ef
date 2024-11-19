using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Daten für zu spawnendes Objekt, kann in Editor erstellt werden (wird im Inspektor an LevelSpawnManager angefügt)
[System.Serializable]
public struct SpawnItem
{
    public GameObject prefab;
    public Transform target;
    public string itemName;
    public SpawningDataHandler.SpawnType spawnType;
}
