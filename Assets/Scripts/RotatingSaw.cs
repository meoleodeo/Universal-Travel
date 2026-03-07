using UnityEngine;

public class RotatingSaw : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform rotationPoint; // Điểm tâm để xoay quanh
    [SerializeField] private float rotationSpeed = 100f; // Tốc độ xoay (độ/giây)
    [SerializeField] private float radius = 3f; // Khoảng cách từ tâm đến lưỡi cưa

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private int damage = 1;

    private float currentAngle = 0f;

    void Update()
    {
        if (rotationPoint == null) return;

        // 1. Tính toán vị trí xoay tròn dựa trên lượng giác
        currentAngle += rotationSpeed * Time.deltaTime;
        float x = rotationPoint.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
        float y = rotationPoint.position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;

        transform.position = new Vector2(x, y);

        // 2. Tự xoay bản thân lưỡi cưa (xoay quanh trục Z) để trông thật hơn
        transform.Rotate(Vector3.forward * rotationSpeed * 2 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            HealthSystem health = collision.GetComponent<HealthSystem>();

            if (player != null)
            {
                // Tính toán hướng bị bật ra (từ tâm lưỡi cưa đến người chơi)
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                
                // Gọi hàm ApplyKnockback có sẵn trong PlayerController của bạn
                player.ApplyKnockback(knockbackDir * knockbackForce);

                // Trừ máu nếu có HealthSystem
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }
}