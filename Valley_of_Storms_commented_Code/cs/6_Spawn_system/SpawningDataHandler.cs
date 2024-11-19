using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningDataHandler : MonoBehaviour
{
    public static SpawningDataHandler Instance;

    [HideInInspector] public bool Updated = false;

    public List<string> pickedUpItems = new(); //Listen für auseinanderhalten der Spawntypen
    public List<string> killedBigEnemies = new();
    public List<string> killedBossEnemies = new();
    

    public enum SpawnType //Definition der Spawntypen
    {
        Item,
        Boss,
        BigEnemy,
        AlwaysRespawn
    }

    private void Awake() //Singleton
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start() //Liest Speicherstand
    {
        pickedUpItems.Clear();
        pickedUpItems.AddRange(SaveHandler.Instance.theSave.pickedUpItemNames);
        killedBigEnemies.Clear();
        killedBigEnemies.AddRange(SaveHandler.Instance.theSave.killedBigEnemyNames);
        killedBossEnemies.Clear();
        killedBossEnemies.AddRange(SaveHandler.Instance.theSave.killedBossEnemyNames);
        Updated = true;
    }
}
