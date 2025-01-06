using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy_DeathBriner : Enemy
{
    public bool bossFightBegun;

    [Header("Spell cast details")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private Vector2 spellOffset;

    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroundingCheckSize;
    public float chanceToTeleport;
    public float defaultChanceToTeleport = 25;

    #region States
    public DeathBrinerBattleState battleState { get; private set; }
    public DeathBrinerAttackState attackState { get; private set; }
    public DeathBrinerIdleState idleState { get; private set; }
    public DeathBrinerDeadState deadState { get; private set; }
    public DeathBrinerSpellCastState spellCastState { get; private set; }
    public DeathBrinerTeleportState teleportState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetupDefaultFacingDir(-1);

        battleState = new DeathBrinerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBrinerAttackState(this, stateMachine, "Attack", this);
        idleState = new DeathBrinerIdleState(this, stateMachine, "Idle", this);
        deadState = new DeathBrinerDeadState(this, stateMachine, "Idle", this);
        spellCastState = new DeathBrinerSpellCastState(this, stateMachine, "SpellCast", this);
        teleportState = new DeathBrinerTeleportState(this, stateMachine, "Teleport", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;
        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * spellOffset.x;
        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellOffset.y);
        
        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBrinerSpell_Controller>().SetupSpell(stats);
    }


    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

        if(!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Looking for new position");
            FindPosition();
        }
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }
        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time > lastTimeCast + spellStateCooldown)
        {
            return true;
        }

        return false;
    }

}
