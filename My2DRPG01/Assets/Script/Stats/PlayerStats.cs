using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    private const int preAddCheatCoinCount = 100;
    
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void DecreaseHpBy(int _damage)
    {
        base.DecreaseHpBy(_damage);

        //todo 添加震屏效果
        //todo 添加音效
        if (_damage > GetMaxHp() * .3f)
        {
            Debug.Log("High damage taken");
            player.SetupKnockbackPower(new Vector2(10, 6));
            //player.fx.ScreenShake(player.fx.shakeHighDamage);
            //int randomSound = Random.Range(6, 7);
            //AudioManager.instance.PlaySFX(randomSound, null);
        }

        //todo 可以在此添加装备效果
    }

    protected override void Die()
    {
        base.Die();
        player.Die();

        player.cheatCoin += preAddCheatCoinCount;
    }

}
