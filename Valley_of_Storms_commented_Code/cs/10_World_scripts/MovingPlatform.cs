using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

//Klasse für Bewegung von Plattform und allfälliges Mitbewegen von Spieler
public class MovingPlatform : MonoBehaviour
{
    private GameObject playerParent;
    private MyCharacterCtrl playerCtrl;
    [SerializeField] private Transform spotLeft;
    [SerializeField] private Transform spotRight;
    float time;
    [SerializeField] float duration;
    bool headingRight;
    bool started = false;
    public bool startAll = false;
    IEnumerator Start()
    {
        time = 0;
        yield return new WaitUntil(() => Initializer.InitInstance.NeverDestroyLoaded);        
        playerParent = GameObject.FindWithTag("PlayerParent"); //Finde Parent, das mitbewegt werden muss, wenn Spieler auf Plattform steht
        playerCtrl = GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>();

        //Erhalte Liste aller beweglichen Plattformen in Raum, damit alle gleichzeitig starten können
        GameObject[] allmoving = GameObject.FindGameObjectsWithTag("MovingPlatform");
        List<GameObject> listallmoving = new();
        listallmoving.AddRange(allmoving);

        //Wait for all
        startAll = true;
        yield return new WaitUntil(() => !listallmoving.Exists((x) => !x.GetComponent<MovingPlatform>().startAll));
        
        transform.position = spotLeft.transform.position; //Startposition
        headingRight = true;
        started = true;
    }

    private void FixedUpdate() //Lerp zwischen Positionen und Richtungswechsel
    {
        if (started)
        {
            time += Time.fixedDeltaTime;
            if (headingRight)
            {
                transform.position = Vector2.Lerp(spotLeft.position, spotRight.position, time / duration); //Bewegt Plattform innerhalb duration von spotLeft (GameObject) zu spotRight
            }
            else
            {
                transform.position = Vector2.Lerp(spotRight.position, spotLeft.position, time / duration);
            }
            if (headingRight && time >= duration) //Jeweils Toggle, wenn Zeit erreicht ist
            {
                headingRight = false;
                time = 0;
            }
            if (!headingRight && time >= duration)
            {
                headingRight = true;
                time = 0;
            }

            if (playerCtrl.PDetect.IsMovingPlat && playerCtrl.PDetect.MovingPlatform == gameObject) //Wenn der Spieler auf dieser Plattform steht, bewege Parent mit (damit Plattform Spieler nicht unter Füssen wegrutscht)
            {
                playerParent.transform.position = transform.position;
            }
        }        
    }
}
