using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public static int KeysCollected = 0; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null && player.GetPlayerNum() == "1")
            {
                KeysCollected++;
                
                // Hiệu ứng âm thanh/hạt nếu cần
                Destroy(gameObject); 
            }
            else
            {
                if (NotificationManager.instance != null)
                {
                    NotificationManager.instance.ShowNotification("Chỉ Player1 (P1) mới có thể nhặt chìa khóa!");
                }
            }
        }
    }

    public static void ResetKeys() => KeysCollected = 0;
}