using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Stat vergleichbar mit HealthV2.cs, der aber stattdessen den Schaden herausgibt
public class Damage : MonoBehaviour, IStat<float>
{
    [SerializeField] private int _basevalue;    
    private int _current;

    public List<NamedFunc<float, float>> Modifiers = new List<NamedFunc<float, float>>(); //Liste von Schadensberechnungen die von Talismanen hinzugefügt werden können, wie mehr Schaden unter Schwelle Hp

    private void Start()
    {
        _current = _basevalue;
    }

    public int Value
    {
        get
        {
            float calcDamage = _current;
            foreach(var func in Modifiers)
            {                
                calcDamage = func.Method(calcDamage); //Ausführen von zusätzlichen Berechnungen
            }
            return (int)calcDamage;
        }
        set
        {
            _current = value;
        }
    }    

    public void Increase(float amount) 
    {
        Value = (int)((float)Value * amount);
    }
    public void Decrease(float amount) 
    {
        Value = (int)((float)Value / amount);
    }
    public void StatReset() => Value = _basevalue;

}

