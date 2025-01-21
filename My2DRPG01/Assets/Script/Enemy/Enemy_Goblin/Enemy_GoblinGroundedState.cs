using System.Collections;
using UnityEngine;


public class Enemy_GoblinGroundedState : EnemyState
{
    private Enemy_Goblin enemy;
    public Enemy_GoblinGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Goblin _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
}
