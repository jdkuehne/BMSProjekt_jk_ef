using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEnums;

[CreateAssetMenu(menuName = "MyStates/Character/WallSlide")]

public class MyWallSlide : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;

    private int _side;
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;
        _side = variation;
        switch (_side)
        {
            case 1: _control.PInputs.GetOffRightPerformed += _control.InitGetOffRight; break;
            case 0: _control.PInputs.GetOffLeftPerformed += _control.InitGetOffLeft; break;
        }
        _control.PInputs.JumpPerformed += _control.InitJump;

        _control.animator.SetInteger("State", 1);
        _control._renderer.flipX = true;
    }
    public override void WhileRunning()
    {
        
        _playerRigid.velocity = new Vector2(_playerRigid.velocity.x, Mathf.Clamp(_playerRigid.velocity.y, -_pParams.SlideSpeed, float.MaxValue));
        _control.CanDash = true;


        if(_pEnums.terrain != ColState.IsOnLeftWall && _pEnums.terrain != ColState.IsOnRightWall) 
        {
            _control.InitIdle(0);
        }
    }
    public override void Exit()
    {
        switch (_side)
        {
            case 1: _control.PInputs.GetOffRightPerformed -= _control.InitGetOffRight; break;
            case 0: _control.PInputs.GetOffLeftPerformed -= _control.InitGetOffLeft; break;
        }
        _control._renderer.flipX = false;
    }
    public override void HorizontalMovement()
    {
        switch (_side)
        {
            case 1: _control.LastDirectionalInput = 1f; _playerRigid.AddForce(_detect.SlideVector * _pParams.SlideForce, ForceMode2D.Force); break;
            case 0: _control.LastDirectionalInput = -1f; _playerRigid.AddForce(_detect.SlideVector * _pParams.SlideForce, ForceMode2D.Force); break;
        }

    }
}
