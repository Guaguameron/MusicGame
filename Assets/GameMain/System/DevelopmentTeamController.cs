using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevelopmentTeamController : MonoBehaviour
{
    
    public GameObject developmentTeamImage;

   
    public Button showDevelopmentTeamButton; 
    public Button closeButton; 
    public Button restartButton; // 重新开始按钮
    public string startSceneName = "StartScene"; //开始场景的名称

    void Start()
    {
        showDevelopmentTeamButton.onClick.AddListener(ShowDevelopmentTeamImage);

        closeButton.onClick.AddListener(HideDevelopmentTeamImage);
        
        // 为重新开始按钮添加监听器
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("Restart Button is not assigned!");
        }

        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(false);
        }
    }

   
    void ShowDevelopmentTeamImage()
    {
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(true);
        }
    }

   
    void HideDevelopmentTeamImage()
    {
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(false);
        }
    }

    //重新开始游戏的方法
    void RestartGame()
    {
        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(startSceneName);
    }
}
