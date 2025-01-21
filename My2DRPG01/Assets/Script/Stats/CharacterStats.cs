using System.Collections;
using UnityEngine;


public enum StatsType
{
    attack,
    pdef,
    mdef,
    hp,

    damage,
    critChance,
    critPower,

    fireDamage,
    iceDamage,
    lightningDamage
    //windDamage
}


public class CharacterStats : MonoBehaviour
{
    protected EntityFX fx;

    [Header("Major stats")]
    public Stats attack;    //��������ÿ10�㹥������1���˺�
    public Stats pdef;  //���������ÿ10�����һ���˺�
    public Stats mdef;  //ħ��������ÿ10�����һ���˺�
    public Stats maxHp;
    public int currentHp;
    public System.Action onHpChanged;

    [Header("Offensive stats")]
    public Stats damage;
    public Stats critChance;    //������
    public Stats critPower; //�����˺�,����Ϊ150������1.5���˺�

    [Header("Magic stats")]
    public Stats fireDamage;
    public Stats iceDamage;
    public Stats lightningDamage;
    //public Stats windDamage;  //todo ������������

    public bool isIgnited;  //����
    public float ignitedCoefficient = .05f; //ȼ����Ч�˺���ϵ��
    public bool isChilled;  //����
    public bool isShocked;  //�е�
    public float shockedCoefficient = .01f; //�����Ч�˺���ϵ��
    //public bool isCutted;   //����

    [SerializeField] private float ailmentsDuration = 4;    //�쳣״̬����ʱ��
    private float ignitedTimer;
    private float chillTimer;
    private float shockedTimer;
    private float cuttedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    private int shockDamage;
    [SerializeField] private GameObject shockStrikePrefab;

