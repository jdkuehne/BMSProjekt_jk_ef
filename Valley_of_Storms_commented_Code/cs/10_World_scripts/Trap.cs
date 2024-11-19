using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<HealthV2>().Damage(100, true); //100 Schaden, true bedeutet, Falle => Position reset
        }
    }
}
