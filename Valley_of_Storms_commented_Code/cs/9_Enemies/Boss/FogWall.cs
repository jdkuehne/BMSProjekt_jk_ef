using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogWall : MonoBehaviour
{
    float opacityStatusTarget;
    float currentOpacityStatus;

    SpriteRenderer sr;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        opacityStatusTarget = 0f;
        currentOpacityStatus = 0f;
    }

    void Update()
    {
        currentOpacityStatus += (opacityStatusTarget - currentOpacityStatus) * 3f * Time.deltaTime;
        sr.color = new Color { r = 1, g = 1, b = 1 , a = Mathf.Clamp(currentOpacityStatus, 0f, 1f)};         
    }

    public void Deactivate()
    {
        GetComponent<Collider2D>().enabled = false; //Deaktiviere Collider
        opacityStatusTarget = 0f; //Setzt Ziel der Farbe auf transparent
    }
    public void Activate() 
    {
        GetComponent<Collider2D>().enabled = true; //aktiviere...
        opacityStatusTarget = 1f;
    }
}
