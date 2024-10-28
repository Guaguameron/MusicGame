using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Comingsoon : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;
    public string startSceneName = "StartScene"; 

    void Start()
    {
        // 为重新开始按钮添加监听器
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("Restart Button is not assigned!");
        }

        // 为退出游戏按钮添加监听器
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("Quit Button is not assigned!");
        }
    }

    void RestartGame()
    {
        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(startSceneName);
    }

    void QuitGame()
    {
        Debug.Log("Quitting the game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
