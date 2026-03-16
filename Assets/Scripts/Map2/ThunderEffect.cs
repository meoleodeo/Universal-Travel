using UnityEngine;

public class ThunderEffect : MonoBehaviour
{
    private Collider2D strikeCollider;
    public int damageAmount = 1; // Số máu sẽ trừ

    void Awake() {
        strikeCollider = GetComponent<Collider2D>();
        if (strikeCollider != null) {
            strikeCollider.enabled = false; // Lúc đầu tắt để tránh gây sát thương nhầm
        }
    }

    // --- PHẦN BỊ THIẾU: Xử lý va chạm ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu Collider đang bật và chạm đúng Player
        if (strikeCollider.enabled && other.CompareTag("Player"))
        {
            // Dùng GetComponentInParent để an toàn nhất (như đã thảo luận)
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                Debug.Log("Sét đã trừ máu của: " + other.name);
            }
        }
    }

    // Hàm này gọi từ Animation Event (Frame sét đánh mạnh nhất)
    public void ApplyDamage() {
        if (strikeCollider != null) {
            strikeCollider.enabled = true; 
            Debug.Log("Sét đánh trúng - Đang kích hoạt vùng sát thương!");
        }
    }

    // Hàm này gọi từ Animation Event (Frame sét biến mất)
    public void EndDamage() {
        if (strikeCollider != null) {
            strikeCollider.enabled = false;
        }
    }
}