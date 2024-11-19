using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerEnums;

[CreateAssetMenu(menuName = "MyStates/Character/Attack")]
public class MyAttack : MyState<MyCharacterCtrl, int>
{
    private float _stateTime;
    private bool _left;
    private bool _chained;
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;

    [SerializeField] float timeTillIdle;
    [SerializeField] float timeTillExit;

    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _stateTime = 0;
        _left = false;
        _chained = false;
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;

        _control.Sword.GetComponent<Sword>().Parried = false;

        switch (variation)
        {
            case 0: _control.animator.SetInteger("State", 2); break;
            case 1: _control.animator.SetInteger("State", 3); _control.IsPogo = true; break;
            case 2: _control.animator.SetInteger("State", 6); break;
        }
        

        _input.AttackPerformed += ChainAttack;
    }
    public override void WhileRunning()
    {
        _stateTime += Time.fixedDeltaTime;
        if(_stateTime > timeTillIdle)
        {
            _control.animator.SetInteger("State", 0);
        }
        if (_stateTime > timeTillExit && !_left)
        {
            _left = true;
            if (_chained)
            {
                _control.LeaveAttackChainToIdle();
                
            }
            else
            {
                _control.LeaveStateToIdle();
            }
            
        }
        if (!_left)
        {
            _playerRigid.velocity = new Vector2(0f, _playerRigid.velocity.y);
        }

        if (_pEnums.terrain == ColState.IsGrounded || _pEnums.terrain == ColState.IsGroundCloseToRight || _pEnums.terrain == ColState.IsGroundCloseToLeft)
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
    }
    public override void Exit()
    {
        //Debug.Log(_stateTime);
        _input.AttackPerformed -= ChainAttack;
        //_control.Sword.GetComponent<Collider2D>().enabled = false;
        _control.IsPogo = false;        
        _control.Sword.GetComponent<Sword>().EnemiesHitThisAttack.Clear();        
        _control.Sword.GetComponent<Sword>().alreadyPogoed = false;
        _control.Sword.GetComponent<Sword>().Parried = false;
        _control.animator.SetInteger("State", 0);
    }
    public override void HorizontalMovement()
    {
        if (_pEnums.terrain == ColState.IsAirborne)
        {
            _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);
            if (_input.HorizontalAxis() > 0.5f || _input.HorizontalAxis() < -0.5f) { /*_control.HorizontalInputStorage();*/ _control.CaptureLastDirectionalInput(); }
        }
        
    }

    public void ChainAttack(InputAction.CallbackContext context)
    {
        _chained = true;
    }
}
