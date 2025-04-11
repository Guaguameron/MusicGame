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
    public BackgroundManager backgroundManager;

    public Image nightAltarImage; // 夜晚祭坛图片
    public AudioSource altarSoundEffect; // 添加祭坛音效组件
    public GameObject S1FBush; // 添加S1FBush的引用
    public Button showButton; // 记忆碎片按钮
    public Image imageToShow; // 记忆碎片点击后显示的说明图片
    public Button jumpSceneButton;//说明图片跳转按钮

    // public Image highlightOutline; // 外框图片（暂时不用）

    private Coroutine pulseCoroutine;

    // 高光效果相关（已注释）
    // private float pulseSpeed = 6f;
    // private bool isHighlighting = false;
    // private Color highlightColor = new Color(1f, 0.84f, 0f, 1f);

    void Start()
    {
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();

        if (nightAltarImage != null)
        {
            nightAltarImage.gameObject.SetActive(false);
        }

        showButton.gameObject.SetActive(false);
        imageToShow.gameObject.SetActive(false);
        jumpSceneButton.gameObject.SetActive(false);

        // if (highlightOutline != null)
        // {
        //     highlightOutline.gameObject.SetActive(false);
        // }

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
                if (backgroundManager != null)
                {
                    backgroundManager.StopScrolling();
                }

                // 等待0.5秒后显示夜晚祭坛图片
                yield return new WaitForSeconds(0.5f);
                if (nightAltarImage != null)
                {
                    // 隐藏S1FBush
                    if (S1FBush != null)
                    {
                        S1FBush.SetActive(false);
                    }

                    // 第一次显示（较长时间）
                    nightAltarImage.gameObject.SetActive(true);
                    if (altarSoundEffect != null) altarSoundEffect.Play();
                    yield return new WaitForSeconds(0.23f);
                    nightAltarImage.gameObject.SetActive(false);
                    if (altarSoundEffect != null) altarSoundEffect.Stop();

                    // 短暂间隔
                    yield return new WaitForSeconds(1.0f);

                    // 第二次闪烁（快速）
                    nightAltarImage.gameObject.SetActive(true);
                    if (altarSoundEffect != null) altarSoundEffect.Play();
                    yield return new WaitForSeconds(0.1f);
                    nightAltarImage.gameObject.SetActive(false);
                    if (altarSoundEffect != null) altarSoundEffect.Stop();

                    // 短暂间隔
                    yield return new WaitForSeconds(0.01f);

                    // 第三次闪烁（快速）
                    nightAltarImage.gameObject.SetActive(true);
                    if (altarSoundEffect != null) altarSoundEffect.Play();
                    yield return new WaitForSeconds(0.1f);
                    nightAltarImage.gameObject.SetActive(false);
                    if (altarSoundEffect != null) altarSoundEffect.Stop();
                }

                showButton.gameObject.SetActive(true);
                showButton.onClick.AddListener(OnButtonClick);

                // 呼吸动画启动：延迟一帧以确保按钮激活后再执行动画
                yield return null;
                pulseCoroutine = StartCoroutine(ButtonPulseEffect(showButton.transform));

                // 高光动画（已注释）
                // highlightOutline.gameObject.SetActive(true);
                // StartCoroutine(HighlightButton());
            }
        }
    }

    IEnumerator WaitForMusicToEnd()
    {
        while (GameMusic.isPlaying || PauseGame.isPaused)
        {
            if (!PauseGame.isPaused)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

   void OnButtonClick()
{
    if (!PauseGame.isPaused)
    {
        // 停止呼吸动画
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;

            // 恢复缩放
            showButton.transform.localScale = Vector3.one;

            // 恢复颜色
            Image buttonImage = showButton.GetComponent<Image>();
            if (buttonImage != null)
                buttonImage.color = Color.white;
        }

        // （高光效果已注释）
        // isHighlighting = false;
        // highlightOutline.gameObject.SetActive(false);

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

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && GameMusic.isPlaying)
        {
            PauseGame.isPaused = true;
            GameMusic.Pause();
            GameController.SetActive(false);
        }
        else if (hasFocus)
        {
            PauseGame.isPaused = false;
            GameMusic.Play();
            GameController.SetActive(true);
        }
    }

    public bool IsMusicPlaying()
    {
        return GameMusic.isPlaying;
    }

    // 呼吸动画
    IEnumerator ButtonPulseEffect(Transform buttonTransform)
    {
    Vector3 originalScale = buttonTransform.localScale;
    float pulseDuration = 1f;
    float scaleMultiplier = 1.5f;

    Image buttonImage = buttonTransform.GetComponent<Image>();
    Color baseColor = Color.white;
    Color pulseColor = new Color(1f, 0.84f, 0f); // 金黄色

    while (buttonTransform.gameObject.activeSelf)
    {
        float timer = 0f;

        // 放大阶段
        while (timer < pulseDuration / 2f)
        {
            float t = timer / (pulseDuration / 2f);
            float scale = Mathf.Lerp(1f, scaleMultiplier, t);
            buttonTransform.localScale = originalScale * scale;

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(baseColor, pulseColor, t);

            timer += Time.deltaTime;
            yield return null;
        }

        // 缩小阶段
        timer = 0f;
        while (timer < pulseDuration / 2f)
        {
            float t = timer / (pulseDuration / 2f);
            float scale = Mathf.Lerp(scaleMultiplier, 1f, t);
            buttonTransform.localScale = originalScale * scale;

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(pulseColor, baseColor, t);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    // 复原
    buttonTransform.localScale = originalScale;
    if (buttonImage != null)
        buttonImage.color = baseColor;
    }


    // 高光协程（已注释）
    /*
    private IEnumerator HighlightButton()
    {
        isHighlighting = true;

        while (isHighlighting && showButton.gameObject.activeSelf)
        {
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            Color newColor = highlightColor;
            newColor.a = alpha;
            highlightOutline.color = newColor;
            yield return null;
        }

        Color finalColor = highlightColor;
        finalColor.a = 0;
        highlightOutline.color = finalColor;
    }
    */
}
