using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

//Dieses Einfache Behaviour nutzt Lerp um die Transparenz des Baumes in Kammer1 zu steuern
public class BigTreeFront : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht anhängen an GameObject
{
    [SerializeField] private Color _frontBaseColor; //nicht Transparente Farbe (Color Picker in Editor)
    [SerializeField] private Color _frontInvisibleColor; //Farbe wenn transparent
    private SpriteRenderer sr; //Renderer-Komponente der Baum-Front

    float colorStatusTarget; //Ziel, wird zwischen 0 und 1 getoggelt
    float currentColorStatus; //Momentanwert
    

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        colorStatusTarget = 0f; //nicht transparent
        currentColorStatus = 0f;
    }

    private void Update() //Jeden Frame ausgeführt
    {
        currentColorStatus += (colorStatusTarget - currentColorStatus) * 7f * Time.deltaTime; //Status = Target-e^(-x)? Haben uns bei der Idee an Kondensatorladekurve orientiert
        sr.color = Color.Lerp(_frontBaseColor, _frontInvisibleColor, currentColorStatus); //Lerp linearinterpoliert die Farbe mit dem Dezimalwert currentColorStatus von 0...1
        //entsprechend ist 0.5f halbtransparent, 1 transparent und 0 opak
    }

    private void OnTriggerEnter2D(Collider2D collision) //Kommt man in den Trigger wird die Front transparent
    {
        colorStatusTarget = 1f;
    }

    private void OnTriggerExit2D(Collider2D collision) //Verlässt man den Bereich wird die Front wieder opak
    {
        colorStatusTarget = 0;
    }    
}
