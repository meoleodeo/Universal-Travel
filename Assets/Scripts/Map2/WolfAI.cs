using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class WolfAI : MonoBehaviour
{
    [Header("Target & Ranges")]
    public Transform targetPlayer;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 3f;

    [Header("Stats")]
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int maxHealth = 15;

    [Header("Explosion Effect")]
    [SerializeField] private GameObject bombExplosionPrefab;
    private Animator anim;
    private Rigidbody2D rb;
    private int currentHealth;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isTakingDamage = false;
    private float lastAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isDead || isTakingDamage)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        FindClosestPlayer();

        if (targetPlayer == null)
        {
            anim.Play("DarkWolf_2d_Idle Animation");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
            else
            {
                anim.Play("DarkWolf_2d_Idle Animation");
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            anim.Play("DarkWolf_2d_Idle Animation");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            targetPlayer = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject p in players)
        {
            float distance = Vector2.Distance(transform.position, p.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = p.transform;
            }
        }

        targetPlayer = closestTarget;
    }

    private void ChasePlayer()
    {
        anim.Play("DarkWolf_2d_Run Animation");
        float direction = Mathf.Sign(targetPlayer.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * runSpeed, rb.linearVelocity.y);
        FlipTowards(targetPlayer.position.x);
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        anim.Play("DarkWolf_2d_Attack Animation", -1, 0f);

        StartCoroutine(ExecuteAttack());
    }

    private IEnumerator ExecuteAttack()
    {
        // Đợi tới đúng frame ra đòn
        yield return new WaitForSeconds(0.2f);

        SpawnExplosionEffect();

        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) <= attackRange + 0.5f)
        {
            HealthSystem playerHealth = targetPlayer.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);

                PlayerController pController = targetPlayer.GetComponent<PlayerController>();
                if (pController != null)
                {
                    float pushDirection = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                    pController.ApplyKnockback(new Vector2(pushDirection * 7f, 5f));
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    private void SpawnExplosionEffect()
    {
        if (bombExplosionPrefab == null) return;

        Vector3 spawnPos = targetPlayer != null ? targetPlayer.position : transform.position;
        GameObject explosion = Instantiate(bombExplosionPrefab, spawnPos, Quaternion.identity);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        rb.linearVelocity = Vector2.zero;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isTakingDamage = true;
            anim.Play("DarkWolf_2d_Damage Animation", -1, 0f);
            StartCoroutine(ResetDamageState());
        }
    }

    private IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(0.4f);
        isTakingDamage = false;
    }

    private void Die()
    {
        isDead = true;
        anim.Play("DarkWolf_2d_Death Animation", -1, 0f);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }

    private void FlipTowards(float targetPositionX)
    {
        if (targetPositionX > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (targetPositionX < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}