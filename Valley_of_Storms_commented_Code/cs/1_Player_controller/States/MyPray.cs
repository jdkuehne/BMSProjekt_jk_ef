using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyStates/Character/Pray")]
public class MyPray : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;

    private float _stateTime;
    private bool _left;

    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _stateTime = 0;
        _left = false;
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;
        _control.animator.SetFloat("Speed", 0f);
        _playerRigid.velocity = new Vector2(0, _playerRigid.velocity.y);
        _control.animator.SetInteger("State", 4);


    }
    public override void WhileRunning()
    {
        _stateTime += Time.fixedDeltaTime;
        if (_stateTime >= _pParams.PrayDuration && !_left)
        {
            _left = true;
            _control.LeaveStateToIdle();
        }
    }
    public override void Exit()
    {
        _control.OnPray.Invoke();
    }
    public override void HorizontalMovement()
    {

    }
}
