using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem hp = collision.GetComponent<HealthSystem>();
            if (hp != null)
            {
                hp.TakeDamage(999);
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
             Destroy(collision.gameObject);
        }
    }
}