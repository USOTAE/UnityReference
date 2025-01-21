using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(player.IsGroundDetected())
            player.currentJumpCount = 0;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.groundedAttackState);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);

        if (Input.GetKeyDown(KeyCode.Q))
            stateMachine.ChangeState(player.counterAttackState);

        if (Input.GetKeyDown(KeyCode.Alpha1) && player.skill.powerUp.powerUpUnlocked && player.skill.powerUp.CanUseSkill())
            stateMachine.ChangeState(player.powerUpState);

        if (Input.GetKey(KeyCode.S))
            stateMachine.ChangeState(player.crouchState);

        if (Input.GetKeyDown(KeyCode.Mouse1) && player.skill.sword.swordUnlocked && player.skill.sword.CanUseSkill())
            stateMachine.ChangeState(player.aimSwordState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
