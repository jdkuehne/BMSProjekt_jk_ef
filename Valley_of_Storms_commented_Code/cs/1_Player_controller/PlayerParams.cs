using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Diese ScriptableObject-Klasse ermöglicht das Erstellen eines Sets von Spielerparametern durch => Rechtsklick in Project-Tab -> Create -> MyParameters -> Player
[CreateAssetMenu(fileName = "Player Params", menuName = "MyParameters/Player")]
public class PlayerParams : ScriptableObject
{      
    [field: SerializeField, Header("Jump")] public float JumpImpulse { get; private set; }
    [field: SerializeField] public float JumpStopFactor { get; private set; }
    //[field: SerializeField] public float JumpImpulse { get; private set; }
    //[field: SerializeField] public float JumpDuration { get; private set; }
    //[field: SerializeField] public float JumpVelocityReduction { get; private set; }
    //[field: SerializeField] public float JumpBrakeFactor { get; private set; }
    //[field: SerializeField] public float JumpBrakeTimeFactor { get; private set; }
    //[field: SerializeField] public float JumpBrakeOffset {  get; private set; }
    [field: SerializeField, Header("Wall Jump")] public float WallJumpDuration { get; private set; }
    [field: SerializeField] public float WallJumpImpulse { get; private set; }
    [field: SerializeField] public float WallJumpAngle { get; private set; }

    [field: SerializeField, Header("Walk")] public float WalkSpeed { get; private set; }    
    [field: SerializeField, Header("Airborne Movement")] public float AirMoveForce { get; private set; }
    [field: SerializeField] public float AirVelocity { get; private set; }
    [field: SerializeField] public float SlideAngle { get; private set; }
    [field: SerializeField] public float SlideForce {  get; private set; }
    [field: SerializeField] public float SlideSpeed { get; private set; }
    [field: SerializeField, Header("Dash")] public float DashSpeed {  get; private set; }
    [field: SerializeField] public float DashDuration {  get; private set; }
    [field: SerializeField, Header("Get Off")] public float GetOffImpulse {  get; private set; }
    [field: SerializeField, Header("Grapple-Hook")] public Vector2 GrappleCastDimensions { get; private set; }
    [field: SerializeField] public float GrappleRange { get; private set; }
    [field: SerializeField] public float GrappleVelocity { get; private set; }
    [field: SerializeField, Header("Knockback and IFrames")] public float KnockbackImpulse {  get; private set; }
    [field: SerializeField] public float KnockbackDuration {  get; private set; }
    [field: SerializeField] public float IFrameTime {  get; private set; } //The Knockbackduration must be added to this param to get full invincibility-time after getting hit
    [field: SerializeField, Header("Healing related")] public float HealDuration {  get; private set; }
    [field: SerializeField] public float HealAmount { get; private set; }
    [field: SerializeField, Header("Pogo")] public float PogoImpulse {  get; private set; }
    [field: SerializeField, Header("Prayer")] public float PrayDuration {  get; private set; }
}
