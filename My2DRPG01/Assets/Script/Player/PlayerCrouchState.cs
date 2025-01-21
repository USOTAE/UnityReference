using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.StartCoroutine(WaitSetZeroVelocity());
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

        if (Input.GetKeyUp(KeyCode.S) && !player.IsGroundOverheadDetected())
            stateMachine.ChangeState(player.idleState);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            stateMachine.ChangeState(player.rollState);
    }

    private IEnumerator WaitSetZeroVelocity()
    {
        yield return null;
        player.SetZeroVelocity();
    }

}
