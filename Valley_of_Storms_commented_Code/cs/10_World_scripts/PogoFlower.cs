using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PogoFlower ist eine "magische" Blume, von der sich der Spieler über eine Abwärtsattacke abstossen kann, dieses Script löst bei Treffern ein Particle aus
public class PogoFlower : MonoBehaviour
{
    [SerializeField] private GameObject HitParticle;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player-Weapon"))
        {
            Instantiate(HitParticle, gameObject.transform);
        }
    }
}
