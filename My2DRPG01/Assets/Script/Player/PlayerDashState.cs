using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float defaultDashSpeed;
    public float dashSpeed = 25;
    public float dashDuration = 0.2f;
    public float dashDir;


    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        //todo wallSlide

        player.SetVelocity(dashSpeed * dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        //todo dash fx
    }
}
