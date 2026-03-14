using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Left-Right Settings")]
    public float patrolDistance = 3f; 
    public float speed = 2f;

    [Header("Combat Settings")]
    public float attackRange = 5f; 
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float fireRate = 1f; 
    
    [Header("Effects")]
    public float flashDuration = 0.05f;

    private Vector2 startPosition;
    private bool movingRight = true; 
    private float nextFireTime = 0f;
    
    // Đổi tên biến thành targetPlayer cho rõ nghĩa
    private Transform targetPlayer; 

    void Start()
    {
        startPosition = transform.position;
        if (firePoint != null) firePoint.gameObject.SetActive(false);
    }

    void Update()
    {
        // Liên tục cập nhật xem ai đang ở gần nhất
        FindClosestPlayer();

        // Nếu cả 2 Player đều chết hoặc không có ai, thì cứ đi tuần
        if (targetPlayer == null)
        {
            PatrolLeftRight();
            return;
        }

        // Nếu có Player, tính khoảng cách tới người GẦN NHẤT
        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            PatrolLeftRight();
        }
    }

    // --- HÀM MỚI: TÌM NGƯỜI CHƠI GẦN NHẤT ---
    private void FindClosestPlayer()
    {
        // Lấy danh sách TẤT CẢ những object có Tag "Player" (Có chữ 's' ở FindGameObjects)
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        float shortestDistance = Mathf.Infinity; // Đặt khoảng cách ban đầu là vô cực
        Transform closestPlayer = null;

        // Quét từng người chơi một
        foreach (GameObject p in players)
        {
            // Tính khoảng cách từ quái đến người chơi này
            float distanceToPlayer = Vector2.Distance(transform.position, p.transform.position);
            
            // Nếu người này gần hơn kỷ lục hiện tại, thì lưu người này lại làm mục tiêu
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                closestPlayer = p.transform;
            }
        }

        // Chốt mục tiêu cuối cùng
        targetPlayer = closestPlayer;
    }

    private void PatrolLeftRight()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            FlipSprite(-1); 
            if (transform.position.x >= startPosition.x + patrolDistance) movingRight = false;
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            FlipSprite(1); 
            if (transform.position.x <= startPosition.x - patrolDistance) movingRight = true;
        }
    }

    private void AttackPlayer()
    {
        float directionToPlayer = targetPlayer.position.x > transform.position.x ? -1 : 1; 
        FlipSprite(directionToPlayer);

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        float facingDirection = -Mathf.Sign(transform.localScale.x); 

        Round bulletScript = bullet.GetComponent<Round>();
        if (bulletScript != null)
        {
            bulletScript.Setup(facingDirection);
        }

        if (firePoint != null)
        {
            StartCoroutine(ShowMuzzleFlash());
        }
    }

    private IEnumerator ShowMuzzleFlash()
    {
        firePoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        firePoint.gameObject.SetActive(false);
    }

    private void FlipSprite(float direction)
    {
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * direction;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Vector3 startPos = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.DrawLine(new Vector3(startPos.x - patrolDistance, startPos.y, startPos.z), 
                        new Vector3(startPos.x + patrolDistance, startPos.y, startPos.z));
    }
}