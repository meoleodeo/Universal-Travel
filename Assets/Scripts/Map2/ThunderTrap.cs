using UnityEngine;

public class ThunderTrap : MonoBehaviour
{
    public GameObject thunderPrefab; 
    public Transform strikePoint;    
    public float delayTime = 0.5f;   
    
    private bool isTriggered = false;
    private Collider2D trapCollider; // Thêm biến để tắt Collider ngay lập tức

    void Awake() {
        trapCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            
            // QUAN TRỌNG: Tắt collider của bẫy ngay để không nhận thêm va chạm nào nữa
            if(trapCollider != null) trapCollider.enabled = false;

            Invoke("SpawnThunder", delayTime);
        }
    }

    void SpawnThunder()
    {
        if (thunderPrefab != null)
        {
            // Tạo ra tia sét
            Instantiate(thunderPrefab, strikePoint.position, Quaternion.identity);
        }
        
        // Xóa cái bẫy đi sau khi đã gọi sét
        Destroy(gameObject, 0.1f); 
    }
}