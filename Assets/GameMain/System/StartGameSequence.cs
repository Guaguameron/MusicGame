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
    public BackgroundManager backgroundManager; // 对BackgroundManager的引用

    public Button showButton; // 石台按钮
    public Image imageToShow; // 点击石台后显示的图片
    public Button jumpSceneButton;
    public Image highlightOutline; // 石台外框图片
    private float pulseSpeed = 6f; // 石台外框高亮闪烁速度
    private bool isHighlighting = false; // 是否正在高亮
    private Color highlightColor = new Color(1f, 0.84f, 0f, 1f); // 金黄色 (255, 215, 0)

    void Start()
    {
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();

        showButton.gameObject.SetActive(false);
        imageToShow.gameObject.SetActive(false);
        jumpSceneButton.gameObject.SetActive(false);
        if (highlightOutline != null)
        {
            highlightOutline.gameObject.SetActive(false);
        }

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

        if (!PauseGame.isPaused)
        {
            GameController.SetActive(true);
            GameMusic.Play();

            yield return StartCoroutine(WaitForMusicToEnd());

            if (!PauseGame.isPaused)
            {
                // 音乐结束后停止背景滚动
                if (backgroundManager != null)
                {
                    backgroundManager.StopScrolling();
                }

                showButton.gameObject.SetActive(true);
                highlightOutline.gameObject.SetActive(true);
                showButton.onClick.AddListener(OnButtonClick);
                StartCoroutine(HighlightButton());
            }
        }
    }

    IEnumerator WaitForMusicToEnd()
    {
        while (GameMusic.isPlaying || PauseGame.isPaused)
        {
            if (!PauseGame.isPaused)
            {
                yield return null; //游戏正在进行时
            }
            else
            {
                yield return new WaitForSeconds(0.1f); //暂停时等待
            }
        }
    }

    // 高亮动画协程
    private IEnumerator HighlightButton()
    {
        isHighlighting = true;

        while (isHighlighting && showButton.gameObject.activeSelf)
        {
            // 使用正弦函数创建平滑的颜色透明度变化
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // 值在0到1之间变化
            Color newColor = highlightColor;
            newColor.a = alpha;
            highlightOutline.color = newColor;
            yield return null;
        }

        // 确保最后是透明的
        Color finalColor = highlightColor;
        finalColor.a = 0;
        highlightOutline.color = finalColor;
    }

    void OnButtonClick()
    {
        if (!PauseGame.isPaused)
        {
            isHighlighting = false; // 停止高亮动画
            highlightOutline.gameObject.SetActive(false);
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
            SceneManager.LoadScene("Puzzle2");
        }
    }

    //处理失去焦点时的暂停问题
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

    public bool IsMusicPlaying()
    {
        return GameMusic.isPlaying;
    }
}
