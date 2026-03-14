using UnityEngine;

public class Round : MonoBehaviour
{
    [Header("Bullet Stats")]
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 7f; 

    private float direction = 1f;

    void Start()
    {
        // Tự hủy sau một thời gian để dọn rác bộ nhớ
        Destroy(gameObject, lifeTime);
    }

    // Hàm nhận hướng bay từ EnemyAI
    public void Setup(float facingDirection)
    {
        direction = facingDirection;
    }

    void Update()
    {
        // Bay thẳng theo trục X
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // QUAN TRỌNG: Đạn của quái thì CHỈ check va chạm với "Player"
        if (hitInfo.CompareTag("Player"))
        {
            // Trừ máu Player
            HealthSystem playerHealth = hitInfo.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            // Phá hủy viên đạn sau khi trúng Player
            Destroy(gameObject);
        }    
        // Hoặc hủy viên đạn nếu đập vào tường/đất
        else if (hitInfo.CompareTag("Ground")) 
        {
            Destroy(gameObject);
        }
        
        // LƯU Ý: Không có dòng check tag "Enemy" ở đây nữa, 
        // nên viên đạn bay xuyên qua người con quái mà không bị tự hủy.
    }
}