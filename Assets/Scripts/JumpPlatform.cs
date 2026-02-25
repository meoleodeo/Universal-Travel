using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float bounceForce = 20f;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visuals (Direct Animation)")]
    [SerializeField] private Animator anim;
    [SerializeField] private string jumpAnimationName = "JumpPlatform"; // Tên chính xác của Clip Animation

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Kiểm tra hướng va chạm từ trên xuống
                if (contact.normal.y < -0.5f)
                {
                    Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

                    if (rb != null)
                    {
                        // Reset vận tốc Y và bật nhảy
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                        // Chạy hiệu ứng
                        PlayJumpEffects();
                        return; 
                    }
                }
            }
        }
    }

    private void PlayJumpEffects()
    {
        // Sử dụng anim.Play để chạy trực tiếp tên Clip
        if (anim != null && !string.IsNullOrEmpty(jumpAnimationName))
        {
            anim.Play(jumpAnimationName, 0, 0f); // Phát lại từ đầu
        }

        if (AudioManager.instance != null)
        {
            // AudioManager.instance.PlaySFX(AudioManager.instance.jump);
        }
    }
}