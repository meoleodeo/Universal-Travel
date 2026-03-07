using UnityEngine;
using TMPro; // Sử dụng TextMeshPro để chữ đẹp hơn
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject notificationPanel; // Panel chứa thông báo
    [SerializeField] private TextMeshProUGUI notificationText; // Thành phần chữ

    [Header("Settings")]
    [SerializeField] private float displayDuration = 2f; // Thời gian hiển thị

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Khởi tạo Singleton để gọi NotificationManager.instance từ mọi nơi
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Ẩn panel lúc bắt đầu
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel == null || notificationText == null) return;

        notificationText.text = message;
        
        // Dừng coroutine cũ nếu đang chạy để bắt đầu cái mới (tránh chồng chéo)
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        notificationPanel.SetActive(true);
        
        // Bạn có thể thêm code hiệu ứng Fade In ở đây nếu muốn

        yield return new WaitForSeconds(displayDuration);

        // Hiệu ứng Fade Out đơn giản hoặc chỉ cần ẩn đi
        notificationPanel.SetActive(false);
    }
}