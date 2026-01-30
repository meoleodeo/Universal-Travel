using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true;

            Invoke("LoadNextLevel", 1.5f);
        }
    }

    private void LoadNextLevel()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.NextLevel();
        }
    }
}