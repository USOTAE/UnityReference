using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{

    public Enemy_SkeletonIdleState idleState { get; private set; }
    public Enemy_SkeletonMoveState moveState { get; private set; }
    public Enemy_SkeletonBattleState battleState { get; private set; }
    public Enemy_SkeletonAttackState attackState { get; private set; }
    public Enemy_SkeletonStunnedState stunnedState { get; private set; }
    public Enemy_SkeletonDeadState deadState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new Enemy_SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new Enemy_SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new Enemy_SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new Enemy_SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new Enemy_SkeletonDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

}
