using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject für Boss Parameter
[CreateAssetMenu(fileName = "BossParams_1", menuName = "MyParameters/Boss")]
public class BossParams : ScriptableObject
{
    [field: SerializeField, Header("Teleport")] public float TeleportTime { get; private set; }
    [field: SerializeField, Header("Downtime")] public float DowntimeFirstPort {  get; private set; }
    
    [field: SerializeField] public float FlashDuration {  get; private set; }
    [field: SerializeField] public float HandsFadeDuration {  get; private set; }



    //Hilfsgeometrien   
    [field: SerializeField, Header("Upwards Sword Barrage")] public Vector2 USB_HandRotationCenter { get; private set; } //Relative to Boss Position
    //[field: SerializeField] public float USB_SwordUpForce { get; private set; }
    //[field: SerializeField] public float USB_SwordDownImpulse { get; private set; }
    [field: SerializeField] public float USB_BladesActiveTime { get; private set; } //The delay between the sword being thrown and the Barrage appearing
    [field: SerializeField] public Vector2 USB_BladesStartPoint { get; private set; } //y will be used for all of them
    [field: SerializeField] public float USB_BladesSpacing { get; private set; }
    [field: SerializeField] public float USB_BladesDelay { get; private set; }
    [field: SerializeField] public float USB_Downtime { get; private set; }
    [field: TextArea, SerializeField] public string USB_Notes {  get; private set; }

    [field: SerializeField, Header("Side Hover")] public float SH_FullAngle {  get; private set; }
    [field: SerializeField] public float SH_Duration {  get; private set; }
    [field: SerializeField] public float SH_HoverSpeed {  get; private set; }
    [field: SerializeField] public float SH_Radius {  get; private set; }
    [field: SerializeField] public float SH_StartAngle {  get; private set; } //from y-Axis
    [field: SerializeField] public float SH_Downtime { get; private set; }
    [field: SerializeField] public float SH_MovementDelay {  get; private set; }
}
