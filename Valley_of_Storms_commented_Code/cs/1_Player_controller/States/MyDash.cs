using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyStates/Character/Dash")]
public class MyDash : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;
        _control.CanDash = false;
        _playerRigid.velocity = Vector2.zero;
        _control.animator.SetInteger("State", 5);
    }
    public override void WhileRunning()
    {

    }
    public override void Exit()
    {
        _playerRigid.velocity = Vector2.zero ;
        _control.DashCooldown = true;
    }
    public override void HorizontalMovement()
    {
        _playerRigid.velocity = _control.LastDirectionalInput * _pParams.DashSpeed * Vector2.right;
    }
}
