using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(menuName = "MyStates/Character/Ragdoll")]
public class MyRagdoll : MyState<MyCharacterCtrl, int>
{
    private Rigidbody2D _playerRigid;
    private MyCharacterCtrl _control;
    private DetectCompact _detect;
    private PlayerEnums _pEnums;
    private InputSystem _input;
    private PlayerParams _pParams;
    private SpriteRenderer _spriteRenderer;
    public override void Init(Rigidbody2D rigid, MyCharacterCtrl charactercontroller, int damage)
    {
        _playerRigid = rigid;
        _control = charactercontroller;
        if (_detect == null) _detect = _control.PDetect;
        _input ??= _control.PInputs;
        if (_pEnums == null) _pEnums = _control.pe;
        if (_pParams == null) _pParams = _control.PParams;
        _control.CanDash = false;
        _playerRigid.velocity = Vector2.zero;
        Vector2 knockback = new (0, 0);
        switch((_control.transform.position - _control.EnemyHitboxes[0].transform.position).x)
        {
            case < 0: knockback = new Vector2(-1, 0.7f); break;
            case >= 0: knockback = new Vector2(1, 0.7f); break;
        }
        _playerRigid.AddForce(knockback * _pParams.KnockbackImpulse, ForceMode2D.Impulse);
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
