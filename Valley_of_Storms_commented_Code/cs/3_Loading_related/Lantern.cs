using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Klasse für die Latenen, Checkpoints die nur für Plattforming-Passagen funktionieren und auch nicht bei Tod
public class Lantern : MonoBehaviour
{    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ShrineHandler.shrinehandler.LanternSpawnPosition = transform.position;
        }
    }
}
