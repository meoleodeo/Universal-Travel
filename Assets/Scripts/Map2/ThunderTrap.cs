using UnityEngine;

public class ThunderTrap : MonoBehaviour
{
    public GameObject thunderPrefab; // Kéo Prefab hiệu ứng sấm sét vào đây
    public Transform strikePoint;    // Điểm mà sấm sét sẽ đánh xuống (thường là vị trí của cái bẫy)
    public float delayTime = 0.5f;   // Thời gian trễ từ lúc báo động đến lúc sét đánh
    
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            Invoke("SpawnThunder", delayTime); // Tạo khoảng trễ để người chơi kịp né
            // Bạn có thể thêm hiệu ứng cảnh báo (vòng tròn đỏ dưới đất) ở đây
        }
    }

    void SpawnThunder()
    {
        if (thunderPrefab != null)
        {
            Instantiate(thunderPrefab, strikePoint.position, Quaternion.identity);
        }
        
        // Nếu bẫy này chỉ dùng 1 lần thì Destroy, nếu dùng nhiều lần thì reset isTriggered sau vài giây
        Destroy(gameObject, 1f); 
    }
}