using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private float counterAttackDuration = .5f;

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = counterAttackDuration;
        player.animator.SetBool("CounterAttackSuccess", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if(hit.GetComponent<Controller_Arrow>() != null)
            {
                hit.GetComponent<Controller_Arrow>().FlipArrow();
                SuccessfulCounterAttack();
            }
            
            if (hit.GetComponent<Enemy>() != null)
            {

                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    hit.GetComponent<Enemy>().DamageImpact();   //��ʱ��ӵĻ���Ч�������������ݼ����ɾ��
                    SuccessfulCounterAttack();
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    private void SuccessfulCounterAttack()
    {
        stateTimer = 10;    //�κα�1���ֵ
        player.animator.SetBool("CounterAttackSuccess", true);
    }
}
