using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuController : MonoBehaviour
{
    public Button startButton;
    public Button developmentTeamButton;
    public Button exitButton;
    public Button settingsButton; 
    public GameObject settingsImage; 
    public Button settingsBackButton; 

    public Image hoverBackgroundImage;

    public string hoverColorHex = "#50848B";
    private Color hoverColor;

    private Color originalColor;

    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    [Range(0f, 1f)]
    public float volume = 1f;  // 这个变量现在只用于按钮音效

    public Slider volumeSlider;
    private const string VolumePrefsKey = "MusicVolume";

    public AudioSource backgroundMusicSource;
    private float musicVolume = 1f; // 背景音乐音量

    void Start()
    {
        if (ColorUtility.TryParseHtmlString(hoverColorHex, out hoverColor))
        {
            // Debug.Log("成功将十六进制颜色转换为 Color 类型");
        }
        else
        {
            // Debug.LogError("十六进制颜色转换失败，请检查格式");
        }

        hoverBackgroundImage.gameObject.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;  // 设置按钮音效的音量

        AddHoverEffect(startButton, StartGame);
        AddHoverEffect(developmentTeamButton, OpenDevelopmentTeamScene);
        AddHoverEffect(exitButton, QuitGame);
        AddHoverEffect(settingsButton, OpenSettings); //为设置按钮添加悬停效果

        // 为设置图片中的返回按钮添加点击事件
        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.AddListener(CloseSettings);
        }

        // 确保设置图片初始时是隐藏的
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
        }

        // 初始化音量滑块
        if (volumeSlider != null)
        {
            musicVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 1f);
            volumeSlider.value = musicVolume;
            volumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // 初始化背景音乐
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = musicVolume;
            backgroundMusicSource.Play();
        }

        // 应用初始音量设置
        ApplyMusicVolumeSettings();
    }

    void OnMusicVolumeChanged(float newVolume)
    {
        musicVolume = newVolume;
        ApplyMusicVolumeSettings();
        PlayerPrefs.SetFloat(VolumePrefsKey, musicVolume);
        PlayerPrefs.Save();
    }

    void ApplyMusicVolumeSettings()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = musicVolume;
        }
    }

    // 为按钮添加悬停效果并绑定点击事件
    void AddHoverEffect(Button button, UnityEngine.Events.UnityAction onClickAction)
    {
        button.onClick.AddListener(() => { PlayButtonClickSound(); onClickAction.Invoke(); });

        originalColor = button.GetComponentInChildren<Text>().color;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // 鼠标进入时更改字体颜色并显示背景图片
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { OnHoverEnter(button); });
        trigger.triggers.Add(pointerEnter);

        // 鼠标移开时恢复字体颜色并隐藏背景图片
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { OnHoverExit(button); });
        trigger.triggers.Add(pointerExit);
    }

    // 悬停时改变字体颜色并显示背景图片
    void OnHoverEnter(Button button)
    {
        button.GetComponentInChildren<Text>().color = hoverColor;

        hoverBackgroundImage.gameObject.SetActive(true);
        hoverBackgroundImage.rectTransform.position = button.transform.position - new Vector3(0, button.GetComponent<RectTransform>().sizeDelta.y / 2, 0);

        hoverBackgroundImage.rectTransform.sizeDelta = button.GetComponent<RectTransform>().sizeDelta;
    }

    // 鼠标移开时恢复原来的字体颜色并隐藏背景图片
    void OnHoverExit(Button button)
    {
        button.GetComponentInChildren<Text>().color = originalColor;
        hoverBackgroundImage.gameObject.SetActive(false);
    }

    // 播放按钮点击音效
    void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound, volume);  // 使用 volume 变量
        }
        else
        {
            Debug.LogError("按钮点击音效未设置！");
        }
    }

    void StartGame()
    {
        PlayButtonClickSound();
        Invoke("LoadLevel1Scene", 0.1f);
    }

    void LoadLevel1Scene()
    {
        SceneManager.LoadScene("Animation-Start");//先转动画场景，动画自动跳转第一关
    }

    void OpenDevelopmentTeamScene()
    {
        PlayButtonClickSound();
        Invoke("LoadDevelopmentTeamScene", 0.1f);
    }

    void LoadDevelopmentTeamScene()
    {
        SceneManager.LoadScene("Development Team");
    }

    void QuitGame()
    {
        Application.Quit();

        // 在Unity中测试
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // 打开设置图片
    void OpenSettings()
    {
        PlayButtonClickSound();
        if (settingsImage != null)
        {
            settingsImage.SetActive(true);
            if (volumeSlider != null)
            {
                volumeSlider.value = musicVolume;
            }
        }
    }

    // 关闭设置图片
    void CloseSettings()
    {
        PlayButtonClickSound();
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
        }
    }
}
