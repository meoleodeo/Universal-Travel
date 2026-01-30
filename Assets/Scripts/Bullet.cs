using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 7f; 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject impactEffect; // Hiệu ứng nổ (nếu có)

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
        
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
{
    if (hitInfo.CompareTag("Enemy"))
    {
        HealthSystem enemyHealth = hitInfo.GetComponent<HealthSystem>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        
        // Tạo hiệu ứng nổ nếu có
        if (impactEffect != null) Instantiate(impactEffect, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }    
    else if (hitInfo.CompareTag("Ground")) 
    {
        Destroy(gameObject);
    }
}
}