using UnityEngine;
using System.Collections;

public class BossSpell : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 1.5f;
    public float delayBeforeDamage = 0.5f; // Thời gian chờ để Player kịp né

    private Collider2D col;
    private SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        
        // 1. Lúc mới sinh ra: Tắt Collider và làm mờ Sprite
        col.enabled = false;
        Color tempColor = sr.color;
        tempColor.a = 0.3f; // Độ trong suốt 30% (nhìn như bóng ma)
        sr.color = tempColor;
    }

    void Start()
    {
        StartCoroutine(ActivateSpell());
        Destroy(gameObject, lifetime);
    }

    IEnumerator ActivateSpell()
    {
        // 2. Đợi một khoảng thời gian cảnh báo
        yield return new WaitForSeconds(delayBeforeDamage);

        // 3. Kích hoạt: Bật Collider và làm Sprite hiện rõ 100%
        col.enabled = true;
        Color tempColor = sr.color;
        tempColor.a = 1f; 
        sr.color = tempColor;
        
        // (Tùy chọn) Thêm hiệu ứng rung màn hình ở đây nếu muốn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var health = collision.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(damage);
        }
    }
}