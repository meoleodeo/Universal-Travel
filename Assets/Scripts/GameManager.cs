using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameplayUI gameplayUI;

    public static int currentLevel = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayAgain()
    {
        PlayerData.ResetData();
        AudioManager.instance.PlayMusic(AudioManager.instance.backgroundMusic);
        Debug.Log("Current Level: " + currentLevel);
        string sceneName = "Level" + currentLevel;

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            gameplayUI.HideGameOverUI();
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void QuitGame()
    {
        PlayerData.ResetData();
        SceneManager.LoadScene("Menu");
    }

    public void NextLevel()
    {
        currentLevel++;

        string nextSceneName = "Level" + currentLevel;
        Debug.Log("asdasdas" + nextSceneName);

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene("Victory");
            currentLevel = 1;
        }
    }
}