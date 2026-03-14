using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Dòng này tự động thêm Rigidbody2D nếu bạn quên
public class SuicidalEnemy : MonoBehaviour
{
    public GameObject explosionPrefab; 
    public float moveSpeed = 2f;
    public float patrolRange = 3f; 
    public float explodeDistance = 1.2f;

    private Transform player;
    private bool isAttacking = false;
    private Vector3 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        startPosition = transform.position; 
        rb = GetComponent<Rigidbody2D>(); // Lấy component vật lý
    }

    public void SetAttackMode(bool active)
    {
        isAttacking = active;
        if (!active) startPosition = transform.position;
    }

    // Update dùng để kiểm tra khoảng cách nổ
    void Update()
    {
        if (isAttacking && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) < explodeDistance) 
            {
                Explode();
            }
        }
    }

    // Hàm này tự động kích hoạt khi Collider của Enemy đập trúng Collider khác
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu kẻ bị đụng trúng có Tag là "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    // FixedUpdate CHUYÊN DÙNG để xử lý di chuyển vật lý (tránh giật lag)
    [System.Obsolete]
    void FixedUpdate()
    {
        if (isAttacking && player != null)
        {
            // Chỉ di chuyển ngang (trục X) theo hướng của Player
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            
            // rb.velocity.y giữ nguyên lực rơi của trọng lực
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Đi tuần tra (Patrol) qua lại quanh điểm gốc
            float targetX = startPosition.x + Mathf.PingPong(Time.time * moveSpeed, patrolRange) - (patrolRange / 2);
            float direction = targetX > transform.position.x ? 1f : -1f;
            
            // Kiểm tra nếu chưa tới đích thì đi tiếp, tới nơi thì đứng lại
            if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
            {
                rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y); // Dừng lại
            }
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}