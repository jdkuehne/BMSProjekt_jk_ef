using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Kann an Spawner-GameObject angefügt werden, sodass im Editor zu spawnende Objekte definiert werden können, fragt immer ab, ob Objekt gespawnt werden darf
public class LevelSpawnManager : MonoBehaviour
{
    public List<SpawnItem> spawnItems = new();

    //Immer bei Load der Scene ausgelöst
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Initializer.InitInstance.NeverDestroyLoaded);
        yield return new WaitUntil(() => SpawningDataHandler.Instance.Updated);
        foreach (var item in spawnItems)
        {            
            
            switch (item.spawnType)
            {
                case SpawningDataHandler.SpawnType.AlwaysRespawn: //Frage nach Spawntyp, grosser Gegner sollte bei Tod oder Rasten geresettet, Boss sollte nie geresettet werden
                    GameObject current = Instantiate(item.prefab, item.target.position, item.target.rotation); //Erstellt das Objekt immer
                    current.name = item.itemName; 
                    break;
                case SpawningDataHandler.SpawnType.Item:
                    if (SpawningDataHandler.Instance.pickedUpItems.Find(x => x == item.itemName) == null) //Erstellt das Item nur wenn noch nicht aufgenommen
                    {
                        GameObject currentitem = Instantiate(item.prefab, item.target);
                        currentitem.name = item.itemName;                         
                    } 
                    break;
                case SpawningDataHandler.SpawnType.BigEnemy:
                    if (SpawningDataHandler.Instance.killedBigEnemies.Find(x => x == item.itemName) == null) //Spawnt Gegnern nur, wenn in diesem Versuch noch nicht besiegt
                    {
                        GameObject currentbeeg = Instantiate(item.prefab, item.target);
                        currentbeeg.name = item.itemName;
                    }
                    break;
                case SpawningDataHandler.SpawnType.Boss:
                    if (SpawningDataHandler.Instance.killedBossEnemies.Find(x => x == item.itemName) == null) //Spawnt nur bis zum ersten Besiegen, Liste wird nie geresettet, abgesehen von Löschen des Speicherstands, wie Items
                    {
                        GameObject currentitem = Instantiate(item.prefab, item.target);
                        currentitem.name = item.itemName;
                    }
                    break;
            }
        }        
    }    
}
