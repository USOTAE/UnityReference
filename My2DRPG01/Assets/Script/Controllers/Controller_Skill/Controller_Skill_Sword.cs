using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Skill_Sword : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Player player;

    private bool canRotate = true;
    //private bool isReturning;

    private float freezeTimeDuration;
    private float vulnurableDuration;
    //private float returnSpeed = 12;   //todo取消召回剑的动作

    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed;
    [SerializeField] private int bounceAmount;
    [SerializeField] private List<Transform> enemyTarget;
    private bool isBouncing;
    private int targetIndex;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;

    private float hitTimer;
    private float hitCooldown;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        BounceLogic();
    }


    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScal,Player _player,float _freezeTimeDuration, float _vulnurableDuration)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        vulnurableDuration = _vulnurableDuration;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScal;

        if (pierceAmount <= 0)
        {
            animator.SetBool("Rotation", true);
        }

        Invoke(nameof(DestroySelf), 5);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount,float _bouceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;
        bounceSpeed = _bouceSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    private void BounceLogic()
    {
        if(isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < .1f)
            {
                SkillSwordDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++;
                bounceAmount--;

                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    DestroySelf();
                }

                if(targetIndex>= enemyTarget.Count)
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void SkillSwordDamage(Enemy _enemy)
    {
        EnemyStats enemyStats = _enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(_enemy.GetComponent<CharacterStats>());

        if (player.skill.sword.timeStopUnlocked)
            _enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword.vulnurableUnlocked)
            enemyStats.MakeVulnurable(vulnurableDuration);

        //todo 装备效果
    }

    private void SetupTargetForBounce(Collider2D _collision)
    {
        if (_collision.GetComponent<Enemy>() != null)
        {
            if(isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SkillSwordDamage(enemy);
        }

        SetupTargetForBounce(collision);

        StuckInto(collision);
    }

    private void StuckInto(Collider2D collision)
    {
        if(pierceAmount>0&&collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        canRotate = false;
        circleCollider.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //todo player fx

        if (isBouncing && enemyTarget.Count > 0)
            return;

        animator.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
