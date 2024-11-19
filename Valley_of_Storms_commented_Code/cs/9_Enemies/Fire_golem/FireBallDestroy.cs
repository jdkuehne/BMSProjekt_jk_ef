using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zerstörungsbedingungen für Feuerball, damit Feuerbälle nicht unendlich weit fliegen
public class FireBallDestroy : MonoBehaviour
{
    bool canBeDestroyed;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        canBeDestroyed = true;
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Is at least colliding");
        if(collision.gameObject.layer == LayerMask.NameToLayer("GroundNWalls") && canBeDestroyed)
        {
            Transform ps = transform.Find("Particles");
            ps.GetComponent<ParticleSystem>().Stop();
            ps.parent = null; //Deparenting, sodass Particle noch kurzzeitig besteht
            Destroy(gameObject);
        }
    }
}
