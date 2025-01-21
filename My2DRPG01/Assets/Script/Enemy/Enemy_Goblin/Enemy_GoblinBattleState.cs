using System.Collections;
using UnityEngine;


public class Enemy_GoblinBattleState : EnemyState
{
    private Enemy_Goblin enemy;

    public Enemy_GoblinBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
}
