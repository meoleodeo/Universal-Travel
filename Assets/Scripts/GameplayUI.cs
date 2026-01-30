using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        HideGameOverUI();
    }

    public void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void HideGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}