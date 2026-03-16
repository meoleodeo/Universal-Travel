using UnityEngine;

public class SpellDamage : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 2f;

    void Start() { Destroy(gameObject, lifeTime); }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
