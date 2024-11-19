using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parameter für Feuergolems, ermöglicht unterschiedliche zeitliche Abstände zwischen "Speien"
[CreateAssetMenu(fileName = "ShotParameter", menuName = "MyParameters/FireGolem")]
public class ShootParameters : ScriptableObject
{   
    [field: SerializeField] public float Period { get; private set; }
    [field: SerializeField] public Vector2 Speed { get; private set; } //Geschwindigkeit des Feuerballs
}
