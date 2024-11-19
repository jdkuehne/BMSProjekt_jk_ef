using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parry Box für Steinrücken des Trolls
public class ParryBox : MonoBehaviour
{
    Sword playerSword;
    [SerializeField] bool hasParticle;
    [SerializeField] GameObject ParryParticle;
    //[SerializeField] Transform ParticleOrigin;
    [SerializeField] GameObject HittableSibling; //Das Objekt, das geschützt wird
    
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Initializer.InitInstance.NeverDestroyLoaded);
        playerSword = GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().Sword.GetComponent<Sword>();                
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 playerPos = GameObject.FindWithTag("Player").transform.position;
        Vector2 shieldPos = transform.position;
        Vector2 bodyPos = HittableSibling.transform.position;
        float playerToShield = Vector2.Distance(playerPos, shieldPos);
        float playerToBody = Vector2.Distance(playerPos, bodyPos);
        //Wenn die ParryBox näher am Spieler war als die Hitbox des zu schützenden Objekts, wird das Parieren ausgelöst und optional ein Particle erstellt
        if (!playerSword.EnemiesHitThisAttack.Contains(HittableSibling) && playerToShield < playerToBody) //Trigger being the PlayerWeapon's is already made sure in Callbacklayers
        {
            playerSword.Parried = true;
            Debug.Log($"Parried while: bodyPos = {bodyPos}, shieldPos = {shieldPos}, collisionPos = {playerPos}");
            if (hasParticle) { Instantiate(ParryParticle, transform.position, transform.rotation); }
        }       
    }
}
