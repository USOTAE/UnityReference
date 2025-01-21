using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy
{
    [Header("Archer spisifc info")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] protected float arrowSpeed;

    public Vector2 jumpVelocity;
    public float jumpCooldown;
    public float safeDistance;
    [HideInInspector] public float lastTimeJumped;

    [Header("Additional collision check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    public Enemy_ArcherIdleState idleState {  get; private set; }
    public Enemy_ArcherMoveState moveState { get; private set; }
    public Enemy_ArcherBattleState battleState { get; private set; }
    public Enemy_ArcherAttackState attackState { get; private set; }
    public Enemy_ArcherDeadState deadState { get; private set; }
    public Enemy_ArcherJumpState jumpState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new Enemy_ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new Enemy_ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new Enemy_ArcherAttackState(this, stateMachine, "Attack", this);
        deadState = new Enemy_ArcherDeadState(this, stateMachine, "Dead", this);
        jumpState = new Enemy_ArcherJumpState(this, stateMachine, "Jump", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    //public override bool CanBeStunned()
    //{
    //}

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    public override void AnimationSpecialAttactTrigger()
    {
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);
        newArrow.GetComponent<Controller_Arrow>().SetupArrow(arrowSpeed * facingDir, stats);
    }

    public bool GroundBehindCheck()
    {
        return Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    public bool WallBehindCheck()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}
