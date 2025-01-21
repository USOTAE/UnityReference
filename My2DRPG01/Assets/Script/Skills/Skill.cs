using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    public float cooldownTimer;

    [Header("Consume mana")]
    [SerializeField] private ItemData consumeItem;
    [SerializeField] private int consumeAmount; 

    protected Player player;

    [Header("Buff effect")]
    public BuffEffects[] buffEffects;


    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        CheckUnlock();
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    protected virtual void CheckUnlock()
    {

    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer <= 0 && InventoryManager.instance.GetStashItemAmount(consumeItem) >= consumeAmount)
        {
            InventoryManager.instance.RemoveItem(consumeItem,consumeAmount);
            UseSkill();
            return true;
        }

        //todo create cooldown fx
        return false;
    }

    public virtual void UseSkill()
    {
        //do some skill specific things
        cooldownTimer = cooldown;
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }

    public void Effect(Transform _enemyPosition)
    {
        foreach (var buff in buffEffects)
        {
            buff.ExecuteEffect(_enemyPosition);
        }
    }
}
