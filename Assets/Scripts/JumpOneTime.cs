using UnityEngine;

public class JumpOneTime : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float bounceForce = 20f;
    [SerializeField] private string playerTag = "Player";

    [Header("Visuals (Animation)")]
    [SerializeField] private Animator anim;
    [SerializeField] private string jumpAnimationName = "JumpOneTime";

    private bool isUsed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;

        if (collision.gameObject.CompareTag(playerTag))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

                    if (rb != null)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                        PlayJumpEffects();

                        isUsed = true;

                        Destroy(gameObject, 0.3f);
                        return;
                    }
                }
            }
        }
    }

    private void PlayJumpEffects()
    {
        if (anim != null && !string.IsNullOrEmpty(jumpAnimationName))
        {
            anim.Play(jumpAnimationName, 0, 0f);
        }
    }
}