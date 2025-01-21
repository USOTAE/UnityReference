using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedAttackState : PlayerGroundedState
{
    public int comboCounter;
    private int maxComboCount = 2;
    private float lastTimeAttacked;
    private float comboWindow = 1;

    public PlayerGroundedAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //todo attack audio effect

        xInput = 0; //Cannot modify facingDir while enter the attack state

        if (comboCounter > maxComboCount || Time.time >= lastTimeAttacked + comboWindow) //Reset combo count.
            comboCounter = 0;

        player.animator.SetInteger("ComboCounter", comboCounter);

        float attackDir = player.facingDir;
        if (xInput != 0)    //Set default attack direction.
            attackDir = xInput;

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(player.BusyFor(.15f));
        lastTimeAttacked = Time.time;
    }
}
