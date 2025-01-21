using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool isBusy {  get; private set; }
    [Header("Player Resource")]
    public int currency;
    public int cheatCoin;   //todo:增加一种特殊货币，player每次死亡时增加100，可以在特殊商店购买一些非常强力的道具帮助玩家闯关

    [Header("Move info")]
    public float moveSpeed = 8;
    public float jumpForce;
    public int currentJumpCount = 0;
    public int maxJumpCount = 2;
    public float wallJumpForce;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Attack details")]
    public Vector2[] attackMovement;

    [Header("Ground check info")]
    [SerializeField] protected Transform groundOverheadCheck;
    [SerializeField] protected float groundOverheadCheckDistance;

    private CapsuleCollider2D capsuleCollider;
    [HideInInspector]public float defaultColliderXSize = 0.8172321f;
    [HideInInspector]public float defaultColliderYSize = 1.903423f;
    [HideInInspector]public float defaultColliderXOffset = -2.38418579e-07f;
    [HideInInspector] public float defaultColliderYOffset = -0.0379298925f;

    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }


    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState {  get; private set; }
    public PlayerMoveState moveState {  get; private set; }
    public PlayerGroundedAttackState groundedAttackState {  get; private set; }
    public PlayerJumpState jumpState {  get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerPowerUpState powerUpState { get; private set; }
    public PlayerCrouchState crouchState { get; private set; }
    public PlayerRollState rollState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        groundedAttackState = new PlayerGroundedAttackState(this, stateMachine, "GroundedAttack");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        powerUpState = new PlayerPowerUpState(this, stateMachine, "PowerUp");
        crouchState = new PlayerCrouchState(this, stateMachine, "Crouch");
        rollState = new PlayerRollState(this, stateMachine, "Roll");
        deadState = new PlayerDeadState(this, stateMachine, "Dead");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;
        fx = GetComponent<PlayerFX>();

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;

        
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.Alpha2) && skill.lightsaber.lightsaberUnlocked)
            skill.lightsaber.CanUseSkill();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    //使用动画事件来增加连击动作的计数
    //todo 空中攻击的计数
    public void AttackCountAnimationTrigger()
    {
        groundedAttackState.comboCounter++;
    }

    private void CheckForDashInput()
    {
        if (skill.dash.dashUnlocked == false)
            return;

        if(Input.GetKeyDown(KeyCode.LeftShift) && skill.dash.CanUseSkill())
        {
            dashState.dashDir = Input.GetAxisRaw("Horizontal");
            if (dashState.dashDir == 0)
                dashState.dashDir = facingDir;

            if (stateMachine.currentState != rollState)
                stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public bool IsGroundOverheadDetected()
    {
        return Physics2D.Raycast(groundOverheadCheck.position, Vector2.up, groundOverheadCheckDistance, whatIsGround);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(groundOverheadCheck.position, new Vector3(groundOverheadCheck.position.x, groundOverheadCheck.position.y + groundOverheadCheckDistance));
    }

    public void ModifyCollider(float _xOffset, float _yOffset, float _xSize, float _ySize)
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.offset = new Vector2(_xOffset, _yOffset);
            capsuleCollider.size = new Vector2(_xSize, _ySize);
        }
    }

    public void  AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public bool HasEnoughCurrency(int _price)
    {
        if(currency >= _price)
        {
            currency-=_price;
            return true;
        }
        Debug.Log("Not enough currency");
        return false;
    }

    public int GetCurrency() 
    { 
        return currency; 
    }
}
