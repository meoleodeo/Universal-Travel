using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private int keysRequired = 3; // Số key cần thiết
    private bool levelCompleted = false;

    private void Start()
    {
        // Reset lại số key về 0 mỗi khi bắt đầu level mới
        KeyItem.ResetKeys();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !levelCompleted)
        {
            // Kiểm tra điều kiện đủ key
            if (KeyItem.KeysCollected >= keysRequired)
            {
                levelCompleted = true;
                Debug.Log("Đủ key! Đang chuyển level...");
                Invoke("LoadNextLevel", 1.5f);
            }
            else
            {
                string msg = "Bạn cần thêm " + (keysRequired - KeyItem.KeysCollected) + " chìa khóa!";
                Debug.Log(msg);

                // Gọi thông báo hiển thị lên màn hình
                if (NotificationManager.instance != null)
                {
                    NotificationManager.instance.ShowNotification(msg);
                }
            }
        }
    }

    private void LoadNextLevel()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.NextLevel();
        }
    }
}