using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 7f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject impactEffect; // Hiệu ứng nổ (nếu có)

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Nếu chạm vào mục tiêu có tag là Enemy
        if (hitInfo.CompareTag("Enemy"))
        {
            // 1. KIỂM TRA QUÁI MÀN 1: Thử tìm HealthSystem
            HealthSystem enemyHealth = hitInfo.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // 2. KIỂM TRA SÓI MÀN 2: Thử tìm WolfAI
            WolfAI wolf = hitInfo.GetComponent<WolfAI>();
            if (wolf != null)
            {
                wolf.TakeDamage(damage);
            }

            // --- THÊM ĐOẠN NÀY ĐỂ ĐÁNH BOSS ---
            BringerOfDeathAI boss = hitInfo.GetComponent<BringerOfDeathAI>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            SuicidalEnemy suicidal = hitInfo.GetComponent<SuicidalEnemy>();
            if (suicidal != null)
            {
                suicidal.TakeDamage(damage);
            }

            // Tạo hiệu ứng nổ nếu có
            if (impactEffect != null) Instantiate(impactEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
        else if (hitInfo.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}