using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ArcherJumpState : EnemyState
{
    private Enemy_Archer enemy;
    private float jumpDuration = .2f;
    public Enemy_ArcherJumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = jumpDuration;  
    }

    public override void Exit()
    {
        base.Exit();

        enemy.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, rb.velocity.y);

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
