using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ScoreController.instance != null)
            {
                ScoreController.instance.AddScore(1);
                AudioManager.instance.PlaySFX(AudioManager.instance.coinCollect);
            }

            Destroy(gameObject);
        }
    }
}