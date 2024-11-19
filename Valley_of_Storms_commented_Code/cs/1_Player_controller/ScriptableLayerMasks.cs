using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//stellt Layer Masken für Casts in DetectCompact.cs zur Verfügung
[CreateAssetMenu(fileName = "ScriptableLayerMenu", menuName = "ScriptableConfigs/MyLayers", order = 1)]
public class ScriptableLayerMasks : ScriptableObject
{
    [field: SerializeField] public LayerMask Ground { get; private set; }
    [field: SerializeField] public LayerMask Enemies { get; private set; }
    [field: SerializeField] public LayerMask Everything { get; private set; }
    [field: SerializeField] public ContactFilter2D GroundFilter { get; private set; }
    [field: SerializeField] public ContactFilter2D LeftWallFilter { get; private set; }
    [field: SerializeField] public ContactFilter2D RightWallFilter { get; private set; }
    [field: SerializeField] public ContactFilter2D HeadbuttFilter { get; private set; }
    [field: SerializeField] public ContactFilter2D GrapplePointFilter { get; private set; }


}
