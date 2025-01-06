using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightningDamage
}


public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;   //1 point increase damage by 1 and criy.power by 1%
    public Stat agility;    //1 point increase evasion by 1% crit.chance by 1%
    public Stat intelligence;   //1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality;   //1 point increase health by 3 or 5 point

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;  //default value 150%

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;  //does damage over time
    public bool isChilled;  //reduce armor by 20%
    public bool isShocked;  //reduce accuracy by 20%

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chillTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnurable;

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }

        if (chillTimer < 0)
        {
            isChilled = false;
        }

        if (shockedTimer < 0)
        {
            isShocked = false;
        }

        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }

    public void MakeVulnurableFor(float _duration)
    {
        StartCoroutine(VulnurableForCoroutine(_duration));
    }

    private IEnumerator VulnurableForCoroutine(float _duration)
    {
        isVulnurable = true;
        yield return new WaitForSeconds(_duration);
        isVulnurable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
        {
            return;
        }

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
            //Debug.Log("total crit damage is " + totalDamage);
        }

        fx.CreateHitFx(_targetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicalDamage(_targetStats);  //remove if you don't want to apply magic hit on primary attack
    }

    #region Magical damage and ailment
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
        {
            return;
        }

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }

            if (Random.value < .5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        if (canApplyShock)
        {
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightningDamage * .1f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chillTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                {
                    return;
                }

                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
        {
            return;
        }

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        //find closest target, only among the enemy
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            //如果周围没有最近的敌人，则对正在被攻击的敌人触发电击效果
            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }
        }

        //instatnitate thunder strike
        //setup thunder strike
        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            currentHealth -= igniteDamage;
            if (currentHealth < 0 && !isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
        {
            Die();
        }
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > GetMaxHealthValue())
        {
            currentHealth = GetMaxHealthValue();
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (isVulnurable)
        {
            _damage = Mathf.RoundToInt(_damage * 1.1f);
        }
        currentHealth -= _damage;

        if (_damage > 0)
        {
            fx.CreatePopUpText(_damage.ToString());
        }

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if(!isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }

    #region Stat calculations
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (isChilled)
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20;
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }

    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        //Debug.Log("total crit power %" + totalCritPower);
        float critDamage = _damage * totalCritPower;
        //Debug.Log("crit damage before round up" + critDamage);
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength)
        {
            return strength;
        }
        else if (_statType == StatType.agility)
        {
            return agility;
        }
        else if (_statType == StatType.intelligence)
        {
            return intelligence;
        }
        else if (_statType == StatType.vitality)
        {
            return vitality;
        }
        else if (_statType == StatType.damage)
        {
            return damage;
        }
        else if (_statType == StatType.critChance)
        {
            return critChance;
        }
        else if (_statType == StatType.critPower)
        {
            return critPower;
        }
        else if (_statType == StatType.health)
        {
            return maxHealth;
        }
        else if (_statType == StatType.armor)
        {
            return armor;
        }
        else if (_statType == StatType.evasion)
        {
            return evasion;
        }
        else if (_statType == StatType.magicResistance)
        {
            return magicResistance;
        }
        else if (_statType == StatType.fireDamage)
        {
            return fireDamage;
        }
        else if (_statType == StatType.iceDamage)
        {
            return iceDamage;
        }
        else if (_statType == StatType.lightningDamage)
        {
            return lightningDamage;
        }

        return null;
    }
}
