using UnityEngine;

public class TrapColinder : MonoBehaviour
{
    [SerializeField] private int damage = 1; // Sát thương của bẫy

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         HealthSystem player = collision.gameObject.GetComponent<HealthSystem>();
    //         if (player != null)
    //         {
    //             player.TakeDamage(damage);
    //         }
    //     }
    // }

    //dạng xuyên qua
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem player = collision.GetComponent<HealthSystem>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player hit a trap and took " + damage + " damage.");
            }
        }
    }
}