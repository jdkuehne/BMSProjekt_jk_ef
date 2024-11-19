using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Health-Stat mit Unity Events usw. Grundlagen aus Video: https://youtu.be/oZ2GWWjL4Fo
public class HealthV2 : MonoBehaviour //Inheritance von Monobehaviour macht Start() und Update() verfügbar und ermöglicht Anhängen an GameObject
{
    [SerializeField] private int _startMaxHp;
    private int _maxHp;
    private int _hp;
    private bool _isTrap;

    public int MaxHp //Property für setzen maximaler Lebenspunkte
    {
        get => _maxHp;
        set
        {
            _maxHp = value; //
            _hp = Mathf.Clamp(_hp, 0, _maxHp); //hält hp im Rahmen der maxHp => sinkt max sinken hp mit
            MaxHpChanged.Invoke(_maxHp); //Event um UI zu setzen
            GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetHp(_hp); //Füllung des Lebensbalken wird für den Fall wo Hp mitgesunken ist geupdated
        }
    }
    public int Hp
    {
        get => _hp;
        //private damit der Wert nur durch mit public-Methoden gesetzt werden kann, wie Damage(), Heal()...
        private set
        {
            var isDamage = value < _hp; //bool = true, wenn der neue Wert kleiner als der alte ist => Schaden
            _hp = Mathf.Clamp(value, 0, _maxHp); //Hp werden auf Max bzw. 0 begrenzt

            if (isDamage) //falls Schaden
            {
                if (_isTrap) //wenn durch Falle
                {
                    HitByTrap?.Invoke(_hp); //Fallen-Event, löst Positionswechsel auf Laterne oder Schrein aus
                    _isTrap = false; //reset des Fallen flag
                }
                else
                {
                    Damaged?.Invoke(_hp); //Schaden-Event, löst UI-Update, Particle und IFrames (OnPlayerDamaged in MyCharacterCtrl.cs) aus
                }
                
            }
            else
            {
                Healed?.Invoke(_hp); //Heilungs-Event, löst UI-Update aus
            }          
            

            if(_hp <= 0)
            {
                if (isPlayer)
                {
                    GetComponent<MyCharacterCtrl>().IsDead = true; //während IsDead = true werden keine Treffer "registriert"
                    GetComponent<MyCharacterCtrl>().EnemyHitboxes.Clear(); //wegen reload der Scene => alle Gegner werden zerstört
                }
                Died?.Invoke(); //Checkpoint Laden, Heilflaschen-Reset etc.
            }
        }

    }
    //**************************************************************************************************************Events
    public UnityEvent<int> Healed; // Unity Events
    public UnityEvent<int> Damaged;
    public UnityEvent<int> HitByTrap;
    public UnityEvent Died;    
    public event Action<int> MaxHpChanged; //c# event, nicht im Editor sichtbar, dafür kann dafür einfacher während Runtime neue Subscriptions erhalten
    //*****************************************************************************************************************TalismanSolutions
    public List<NamedAction<int>> TalismanEverySecond = new List<NamedAction<int>>(); //Talisman Effekte, die Pro Sekunde geschehen, können in diese Liste geladen werden
    //Um Funktionen als Variable behandeln zu können, wird die Action delegate class genutzt, eigene Wrapper-class (NamedAction) um per Name nach genauer Action suchen zu können bei ablegen des Talisman
    //******************************************************************************************************************************Variables
    public bool isPlayer; //gewisse Dinge müssen bei Gegnern nicht gemacht werden, universelles Script

    private void Start()
    {
        _maxHp = _startMaxHp;
        _hp = _maxHp;
        if (isPlayer)
        {                     
            GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetMaxHealth(MaxHp); //Setze Grundewerte des UI 
            GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetHp(Hp);
            MaxHpChanged += GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetMaxHealth; 
            StartCoroutine(DoEverySecond()); //Starte Coroutine, die in 1 Sekunden Rhythmus Befehle ausführt
        }
        
    }
    private IEnumerator DoEverySecond()
    {
        while (true)
        {
            foreach (var action in TalismanEverySecond) //für jeden TalismanEffekt wird Inhalt ausgeführt, wie Heilung beim PraiseTheSun-Talisman
            {
                action.Method(MaxHp);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    //Methoden für public Zugriff auf Hp, MaxHp
    public void Damage(int amount, bool trap) { _isTrap = trap; Hp -= amount; }
   

    public void Heal(int amount) => Hp += amount;
    

    //public void TrapDamage(int percentage) => Hp -= (percentage * _maxHp);        


    public void HealFull() => Hp = MaxHp;
    

    public void Kill() => Hp = 0;
    

    public void Adjust(int value) => Hp = value;

    public void IncreaseMax(int value) => MaxHp += value;
    public void DecreaseMax(int value) => MaxHp -= value;
    

}

