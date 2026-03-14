using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingBomb : MonoBehaviour
{
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private SpriteRenderer[] allSprites;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tạm thời tắt trọng lực để bom lơ lửng
        rb.gravityScale = 0f;

        // Tìm tất cả các hình ảnh (Sprite) của quả bom và ẩn chúng đi
        allSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in allSprites)
        {
            sprite.enabled = false;
        }
    }

    // Hàm này kiểm tra xem Player có đi vào "vùng quét" bên dưới bom không
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFalling && other.CompareTag("Player"))
        {
            StartFalling();
        }
    }

    void StartFalling()
    {
        isFalling = true;

        // Bật trọng lực để bom rơi xuống
        rb.gravityScale = 1f;

        // Bật lại hình ảnh để bom hiện ra
        foreach (SpriteRenderer sprite in allSprites)
        {
            sprite.enabled = true;
        }
    }

    // Nổ khi chạm vào bất kỳ vật thể vật lý nào (Player hoặc Mặt đất)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling)
        {
            // Kiểm tra xem vật va chạm có phải là Player không
            if (collision.gameObject.CompareTag("Player"))
            {
                // Lấy component HealthSystem và trừ 1 máu
                HealthSystem health = collision.gameObject.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(1);
                }
            }

            Explode(); // Sau đó gọi hàm nổ và xóa bom
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // Xóa quả bom
    }
}