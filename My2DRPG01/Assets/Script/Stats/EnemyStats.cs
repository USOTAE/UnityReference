using System;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop itemDropSystem;
    public Stats currencyDropAmount;

    [Header("Level details")]
    [SerializeField] private int level; //根据敌人等级修正敌人属性

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier;


    protected override void Start()
    {
        //currencyDropAmount.SetDefaultValue(100);
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        itemDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(attack);
        Modify(pdef);
        Modify(mdef);
        Modify(maxHp);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightningDamage);

        Modify(currencyDropAmount);
    }

    private void Modify(Stats _stats)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stats.GetValue() * percentageModifier;

            _stats.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void DecreaseHpBy(int _damage)
    {
        base.DecreaseHpBy(_damage);

        if (_damage > 0)
            fx.CreatePopupText(_damage.ToString());
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();

        itemDropSystem.GenerateDropList();
        PlayerManager.instance.player.currency += currencyDropAmount.GetValue();
        Destroy(gameObject, 2f);
    }
}
