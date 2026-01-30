using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class EnemyController : MonoBehaviour
{
    // ... (Các biến settings cũ giữ nguyên) ...
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private int damage = 1;

    private Transform currentTarget;
    private SpriteRenderer sprite;
    private HealthSystem healthSystem;
    private Animator anim;
    private bool isHurting = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();

        currentTarget = pointB;

        // 1. Đăng ký sự kiện Chết
        healthSystem.OnDeath.AddListener(OnEnemyDeath);

        // 2. Đăng ký sự kiện Bị Đau (Gắn hàm bạn muốn vào đây)
        healthSystem.OnDamaged.AddListener(OnTakeDamaged);
    }

    // --- HÀM BẠN MUỐN ĐÂY ---
    public void OnTakeDamaged()
    {
        if (anim != null)
        {
            anim.Play("EnemyTakeDamaged");
        }

        isHurting = true;

        CancelInvoke(nameof(RecoverFromHit));
        Invoke(nameof(RecoverFromHit), 0.5f);
    }

    private void RecoverFromHit()
    {
        isHurting = false; 
    }
    private void OnEnemyDeath()
    {
        // Nhớ hủy đăng ký (RemoveListener) để code sạch
        healthSystem.OnDeath.RemoveListener(OnEnemyDeath);
        healthSystem.OnDamaged.RemoveListener(OnTakeDamaged);

        Destroy(gameObject);
    }

    void Update()
    {
        if (isHurting) return;
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            if (currentTarget == pointB) currentTarget = pointA;
            else currentTarget = pointB;
            Flip();
        }
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        sprite.flipX = currentTarget.position.x < transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                HealthSystem hp = player.GetComponent<HealthSystem>();
                if (hp != null) hp.TakeDamage(damage);

                Vector2 direction = (player.transform.position - transform.position).normalized;
                Vector2 knockbackDir = new Vector2(direction.x > 0 ? 1 : -1, 0.5f);
                player.ApplyKnockback(knockbackDir * knockbackForce);
            }
        }
    }
}