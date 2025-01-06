using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;  

public class StartMenuController : MonoBehaviour
{
    public Button startButton;
    public Button continueButton;
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

    // 添加按键设置相关的UI元素
    [Header("Key Settings")]
    public Text upperKeyText;     // 显示上层按键的文本
    public Text lowerKeyText;     // 显示下层按键的文本
    public Button upperKeyButton; // 更改上层按键的按钮
    public Button lowerKeyButton; // 更改下层按键的按钮
    public Text conflictText;     // 用于显示冲突提示的文本框
    public Button confirmButton;  // 确定按钮
    
    private bool isWaitingForUpperKey = false;
    private bool isWaitingForLowerKey = false;
    
    private const string UpperKeyPrefsKey = "UpperKey";
    private const string LowerKeyPrefsKey = "LowerKey";

    private string tempUpperKey;
    private string tempLowerKey;

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
        AddHoverEffect(continueButton, ContinueGame);
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

        // 初始化按键设置
        LoadKeySettings();
        
        // 添加按键设置按钮的监听器
        if (upperKeyButton != null)
        {
            upperKeyButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                StartWaitingForUpperKey();
            });
        }
        
        if (lowerKeyButton != null)
        {
            lowerKeyButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                StartWaitingForLowerKey();
            });
        }

        // 确保冲突提示文本初始时是隐藏的
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(false);
        }

        // 初始化临时按键值
        tempUpperKey = PlayerPrefs.GetString(UpperKeyPrefsKey, "J");
        tempLowerKey = PlayerPrefs.GetString(LowerKeyPrefsKey, "K");
        
        // 添加确定按钮的监听器
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                SaveKeySettings();
            });
        }
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

        // 修改这部分代码
        hoverBackgroundImage.gameObject.SetActive(true);
        
        // 使用 SetParent 将 hoverBackgroundImage 设置为按钮的子对象
        hoverBackgroundImage.transform.SetParent(button.transform, false);
        
        // 设置 RectTransform
        RectTransform hoverRect = hoverBackgroundImage.rectTransform;
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        
        // 重置位置和大小
        hoverRect.anchorMin = Vector2.zero;
        hoverRect.anchorMax = Vector2.one;
        hoverRect.offsetMin = Vector2.zero;
        hoverRect.offsetMax = Vector2.zero;
        
        // 确保在按钮文字的后面
        hoverBackgroundImage.transform.SetSiblingIndex(0);
    }

    // 鼠标移开时恢复原来的字体颜色并隐藏背景图片
    void OnHoverExit(Button button)
    {
        button.GetComponentInChildren<Text>().color = originalColor;
        // 移回原来的父对象（通常是 Canvas）
        hoverBackgroundImage.transform.SetParent(transform, false);
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

    void ContinueGame()
    {
        PlayButtonClickSound();
        string lastScene = PlayerPrefs.GetString("LastScene", "");
        if (!string.IsNullOrEmpty(lastScene) && lastScene != SceneManager.GetActiveScene().name)
        {
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Invoke("LoadLevel1Scene", 0.1f);
        }
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

    void Update()
    {
        if (isWaitingForUpperKey || isWaitingForLowerKey)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (isWaitingForUpperKey)
                    {
                        SetUpperKey(keyCode);
                    }
                    else if (isWaitingForLowerKey)
                    {
                        SetLowerKey(keyCode);
                    }
                    break;
                }
            }
        }
    }

    void LoadKeySettings()
    {
        string upperKey = PlayerPrefs.GetString(UpperKeyPrefsKey, "J");
        string lowerKey = PlayerPrefs.GetString(LowerKeyPrefsKey, "K");
        
        UpdateKeyDisplayText(upperKeyText, upperKey);
        UpdateKeyDisplayText(lowerKeyText, lowerKey);
    }
    
    void UpdateKeyDisplayText(Text textComponent, string keyName)
    {
        if (textComponent != null)
        {
            textComponent.text = keyName;  // 直接显示按键名称
        }
    }
    
    void StartWaitingForUpperKey()
    {
        isWaitingForUpperKey = true;
        isWaitingForLowerKey = false;
        if (upperKeyText != null)
        {
            upperKeyText.text = "";  // 清空文本
        }
    }
    
    void StartWaitingForLowerKey()
    {
        isWaitingForLowerKey = true;
        isWaitingForUpperKey = false;
        if (lowerKeyText != null)
        {
            lowerKeyText.text = "";  // 清空文本
        }
    }
    
    void SetUpperKey(KeyCode keyCode)
    {
        string currentLowerKey = tempLowerKey;  // 使用临时存储的值
        
        if (keyCode.ToString() != currentLowerKey)
        {
            tempUpperKey = keyCode.ToString();  // 存储到临时变量
            upperKeyText.text = keyCode.ToString();
            
            if (conflictText != null)
            {
                conflictText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (conflictText != null)
            {
                conflictText.gameObject.SetActive(true);
                conflictText.text = "按键冲突！";
                StartCoroutine(HideConflictText(1f));
            }
            upperKeyText.text = tempUpperKey;
        }
        
        isWaitingForUpperKey = false;
    }
    
    void SetLowerKey(KeyCode keyCode)
    {
        string currentUpperKey = tempUpperKey;  // 使用临时存储的值
        
        if (keyCode.ToString() != currentUpperKey)
        {
            tempLowerKey = keyCode.ToString();  // 存储到临时变量
            lowerKeyText.text = keyCode.ToString();
            
            if (conflictText != null)
            {
                conflictText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (conflictText != null)
            {
                conflictText.gameObject.SetActive(true);
                conflictText.text = "按键冲突！";
                StartCoroutine(HideConflictText(1f));
            }
            lowerKeyText.text = tempLowerKey;
        }
        
        isWaitingForLowerKey = false;
    }
    
    IEnumerator HideConflictText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(false);
        }
    }

    // 保存按键设置的方法
    void SaveKeySettings()
    {
        // 保存设置到 PlayerPrefs
        PlayerPrefs.SetString(UpperKeyPrefsKey, tempUpperKey);
        PlayerPrefs.SetString(LowerKeyPrefsKey, tempLowerKey);
        PlayerPrefs.Save();
        
        // 可以添加保存成功的提示
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(true);
            conflictText.text = "设置已保存！";
            StartCoroutine(HideConflictText(1f));
        }
        
        // 关闭设置面板
        CloseSettings();
    }
}
