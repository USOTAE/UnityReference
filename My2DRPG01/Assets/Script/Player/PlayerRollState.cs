using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerState
{
    private float rollSpeed = 10;
    private float rollDir;

    public PlayerRollState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rollDir = Input.GetAxisRaw("Horizontal");
        if (rollDir == 0)
            rollDir = player.facingDir;
        player.ModifyCollider(player.defaultColliderXOffset, -0.5061231f, player.defaultColliderXSize, 0.9637442f);
    }

    public override void Exit()
    {
        base.Exit();
        player.ModifyCollider(player.defaultColliderXOffset, player.defaultColliderYOffset, player.defaultColliderXSize, player.defaultColliderYSize);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(rollSpeed * rollDir, 0);

        if (triggerCalled)
        {
            if (player.IsGroundOverheadDetected())
                stateMachine.ChangeState(player.crouchState);
            else
                stateMachine.ChangeState(player.idleState);
        }

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);
    }
}
