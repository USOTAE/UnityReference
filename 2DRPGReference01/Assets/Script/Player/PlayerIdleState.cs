using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //进入idle状态时设置速度为0,防止落地滑行
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //靠墙的时候再向面对的方向输入不再进入跑动状态--可加可不加;加上先保持和教程代码一致
        if (xInput == player.facingDir && player.IsWallDetected())
        {
            return;
        }

        if (xInput != 0 && !player.isBusy)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
