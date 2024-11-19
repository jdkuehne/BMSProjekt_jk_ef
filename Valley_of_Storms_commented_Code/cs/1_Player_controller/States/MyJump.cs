using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static PlayerEnums;

[CreateAssetMenu(menuName = "MyStates/Character/Jump")]
public class MyJump : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;
    //[SerializeField] private float _velocityReduction;
    private float _reductionNow;
    private float _jumptime;
    private bool _variationJump;
    private Vector2 _wallJumpVector;
    private float _currentJumpVelocity;

    private bool _attackenabled;    
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int variation)
    {
        _playerRigid = rigid;
        _control = charactercontroller;
        if(_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if(_pEnums == null) _pEnums = _control.pe;
        if(_pParams == null) _pParams = _control.PParams;
        //_input.DashPerformed += _control.InterruptJumpAccelerateByDash;
        _input.DashPerformed += _control.InitDash;
        
        _jumptime = 0;
        _attackenabled = false;
        
        //_control.IsJumping = true;
        
        _control.animator.SetFloat("YSpeed", 1); //Makes sure start with rising
        _control.animator.SetBool("Ground", false); 
        
        _playerRigid.AddForce(Vector2.up * _pParams.JumpImpulse, ForceMode2D.Impulse);
    }
    public override void WhileRunning()
    {        
        _jumptime += Time.fixedDeltaTime;        
        if (_jumptime > 0.05f && _attackenabled == false)
        {
            _control.PInputs.AttackPerformed += _control.InitAttack;
            _attackenabled = true;
        }        

        _control.CaptureLastDirectionalInput();        
        //if (_input.JumpAcceleration() < 0.5f && !_exited)     => Handler
        //{
        //    _playerRigid.velocity = new Vector2(_playerRigid.velocity.x, _pParams.JumpStopFactor * _playerRigid.velocity.y);
        //    _control.IsJumping = false;
        //    _exited = true;            
        //    _control.LeaveStateToIdle();            
        //}
        //else if(_playerRigid.velocity.y <= 0f && !_exited)
        //{
        //    _control.IsJumping = false;
        //    _exited = true;            
        //    _control.LeaveStateToIdle();            
        //}        
    }
    public override void Exit()
    {
        //Inputs
        _input.DashPerformed -= _control.InitDash;
        _input.AttackPerformed -= _control.InitAttack;
        
        //Anim
        _control.animator.SetFloat("YSpeed", 0f); //Check exact logic => transistions      
    }

    public override void HorizontalMovement()
    {
        
        switch (_pEnums.terrain)
        {
            case ColState.IsGrounded: _playerRigid.velocity = new Vector2(_control.PInputs.HorizontalAxis() * _control.PParams.WalkSpeed, _playerRigid.velocity.y); break;  //dependency on angle?

            case ColState.IsAirborne:                
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);
                if (_input.HorizontalAxis() > 0.5f || _input.HorizontalAxis() < -0.5f) { _control.StoreInputX(); }
                break;            

            case ColState.IsOnLeftWall: //_playerRigid.velocity = new Vector2(
                //Mathf.Clamp(_input.HorizontalAxis(), _detect.WallNormalPerpendicular.x / _detect.WallNormalPerpendicular.y, 1f) * _currentJumpVelocity, _playerRigid.velocity.y);
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);
                if (_input.HorizontalAxis() > 0.5f || _input.HorizontalAxis() < -0.5f) { _control.StoreInputX(); } break; //still sus

            case ColState.IsOnRightWall: //_playerRigid.velocity = new Vector2(
                //Mathf.Clamp(_input.HorizontalAxis(), -1f, _detect.WallNormalPerpendicular.x / _detect.WallNormalPerpendicular.y) * _currentJumpVelocity, _playerRigid.velocity.y);
                _playerRigid.velocity = new Vector2(_input.HorizontalAxis() * _pParams.AirVelocity, _playerRigid.velocity.y);
                if (_input.HorizontalAxis() > 0.5f || _input.HorizontalAxis() < -0.5f) { _control.StoreInputX(); } break;
        }        
    }
}
