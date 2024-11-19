using StateMachine;
using System.Collections;
using UnityEngine;
using System;
using static PlayerEnums;

[CreateAssetMenu(menuName = "MyStates/Character/Idle")] //erlaubt Erstellung von Objekten dieses Typs im Project-Tab (nur ScriptableObject)
public class MyIdle : MyState<MyCharacterCtrl, int> //Inheritance von abstract class MyState => jede MyState-Methode muss implementiert werden, die Methoden können daher in verschiedenen State-Scripts verschiedene Inhalte
//jedoch den selben Namen haben. Dies erlaubt uns, einem MyState Objekt verschiedenste Methoden mit dem selben Aufruf zu geben
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;

    private float _stateTime;
    private bool _chainAttack;    
        
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        //Zuweisen von wichtigen Objekten im Charaktercontroller
        _chainAttack = false;
        _stateTime = 0;
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        //if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;

        //Animationen setzen
        _control.animator.SetFloat("YSpeed", _playerRigid.velocity.y); //0 => siehe unten, 1 => Rising, -1 => Falling
        _control.animator.SetFloat("Speed", Mathf.Abs(_control.PInputs.HorizontalAxis())); //0 => Idle, -1/1 => Running
        _control.animator.SetInteger("State", 0); //Running/Idle/Falling/Rising abhängig von vorherigen Parametern

        //ChainAttack
        if (variation == 2)
        {            
            _chainAttack = true;
            return;
        }

                   
        _control.CaptureLastDirectionalInput(); //siehe MyCharacterCtrl.cs

        if (_playerRigid.velocity.y < 0 || variation == 1) //möglicher Wall Slide Init, abhängig von Input & variation Argument
        {
            if ((_input.HorizontalAxis() > 0.5f || variation == 1) && _control.pe.terrain == ColState.IsOnRightWall)
            {
                _control.InitWallSlide(0);
            }
            if ((_input.HorizontalAxis() < -0.5f || variation == 1) && _control.pe.terrain == ColState.IsOnLeftWall)
            {
                _control.InitWallSlide(1);
            }
        }
        
        //Diese Inputs sind während dem Idle State möglich bzw. in diese States kann von Idle aus gewechselt werden
        _control.PInputs.JumpPerformed += _control.InitJump;
        _control.PInputs.DashPerformed += _control.InitDash;
        _control.PInputs.GrapplePerformed += _control.InitGrapple;
        _control.PInputs.AttackPerformed += _control.InitAttack;
        _control.PInputs.HealPerformed += _control.InitHeal;
        _control.PInputs.PickUpPerformed += _control.InitPickup;
        _control.PInputs.PrayPerformed += _control.InitPray;        
    }
    public override void WhileRunning()
    {
        if(_stateTime < 0.5f) //Timer falls Input erst ab gewisser Zeit verfügbar sein sollte
        {
            _stateTime += Time.fixedDeltaTime;
        }
        if(_stateTime > 0.1f && _chainAttack) //Wechsel zu Attacke mit leichter Verzögerung
        {
            _control.InitChainAttack();
            _chainAttack = false;
        }        
        
        //Handling von Animator-Parametern, um zwischen den vier Idle-States: Still, Rennend, Aufsteigend (Sprung hat den selben Animationsstate, da Anstieg auch durch Trampolin usw. im Idle möglich wäre), Fallend
        if (_control.pe.terrain == ColState.IsGrounded || _control.pe.terrain == ColState.IsGroundCloseToRight || _control.pe.terrain == ColState.IsGroundCloseToLeft)
        {
            _control.animator.SetBool("Ground", true);
            _control.animator.SetFloat("Speed", Mathf.Abs(_control.PInputs.HorizontalAxis()));
        }
        else
        {
            _control.animator.SetFloat("Speed", 0f);
            _control.animator.SetBool("Ground", false);
            _control.animator.SetFloat("YSpeed", _playerRigid.velocity.y);
        }                
        _control.CaptureLastDirectionalInput(); //Input-Aufnahme, siehe MyCharacterCtrl.cs

        //Wechsel in Wall Slide State, wenn der Spieler fallend ist (y-Geschwindigkeit < 0), der Terrain-Enum auf einer der Wände ist und ein Input in die entsprechende Richtung vorliegt       
        if (_playerRigid.velocity.y < 0)
        {
            if (((_control.LastDirectionalInput > 0.5f && _control.HasStoredHorizontalInput) || _input.HorizontalAxis() > 0.5f) && _control.pe.terrain == ColState.IsOnRightWall)
            {
                _control.InitWallSlide(0); //Argument bestimmt über Richtung
            }
            if (((_control.LastDirectionalInput < -0.5f && _control.HasStoredHorizontalInput) || _input.HorizontalAxis() < -0.5f) && _control.pe.terrain == ColState.IsOnLeftWall)
            {
                _control.InitWallSlide(1);
            }
        }
    }
    public override void Exit()
    {       
        //Inputs wieder deaktiviert
        _input.JumpPerformed -= _control.InitJump;
        _input.DashPerformed -= _control.InitDash;
        _input.GrapplePerformed -= _control.InitGrapple;
        _input.AttackPerformed -= _control.InitAttack;
        _input.HealPerformed -= _control.InitHeal;
        _input.PickUpPerformed -= _control.InitPickup;
        _input.PrayPerformed -= _control.InitPray;        
    }
    public override void HorizontalMovement()
    {
        switch (_control.pe.terrain)
        {
            case ColState.IsGrounded: _playerRigid.velocity = new Vector2(_control.PInputs.HorizontalAxis() * _control.PParams.WalkSpeed, _playerRigid.velocity.y); _control.CanDash = true; break;
            //Dash wird auf dem Boden resetted und der Spieler bewegt sich mit dem WalkSpeed Parameter aus dem ausgewählten PlayerParams-ScriptableObject

            case ColState.IsAirborne:                
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y); //alle anderen Fälle bewegen sich mit leicht verringerter "Luft"-Geschwindigkeit               
                break;

            case ColState.IsOnRightWall:
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);                
                break;
            
            case ColState.IsOnLeftWall:
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);                
                break;
        }        
    } 

    public void Hello() //Test-Methode
    {
        //Debug.Log("Hello");
    }
    
}
