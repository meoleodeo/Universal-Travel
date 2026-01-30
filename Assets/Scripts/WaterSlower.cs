using UnityEngine;

public class WaterSlower : MonoBehaviour
{
    [Header("Water Settings")]
    [Range(0.1f, 1f)]
    public float waterSlowDownFactor = 0.4f; // Giảm còn 40% tốc độ

    private PlayerController playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            playerMovement.SetSpeedMultiplier(waterSlowDownFactor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            playerMovement.SetSpeedMultiplier(1f);
        }
    }
}