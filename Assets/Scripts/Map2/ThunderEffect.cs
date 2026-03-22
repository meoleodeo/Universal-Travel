using UnityEngine;

public class ThunderEffect : MonoBehaviour
{
    private Collider2D strikeCollider;
    public int damageAmount = 1;
    private bool hasDealtDamage = false; // Biến kiểm soát chỉ gây sát thương 1 lần

    void Awake() {
        strikeCollider = GetComponent<Collider2D>();
        if (strikeCollider != null) {
            strikeCollider.enabled = false; 
        }
        // Tự hủy tia sét sau khi diễn xong animation (ví dụ 1.5s)
        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ gây sát thương nếu chưa gây lần nào trong vòng đời của tia sét này
        if (!hasDealtDamage && strikeCollider.enabled && other.CompareTag("Player"))
        {
            HealthSystem health = other.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                hasDealtDamage = true; // Khóa sát thương lại
                Debug.Log("Sét đánh trúng!");
            }
        }
    }

    public void ApplyDamage() {
        if (strikeCollider != null) strikeCollider.enabled = true; 
    }

    public void EndDamage() {
        if (strikeCollider != null) strikeCollider.enabled = false;
    }
}