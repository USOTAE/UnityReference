using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if(player.IsWallDetected() == false)
        {
            stateMachine.ChangeState(player.airState);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        if(xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        //控制人物滑墙速度:如果y轴输入为负加快滑墙速度
        if(yInput < 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y * .7f);
        }
        

        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (!player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
