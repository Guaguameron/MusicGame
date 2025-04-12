using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; 

public class VideoSceneTransition : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image fadeImage; // 引用用于淡出效果的黑色图像
    public float fadeDuration = 1f; // 淡出持续时间
    public Button skipButton; // 新增：跳过按钮

    // 添加一个字典来存储场景转换规则
    private Dictionary<string, string> sceneTransitions = new Dictionary<string, string>
    {
        {"Animation-Start", "Level1"},
        {"Animation-Lv1Mid", "Development Team"}
    };

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.loopPointReached += OnVideoFinished;
        
        // 确保淡出图像初始时是透明的
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // 设置跳过按钮的点击事件
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipVideo);
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    // 跳过视频的方法
    private void SkipVideo()
    {
        // 直接调用淡出并加载场景的方法
        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        if (fadeImage != null)
        {
            // 逐渐增加图像的不透明度
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
                yield return null;
            }
        }

        // 获取当前场景名称
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 根据当前场景名称决定下一个场景
        string nextSceneName = "Level1"; // 默认场景
        if (sceneTransitions.ContainsKey(currentSceneName))
        {
            nextSceneName = sceneTransitions[currentSceneName];
        }

        // 加载下一个场景
        SceneManager.LoadScene(nextSceneName);
    }
}
