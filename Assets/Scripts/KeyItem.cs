using UnityEngine;

public class KeyItem : MonoBehaviour
{
    // Biến static để lưu trữ số lượng key thu thập được cho cả 2 player
    public static int KeysCollected = 0; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KeysCollected++; // Tăng số lượng key
            Debug.Log("Số key hiện tại: " + KeysCollected);
            
            // Có thể thêm hiệu ứng âm thanh hoặc hạt (Particle) ở đây
            // AudioManager.instance.PlaySFX(AudioManager.instance.coin);

            Destroy(gameObject); // Xóa key sau khi nhặt
        }
    }

    // Hàm để reset lại số key khi bắt đầu level mới
    public static void ResetKeys()
    {
        KeysCollected = 0;
    }
}