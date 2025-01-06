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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackHoleUnlocked)
        {
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("Cooldown");
                return;
            }
            stateMachine.ChangeState(player.blackhole);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword.swordUnlocked)
        {
            stateMachine.ChangeState(player.aimSword);
        }

        if(Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked)
        {
            stateMachine.ChangeState(player.counterAttack);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))    //if(Input.GetKey(KeyCode.Mouse0)):可以按住鼠标持续攻击
        {
            stateMachine.ChangeState(player.primaryAttack);
        }

        //使空中冲刺能衔接下落的动画
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