    public bool isInvincible { get; private set; }  //�޵�
    private bool isVulnurable;  //����
    private float vulnurableCoefficient = 1.1f;
    public bool isDead { get; private set; }

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);
        currentHp = GetMaxHp();
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chillTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chillTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
        {
            ApplyIgniteDamage();
        }
    }

    public int GetMaxHp()
    {
        return maxHp.GetValue();
    }

    public void MakeInvincible(bool _invincible)
    {
        isInvincible = _invincible;
    }

    //��������״̬
    public void MakeVulnurable(float _duration)
    {
        StartCoroutine(VulnurableForCoroutine(_duration));
    }

    private IEnumerator VulnurableForCoroutine(float _duration)
    {
        isVulnurable = true;
        yield return new WaitForSeconds(_duration);
        isVulnurable = false;
    }

    //�ı�Stats����-ͨ������buff��״̬���޸�
    public void ChangeStatsBy(int _modifier, float _duration, Stats _statsToModifier)
    {
        StartCoroutine(ChangeStatsForCoroutine(_modifier, _duration, _statsToModifier));
    }

    private IEnumerator ChangeStatsForCoroutine(int _modifier, float _duration, Stats _statToModifier)
    {
        _statToModifier.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModifier.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        //todo big damage��ɵ�����Ч��

        if (_targetStats.isInvincible)
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalPhysicalDamage = CalculatePhysicalDamage(_targetStats);

        int totalMagicalDamage = CalculateMagicalDamage(_targetStats);

        int totalDamage = Mathf.RoundToInt(totalPhysicalDamage * .7f + totalMagicalDamage * .3f);

        _targetStats.TakeDamage(totalDamage);
    }

    public virtual int CalculatePhysicalDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;    //�Ƿ�ʵ����������Ч���ж�
        int totalPhysicalDamage = Mathf.RoundToInt(damage.GetValue() + attack.GetValue() * .1f);

        if (CanCrit())
        {
            totalPhysicalDamage = CalculateCriticalDamage(totalPhysicalDamage);
            criticalStrike = true;
        }

        //todo crit fx

        totalPhysicalDamage = CheckTargetPhysicalDefense(_targetStats, totalPhysicalDamage);

        return totalPhysicalDamage;
    }

    protected bool CanCrit()
    {
        if (Random.Range(0, 100) <= critChance.GetValue())
            return true;

        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = critPower.GetValue() * .01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    protected int CheckTargetPhysicalDefense(CharacterStats _targetStats, int _totalPhysicalDamage)
    {
        _totalPhysicalDamage -= Mathf.RoundToInt(Mathf.Pow(_targetStats.pdef.GetValue(), .5f));
        _totalPhysicalDamage = Mathf.Clamp(_totalPhysicalDamage, 0, int.MaxValue);
        return _totalPhysicalDamage;
    }

    public virtual int CalculateMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = Mathf.RoundToInt(fireDamage.GetValue() * .1f);
        int _iceDamage = Mathf.RoundToInt(iceDamage.GetValue() * .1f);
        int _lightningDamage = Mathf.RoundToInt(lightningDamage.GetValue() * .1f);

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage;

        totalMagicalDamage = CheckTargetMagicalDefense(_targetStats, totalMagicalDamage);

        if (totalMagicalDamage <= 0)
            return 0;

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);

        return totalMagicalDamage;
    }

    protected int CheckTargetMagicalDefense(CharacterStats _targetStats, int _totalMagicalDamage)
    {
        _totalMagicalDamage -= Mathf.RoundToInt(Mathf.Pow(_targetStats.mdef.GetValue(), .5f));
        _totalMagicalDamage = Mathf.Clamp(_totalMagicalDamage, 0, int.MaxValue);
        return _totalMagicalDamage;
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
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                break;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                break;
            }

            if (Random.value < .5f && _lightningDamage > 0)
            {
                canApplyShock = true;
                //_targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                break;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(_fireDamage);

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(_lightningDamage);

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

            float slowPercentage = 1f;
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
            return;

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

            //�����Χû������ĵ��ˣ�������ڱ������ĵ��˴������Ч��
            if (closestEnemy == null)
            {
                closestEnemy = transform;
            }
        }

        //todo
        //instatnitate thunder strike
        //setup thunder strike
        //if (closestEnemy != null)
        //{
        //    GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
        //    //newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        //}
    }

    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        DecreaseHpBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.ExecuteFlashFX();

        if (currentHp < 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void DecreaseHpBy(int _damage)
    {
        if (isVulnurable)
        {
            _damage = Mathf.RoundToInt(_damage * vulnurableCoefficient);
        }
        currentHp -= _damage;

        if (onHpChanged != null)
            onHpChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHp += _amount;
        if (currentHp > GetMaxHp())
            currentHp = GetMaxHp();

        if (onHpChanged != null)
            onHpChanged();
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = Mathf.RoundToInt(_damage * ignitedCoefficient);

    public void SetupShockStrikeDamage(int _damage) => shockDamage = Mathf.RoundToInt(_damage * shockedCoefficient);


    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHpBy(igniteDamage);

            currentHp -= igniteDamage;
            if (currentHp < 0 && !isDead)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }
    }

    public Stats GetStats(StatsType _statsType)
    {
        if (_statsType == StatsType.attack)
            return attack;
        else if (_statsType == StatsType.pdef)
            return pdef;
        else if (_statsType == StatsType.mdef)
            return mdef;
        else if (_statsType == StatsType.hp)
            return maxHp;
        else if (_statsType == StatsType.damage)
            return damage;
        else if (_statsType == StatsType.critChance)
            return critChance;
        else if (_statsType == StatsType.critPower)
            return critPower;
        else if (_statsType == StatsType.fireDamage)
            return fireDamage;
        else if (_statsType == StatsType.iceDamage)
            return iceDamage;
        else if (_statsType == StatsType.lightningDamage)
            return lightningDamage;

        return null;
    }

    public virtual void IncreaseStatsBy(int _modifier, float _duration, Stats _statsToModify)
    {
        StartCoroutine(StatsModifyCoroutine(_modifier, _duration, _statsToModify));
    }

    private IEnumerator StatsModifyCoroutine(int _modifier, float _duration, Stats _statsToModify)
    {
        _statsToModify.AddModifier(_modifier);
        //todo fx
        yield return new WaitForSeconds(_duration);
        _statsToModify.RemoveModifier(_modifier);
    }

}
