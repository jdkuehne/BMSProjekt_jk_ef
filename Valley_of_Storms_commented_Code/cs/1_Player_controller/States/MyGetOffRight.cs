using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "MyStates/Character/GetOffR")]
public class MyGetOffRight : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;
    private Vector2 _dir;
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;
        switch (variation)
        {
            case 0: _dir = Vector2.right; break;
            case 1: _dir = Vector2.left; break;
            default: Debug.Log("Wrong variation in parameter"); break;
        }
        _playerRigid.AddForce(_pParams.GetOffImpulse * _dir, ForceMode2D.Impulse);
        _control.animator.SetInteger("State", 0);
    }
    public override void WhileRunning()
    {

    }
    public override void Exit()
    {

    }
    public override void HorizontalMovement()
    {
        
    }
}
