using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //使玩家进入滑墙状态
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlide);
        }

        if(player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);

            //防止空中落地后滑行 -- 先注释保持和教程代码一致
            //player.SetVelocity(0, rb.velocity.y);
        }

        //可以让玩家在空中转向
        if(xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);

        }
    }
}
