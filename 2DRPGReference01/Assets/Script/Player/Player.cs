using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    
    [Header("Attack details")]
    public Vector2[] attackMovement;    //可以单独设置攻击动作间的间隔
    public float counterAttackDuration = .2f;
    
    //用来解决玩家状态切换间隙，如primaryattack攻击连段中间进入idle状态使其可以移动
    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 8;
    public float jumpForce;
    public float swordReturnImpact;
    public float wallJumpSpeed = .5f;
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    public float dashDir {  get; private set; }


    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }

    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }

    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }

    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }

    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSword { get; private set; }

    public PlayerCatchSwordState catchSword { get; private set; }

    public PlayerBlackholeState blackhole { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        blackhole = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {

        base.Start();

        fx = GetComponent<PlayerFX>();

        skill = SkillManager.instance;

        //开始时进入idle状态
        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();
        CheckForDashInput();
        
        if(Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("use flask");
            Inventory.instance.UseFlask();
        }

    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke(nameof(ReturnDefaultSpeed), _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }


    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        //在地面贴墙的时候不能冲刺
        if (IsWallDetected())
        {
            return;
        }

        if(skill.dash.dashUnlocked == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            //如果没有输入默认冲刺方向为当前面对的方向
            if(dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
