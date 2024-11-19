using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wird einem GameObject mit Trigger in der Bossarena angefügt, startet Bosskampf
public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] float _bossRoomCamOffset; //Kamera-Offset während Kampf, damit gesamte Arena im Blick behalten werden kann
    string _bossName; //Name des Boss Objekts
    private void Start()
    {
        _bossName = GameObject.FindWithTag("Spawner").GetComponent<LevelSpawnManager>().spawnItems.Find(x => x.spawnType == SpawningDataHandler.SpawnType.Boss).itemName; //Finde Name des Bosses (nach Spawntype)    
    }
    private void OnTriggerEnter2D(Collider2D collision) //Callback Layer: PlayerBody => nur Player kann den Trigger auslösen
    {
        if (!SpawningDataHandler.Instance.killedBossEnemies.Contains(_bossName)) //Diese Schritte sollen nur ausgeführt werden, wenn der Boss auch tatsächlich gespawnt wird (und noch lebt)
        {
            GameObject boss = GameObject.FindWithTag("Boss");
            GameObject[] fogwalls = GameObject.FindGameObjectsWithTag("FogWall");
            foreach (GameObject fogwall in fogwalls)
            {
                fogwall.GetComponent<FogWall>().Activate(); //Aktiviere Nebel (Sperre, damit Bossraum nicht verlassen werden kann, während der Kampf läuft)
            }
            GameObject.FindWithTag("MainCamera").GetComponent<CameraFollow>().SetBossRoomCamOffset(_bossRoomCamOffset); //Offset für Kamera
            GetComponent<Collider2D>().enabled = false; //Trigger wird deaktiviert
            boss.GetComponent<BossController>().BossTrig = true; //Boss AI gestartet
        }
    }
}
