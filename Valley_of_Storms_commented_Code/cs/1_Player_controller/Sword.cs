using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//Klasse, die Handling von Schwerttreffern ausführt
public class Sword : MonoBehaviour
{
    public bool alreadyPogoed;
    public bool Parried;
    public List<GameObject> EnemiesHitThisAttack = new();
    private Damage playerDamage;

    public List<NamedAction<int>> TalismanAttackEffects = new();

    private void Start()
    {
        playerDamage = GameObject.FindWithTag("Player").GetComponent<Damage>(); //finden des "Schaden" Stats
        Parried = false;
    }

    //Verhalten bei Treffer
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Überprüfe ob Treffer pariert wird
        if (!other.gameObject.CompareTag("OnlyPogo") && !EnemiesHitThisAttack.Contains(other.gameObject))
        {
            StartCoroutine(CheckParry(other));
        }

        //Abstossen, wenn eine Abwärtsattacke gelandet wurde
        Rigidbody2D pRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        MyCharacterCtrl ctrl = pRb.GetComponent<MyCharacterCtrl>();
        if (ctrl.IsPogo && !alreadyPogoed)
        {
            pRb.velocity = new Vector2(pRb.velocity.x, ctrl.PParams.PogoImpulse);
            alreadyPogoed = true;
        }
    }

    //Kontrolliert ob eine Parierhitbox im Weg stand, löst sonst den Schaden aus
    IEnumerator CheckParry(Collider2D other)
    {
        for (float time = 0f; time < 0.1f; time += Time.deltaTime)
        {
            if (Parried)
            {
                yield break;
            }
            yield return null;
        }

        yield return null;

        if (other.gameObject.TryGetComponent(out HealthV2 otherhealth))
        {
            foreach (var action in TalismanAttackEffects)
            {
                action.Method(playerDamage.Value);
            }
            otherhealth.Damage(playerDamage.Value, false);
            EnemiesHitThisAttack.Add(other.gameObject);
            Debug.Log("Damaged " + other.gameObject.name + " Value: " + playerDamage.Value);
        }
    }
}


