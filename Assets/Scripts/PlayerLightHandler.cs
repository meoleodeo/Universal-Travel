using UnityEngine;

public class PlayerLightHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite; // Kéo Sprite của Player vào đây
    [SerializeField] private GameObject lightObject;      // Kéo cái đèn (Spot Light) vào đây

    [Header("Settings")]
    [SerializeField] private bool followFlipX = true;     // Xoay theo hướng flipX của Sprite

    void Update()
    {
        if (playerSprite == null || lightObject == null) return;

        // Kiểm tra hướng của Sprite và xoay đèn tương ứng
        if (followFlipX)
        {
            // Nếu flipX = true (quay trái) -> góc 180 độ
            // Nếu flipX = false (quay phải) -> góc 0 độ
            float targetAngle = playerSprite.flipX ? 90f : 270f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // Xoay mượt mà với tốc độ 10f
            lightObject.transform.rotation = Quaternion.Lerp(lightObject.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}