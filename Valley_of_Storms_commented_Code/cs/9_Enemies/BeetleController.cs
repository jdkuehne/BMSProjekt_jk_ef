using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Einfaches Controllerscript für Hin- und Herbewegen des Käfer-Gegners
public class BeetleController : MonoBehaviour
{
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private Transform _leftEnd;
    [SerializeField] private Transform _rightEnd;
    [SerializeField] private float _walkSpeedX;
    
    private SpriteRenderer sr;
    private Rigidbody2D _rigidbody;
    
    private enum FacingDir
    {
        Right, Left
    }
    private FacingDir _facingDir;   

    //Lerp stuff
    private float time;
    private float hitColorStatus;
    private float currentColorStatus;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color baseColor;
    [SerializeField] private float durationUntilHitColor;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _facingDir = new FacingDir();
        _facingDir = FacingDir.Right;            
    }
    private void Update() //Regeln von Farbe bei Treffer
    {
        if (hitColorStatus > 0.5f)
        {
            time += Time.deltaTime;
            if (time > durationUntilHitColor)
            {
                hitColorStatus -= 1f;
                time = 0;
            }
        }
        currentColorStatus += (hitColorStatus - currentColorStatus) * 8f * Time.deltaTime;
        sr.color = Color.Lerp(baseColor, hitColor, currentColorStatus);
    }

    void FixedUpdate() //Bewegt Käfer von einem zum Anderen Ende einer Strecke (wechselt Richtung)
    {
        if (_facingDir == FacingDir.Right)
        {
            if (transform.position.x > _rightEnd.position.x) //rightEnd ist ein GameObject, bis zu dessen x-Koordinate der Käfer läuft, bevor er umkehrt
            {
                _facingDir = FacingDir.Left;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                //_spriteRenderer.flipX = true;
            }
            _rigidbody.velocity = new Vector2(_walkSpeedX, _rigidbody.velocity.y); //Setzt Geschwindigkeit
        }
        else
        {
            if (transform.position.x < _leftEnd.position.x)
            {                
                _facingDir = FacingDir.Right;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                //_spriteRenderer.flipX = false;
            }
            _rigidbody.velocity = new Vector2(-_walkSpeedX, _rigidbody.velocity.y);
        }
    }    

    public void Death()
    {
        GameObject.FindWithTag("Player").GetComponent<MyCharacterCtrl>().EnemyHitboxes.Remove(gameObject);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(DeathManager());        
    }

    public IEnumerator DeathManager() //Einfache Tod-Coroutine für Particles
    {
        yield return null;        
        Instantiate(deathParticle, transform.position, transform.rotation); //Erstelle Particle-Prefab
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);

    }

    public void IncrementColorStatus() //Setzt Lerp Ziel => Farbe auf rot bei Treffer
    {
        hitColorStatus += 1f;
    }
}
