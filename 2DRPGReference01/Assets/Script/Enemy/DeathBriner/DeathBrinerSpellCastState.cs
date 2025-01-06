using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBrinerSpellCastState : EnemyState
{
    private Enemy_DeathBriner enemy;
    private int amountOfSpells;
    private float spellTimer;

    public DeathBrinerSpellCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBriner _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        amountOfSpells = enemy.amountOfSpells;
        spellTimer = .5f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeCast = Time.time;
    }

    public override void Update()
    {
        base.Update();

        spellTimer -= Time.deltaTime;

        if(CanCast())
            enemy.CastSpell();

        if (amountOfSpells <= 0)
            stateMachine.ChangeState(enemy.teleportState);
    }

    private bool CanCast()
    {
        if (amountOfSpells > 0 && spellTimer < 0)
        {
            amountOfSpells--;
            spellTimer = enemy.spellCooldown;
            return true;
        }
        return false;
    }
}
