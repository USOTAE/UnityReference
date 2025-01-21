using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ArcherDeadState : EnemyState
{
    private Enemy_Archer enemy;
    public Enemy_ArcherDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        //enemy.animator.SetBool(enemy.lastAnimBoolName, true);
        //enemy.animator.speed = 0;
        //enemy.colliderCapsule.enabled = false;
        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}
