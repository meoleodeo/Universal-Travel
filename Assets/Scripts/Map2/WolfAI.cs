using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class WolfAI : MonoBehaviour
{
    [Header("Target & Ranges")]
    public Transform targetPlayer; 
    [SerializeField] private float chaseRange = 6f;   
    [SerializeField] private float attackRange = 3.5f; // Đã tăng sẵn lên mức an toàn
    
    [Header("Stats")]
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float attackCooldown = 1.5f; 
    [SerializeField] private int maxHealth = 3;

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
        
        // CỨU CÁNH Ở ĐÂY: Thêm ", -1, 0f" để ép Unity CHẮC CHẮN phải chạy lại animation từ đầu
        anim.Play("DarkWolf_2d_Attack Animation", -1, 0f);

        StartCoroutine(ExecuteAttack());
    }

    private IEnumerator ExecuteAttack()
    {
        // 1. Đợi một chút xíu (0.2 giây) cho khớp với lúc móng vuốt vung xuống chạm người
        yield return new WaitForSeconds(0.2f); 

        // 2. Gây sát thương: Nếu Player vẫn còn nằm trong tầm cào (cộng thêm 0.5f phòng hờ Player nhích nhẹ)
        if (targetPlayer != null && Vector2.Distance(transform.position, targetPlayer.position) <= attackRange + 0.5f)
        {
            HealthSystem playerHealth = targetPlayer.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Trừ máu theo script HealthSystem của bạn
                
                // Hất văng Player ra sau để tránh bị kẹt góc vĩnh viễn
                PlayerController pController = targetPlayer.GetComponent<PlayerController>();
                if (pController != null)
                {
                    float pushDirection = Mathf.Sign(targetPlayer.position.x - transform.position.x);
                    pController.ApplyKnockback(new Vector2(pushDirection * 7f, 5f));
                }
            }
        }

        // 3. Đợi nốt phần thời gian còn lại của animation (tổng là 0.5s)
        yield return new WaitForSeconds(0.3f); 
        isAttacking = false;
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
        
        // 1. Dừng mọi di chuyển
        rb.linearVelocity = Vector2.zero;
        
        // 2. Tắt trọng lực để xác không bị rơi xuyên qua mặt đất
        rb.gravityScale = 0f; 
        
        // 3. Tắt khung va chạm để Player đi xuyên qua xác chết
        GetComponent<Collider2D>().enabled = false; 
        
        // 4. Hủy object sau 2 giây (đủ để xem hết animation chết)
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