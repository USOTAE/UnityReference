using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Lightsaber : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private Animator animator => GetComponentInChildren<Animator>();
    private CapsuleCollider2D capsuleCollider => GetComponent<CapsuleCollider2D>();
    [SerializeField] private string targetLayerName = "Enemy";
    private CharacterStats myStats;

    [SerializeField] private bool canMove;
    private float xVelocity;
    private bool canExplode;
    private float lightsaberExistTime;
    private float lightsaberTimer;
    private bool flipped;

    private void Start()
    {
        lightsaberTimer = lightsaberExistTime;
        if (xVelocity < 0)
            FlipLightsaber();
        SkillManager.instance.lightsaber.hitTarget = false;
    }

    private void Update()
    {
        lightsaberTimer -= Time.deltaTime;
        if (lightsaberTimer < 0)
        {
            if (canExplode)
                animator.SetTrigger("Explode");
            else
                Destroy(gameObject);
        }

        if (canMove)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    public void SetupLightsaber(float _lightsaberExistTime, float _speed, CharacterStats _stats, bool _canExplode)
    {
        lightsaberExistTime = _lightsaberExistTime;
        xVelocity = _speed;
        myStats = _stats;
        canExplode = _canExplode;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStats.DoDamage(collision.GetComponent<CharacterStats>());
            StuckInto(collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckInto(collision, true);
        }
    }

    private void StuckInto(Collider2D collision, bool _isGround=false)
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        canMove = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;

        if(!_isGround)
            SkillManager.instance.lightsaber.hitTarget = true;
    }

    public void LightsaberFinishTrigger()
    {
        Destroy(gameObject);
    }

    public void LightsaberExplodeEventTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, capsuleCollider.size.x);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                myStats.DoDamage(hit.GetComponent<EnemyStats>());
            }
        }
    }

    public void FlipLightsaber()
    {
        if (flipped)
            return;

        flipped = true;
        transform.Rotate(0, 180, 0);
    }
}
