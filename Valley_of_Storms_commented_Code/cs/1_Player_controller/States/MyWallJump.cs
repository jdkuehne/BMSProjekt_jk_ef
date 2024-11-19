using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEnums;

[CreateAssetMenu(menuName = "MyStates/Character/Wall Jump")]
public class MyWallJump : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;    
    
    private bool _exited;
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        //Components
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;

        //Inputs
        _input.DashPerformed += _control.InitDash;
        
        //Set Variables
        _exited = false;

        //Anim
        _control.animator.SetFloat("YSpeed", _playerRigid.velocity.y);
        _control.animator.SetInteger("State", 0);

        //Action
        _playerRigid.velocity = Vector2.zero;
        _playerRigid.AddForce(_detect.WallJumpVector * _pParams.WallJumpImpulse, ForceMode2D.Impulse);
    }
    public override void WhileRunning()
    {
        if (_input.JumpAcceleration() < 0.5f && !_exited)
        {
            _playerRigid.velocity = new Vector2(_playerRigid.velocity.x, _pParams.JumpStopFactor * _playerRigid.velocity.y);
            _control.LeaveStateToIdle();
            _exited = true;
        }
        else if (_playerRigid.velocity.y <= 0f && !_exited)
        {
            _control.LeaveStateToIdle();
            _exited = true;
        }

        //Anim
        _control.animator.SetFloat("YSpeed", _playerRigid.velocity.y);
    }
    public override void Exit()
    {
        _control.StoreInputX();

        //Inputs
        _input.DashPerformed -= _control.InitDash;        
    }
    public override void HorizontalMovement() { }
}
