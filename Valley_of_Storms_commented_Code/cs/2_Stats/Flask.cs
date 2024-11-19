using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//Stat vergleichbar mit HealthV2.cs, der aber stattdessen die Anzahl Heilflaschen herausgibt und kontrolliert
public class Flask : MonoBehaviour
{
    [SerializeField] private int _startMaxFlasks;
    private int _maxFlasks;
    private int _flasks;
    
    public int Flasks
    {
        get => _flasks;
        set
        {
            _flasks = Mathf.Clamp(value, 0, _maxFlasks);
            OnFlaskUse?.Invoke(_flasks); //Auslösen eines Event für UI-Update
        }
    }

    public int MaxFlasks
    {
        get => _maxFlasks;
        set
        {
            _maxFlasks = value;
            Flasks = Mathf.Clamp(Flasks, 0, _maxFlasks);

        }
    }

    public event Action<int> OnFlaskUse;

    private void Start()
    {
        _maxFlasks = _startMaxFlasks;
        _flasks = _maxFlasks;
        GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetFlask(_flasks);
        OnFlaskUse += GameObject.FindWithTag("UIWhilePlay").GetComponent<RuntimeUIManager>().SetFlask;
    }

    public void FillFlasks() => Flasks = MaxFlasks;
    public void Increment() => Flasks++;
    public void Decrement() => Flasks--;
    public void IncreaseMax(int value) => MaxFlasks += value;
    public void DecreaseMax(int value) => MaxFlasks -= value;



}
