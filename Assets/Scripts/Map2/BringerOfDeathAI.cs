using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BringerOfDeathAI : MonoBehaviour
{
    public enum BossType { MeleeOnly, RangedOnly, Hybrid }

    [Header("Boss Configuration")]
    public BossType bossType = BossType.Hybrid;

    [Header("Targeting")]
    private Transform targetPlayer;

    [Header("Stats")]
    public float moveSpeed = 3f;
    public float rangedRange = 10f;
    public float meleeRange = 2f;
    public float meleeCooldown = 1.5f;
    public float rangedCooldown = 5f;
    public int currentHealth = 30;

    [Header("Prefabs")]
    public GameObject spellPrefab;
    public GameObject rowSpellPrefab;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isInvincible = false;
    private float lastMeleeTime;
    private float lastRangedTime;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Chặn boss bị ngã nghiêng
        lastMeleeTime = -meleeCooldown;
        lastRangedTime = -rangedCooldown;
    }

    void Update()
    {
        if (currentHealth <= 0 || isInvincible)
        {
            StopMoving();
            return;
        }

        FindClosestPlayer();

        if (targetPlayer == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        // QUAN TRỌNG: Nếu đang tấn công thì KHÔNG cho phép tính toán di chuyển
        if (isAttacking)
        {
            // Giữ vận tốc X bằng 0 nhưng vẫn giữ vận tốc Y (trọng lực)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        HandleAIBehavior(distance);
    }

    void FindClosestPlayer()
    {
        // Tự động tìm tất cả đối tượng có Tag "Player" (Giống Wolf - rất an toàn)
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        targetPlayer = null;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                targetPlayer = p.transform;
            }
        }
    }

    void HandleAIBehavior(float distance)
    {
        switch (bossType)
        {
            case BossType.MeleeOnly:
                if (distance <= meleeRange) TryMeleeAttack();
                else MoveToPlayer();
                break;

            case BossType.RangedOnly:
                if (distance <= rangedRange) TryRangedAttack();
                else StopMoving();
                break;

            case BossType.Hybrid:
                // Ưu tiên cận chiến nếu ở gần
                if (distance <= meleeRange)
                {
                    TryMeleeAttack();
                }
                // Nếu ở xa nhưng trong tầm bắn
                else if (distance <= rangedRange)
                {
                    // Thử bắn, nếu đang hồi chiêu thì vẫn tiếp tục tiến lại gần
                    if (Time.time >= lastRangedTime + rangedCooldown) TryRangedAttack();
                    else MoveToPlayer();
                }
                else
                {
                    MoveToPlayer();
                }
                break;
        }
    }

    void MoveToPlayer()
    {
        // Kiểm tra xem có đang ở trạng thái Walk trong Animator không
        anim.SetBool("isWalking", true);

        float directionX = Mathf.Sign(targetPlayer.position.x - transform.position.x);

        // Áp dụng vận tốc di chuyển mượt mà
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);

        Flip(directionX);
    }

    void StopMoving()
    {
        anim.SetBool("isWalking", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void Flip(float dir)
    {
        if (dir > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dir < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void TryMeleeAttack()
    {
        if (Time.time >= lastMeleeTime + meleeCooldown)
        {
            StartCoroutine(MeleeSequence());
        }
    }

    IEnumerator MeleeSequence()
    {
        isAttacking = true;
        lastMeleeTime = Time.time;
        StopMoving();

        anim.SetTrigger("Attack");

        // Đợi một chút để đồng bộ với hoạt ảnh (Animation)
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    public void DealMeleeDamage()
    {
        if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= meleeRange)
        {
            var health = targetPlayer.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(1);
        }
    }

    void TryRangedAttack()
    {
        if (Time.time >= lastRangedTime + rangedCooldown)
        {
            StartCoroutine(RangedSequence());
        }
    }

    IEnumerator RangedSequence()
    {
        isAttacking = true;
        lastRangedTime = Time.time;
        StopMoving();

        anim.SetTrigger("Cast");

        // Spawn chiêu tại vị trí Player
        Vector3 spawnPos = targetPlayer.position + Vector3.up * 2f;
        Instantiate(spellPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(0.8f);
        isAttacking = false;
    }

    // Hàm nhận sát thương cho Boss
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 1 && !isInvincible)
        {
            StartCoroutine(InvincibilityPhase());
        }
        else if (currentHealth <= 0)
        {
            anim.SetTrigger("Death");
            this.enabled = false;
        }
    }

    void BossDie()
    {
        currentHealth = 0;
        isInvincible = false; // Đảm bảo tắt bất tử để không lỗi
        anim.SetTrigger("Death");

        // Tắt script AI để boss không di chuyển hay tấn công nữa
        this.enabled = false;

        // Tắt Collider để Player không bị vấp vào xác Boss hoặc bị dính sát thương va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    IEnumerator InvincibilityPhase()
    {
        isInvincible = true;
        currentHealth = 1;
        Debug.Log("Boss tung tàn lực cuối cùng!");

        float timer = 0;
        float phaseDuration = 5f; // Thời gian cuồng nộ

        while (timer < phaseDuration)
        {
            anim.SetTrigger("Cast");
            SpawnRowSpells();

            float delay = 1.0f; // Thời gian nghỉ giữa mỗi đợt hàng ngang
            yield return new WaitForSeconds(delay);
            timer += delay;
        }

        // --- HẾT 5 GIÂY CUỒNG NỘ -> BOSS TỰ CHẾT ---
        BossDie();
    }

    void SpawnRowSpells()
    {
        // Chỉ thực hiện nếu có Player trong tầm ngắm
        if (targetPlayer == null) return;

        // Xác định tâm của hàng spell: Tại vị trí X của Player, nhưng cao hơn 2 ô (Y + 2)
        // Chúng ta giữ nguyên Z để đảm bảo va chạm chính xác
        Vector3 centerPos = targetPlayer.position + Vector3.up * 2f;

        for (int i = -2; i <= 2; i++)
        {
            // Tính toán vị trí từng viên trong hàng 5 viên
            // Mỗi viên cách nhau 2 đơn vị (2.0f) theo trục X
            float xOffset = i * 2.0f;
            Vector3 spawnPos = new Vector3(centerPos.x + xOffset, centerPos.y, centerPos.z);

            // Tạo Spell
            GameObject go = Instantiate(rowSpellPrefab, spawnPos, Quaternion.identity);

            // Gán thông số sát thương và thời gian tồn tại
            BossSpell s = go.GetComponent<BossSpell>();
            if (s != null)
            {
                s.damage = 1;
                s.lifetime = 1.0f; // Tự biến mất sau 1 giây
            }
        }
    }
}