using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class DevelopmentTeamController : MonoBehaviour
{
    public GameObject developmentTeamImage;  // 演职人员图片
    public Button closeButton;               // 关闭按钮
    public Button restartButton;             // 重新开始按钮
    public Button replayButton;              // 重播按钮
    public string startSceneName = "StartGame";  // 起始场景名称

    [Header("Panel Animation Settings")]
    public float scrollDuration = 5f;        // 滚动持续时间
    public float moveSpeed = 100f;           // 每秒移动速度
    private RectTransform panelRect;         // 用于滚动的面板
    private bool isScrolling = false;        // 是否正在滚动中

    [Header("音效设置")]
    public AudioSource audioSource;          // 播放按钮音效的音频源
    public AudioClip hoverClip;              // 悬停音效
    public AudioClip clickClip;              // 点击音效

    public float hoverScale = 1.1f;           // 鼠标悬停时缩放倍数
    public float normalScale = 1f;            // 默认缩放

    void Start()
    {
        InitializeComponents();
        SetupButtons();

        // 游戏开始时立即显示并开始动画
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(true);
            StartCoroutine(ScrollPanel());
        }
    }

    private void InitializeComponents()
    {
        if (developmentTeamImage != null)
        {
            panelRect = developmentTeamImage.GetComponent<RectTransform>();
        }

        // 初始时隐藏两个按钮
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(false);
        }
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        SetupButton(closeButton, HideDevelopmentTeamImage);
        SetupButton(restartButton, RestartGame);
        SetupButton(replayButton, ReplayAnimation);
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                PlayClickSound();
                action.Invoke();
            });
            AddHoverEffects(button);
        }
    }

    // 隐藏演职人员面板
    void HideDevelopmentTeamImage()
    {
        if (developmentTeamImage != null)
        {
            developmentTeamImage.SetActive(false);
            StopAllCoroutines();
            isScrolling = false;
        }
    }

    // 重播动画：重置位置并重新开始滚动
    void ReplayAnimation()
    {
        if (developmentTeamImage != null && !isScrolling)
        {
            // 重新开始动画时隐藏两个按钮
            if (replayButton != null)
            {
                replayButton.gameObject.SetActive(false);
            }
            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(false);
            }

            // 重置 panel 的位置
            panelRect.anchoredPosition = Vector2.zero;
            StartCoroutine(ScrollPanel());
        }
    }

    // 面板滚动动画
    IEnumerator ScrollPanel()
    {
        isScrolling = true;
        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;

            // 简单地向上移动
            Vector2 currentPos = panelRect.anchoredPosition;
            currentPos.y += moveSpeed * Time.deltaTime;
            panelRect.anchoredPosition = currentPos;

            yield return null;
        }

        isScrolling = false;

        // 滚动完毕后显示两个按钮
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(true);
        }
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    // 重新开始游戏
    void RestartGame()
    {
        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(startSceneName);
    }

    // 播放点击音效
    void PlayClickSound()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    // 添加按钮悬停音效 + 放大缩小动画
    void AddHoverEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((data) => {
            PlayHoverSound();
            button.transform.localScale = Vector3.one * hoverScale;
        });

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((data) => {
            button.transform.localScale = Vector3.one * normalScale;
        });

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    // 播放悬停音效
    void PlayHoverSound()
    {
        if (audioSource != null && hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }

#if UNITY_EDITOR
    // 在编辑器中添加调试按钮
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Log Positions"))
        {
            Debug.Log($"Panel Position: {panelRect.anchoredPosition}");
            Debug.Log($"Panel Size: {panelRect.rect.size}");
            Debug.Log($"Panel Anchor Min: {panelRect.anchorMin}");
            Debug.Log($"Panel Anchor Max: {panelRect.anchorMax}");
            Debug.Log($"Panel Pivot: {panelRect.pivot}");
        }
    }
#endif
}