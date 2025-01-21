using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_ThunderStrike : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoDamage(enemyTarget);
        }
    }
}
