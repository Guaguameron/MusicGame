using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameSequence : MonoBehaviour
{
    public Image ReadyImage;
    public Image GoImage;
    public float ReadyDisplayTime = 1.5f;
    public float GoDisplayTime = 1.0f;
    public GameObject GameController;
    public AudioSource GameMusic;

    public Button showButton; // 石台按钮
    public Image imageToShow; // 点击石台后显示的图片
    public Button jumpSceneButton;

    void Start()
    {
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();

        showButton.gameObject.SetActive(false);
        imageToShow.gameObject.SetActive(false);
        jumpSceneButton.gameObject.SetActive(false);

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        ReadyImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(ReadyDisplayTime);
        ReadyImage.gameObject.SetActive(false);

        GoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(GoDisplayTime);
        GoImage.gameObject.SetActive(false);

        if (!PauseGame.isPaused) // 确保游戏未暂停才继续
        {
            GameController.SetActive(true);
            GameMusic.Play();

            // 检查音乐播放状态并处理暂停的情况
            yield return StartCoroutine(WaitForMusicToEnd());

            if (!PauseGame.isPaused)
            {
                showButton.gameObject.SetActive(true);
                showButton.onClick.AddListener(OnButtonClick);
            }
        }
    }

    IEnumerator WaitForMusicToEnd()
    {
        while (GameMusic.isPlaying || PauseGame.isPaused)
        {
            if (!PauseGame.isPaused)
            {
                yield return null; // 游戏正在进行时
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // 暂停时等待
            }
        }
    }

    void OnButtonClick()
    {
        if (!PauseGame.isPaused)
        {
            imageToShow.gameObject.SetActive(true);
            showButton.gameObject.SetActive(false);

            jumpSceneButton.gameObject.SetActive(true);
            jumpSceneButton.onClick.AddListener(JumpToNextScene);
        }
    }

    void JumpToNextScene()
    {
        if (!PauseGame.isPaused)
        {
            SceneManager.LoadScene("Puzzle2");//进解谜游戏
        }
    }

    // 处理失去焦点时的暂停问题
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && GameMusic.isPlaying)
        {
            // 如果失去焦点并且音乐正在播放，将暂停游戏
            PauseGame.isPaused = true;
            GameMusic.Pause(); // 暂停音乐
            GameController.SetActive(false); // 暂停游戏中的其他操作
        }
        else if (hasFocus)
        {
            // 恢复焦点时的处理逻辑
            PauseGame.isPaused = false;
            GameMusic.Play(); // 恢复音乐
            GameController.SetActive(true); // 恢复游戏
        }
    }
}
