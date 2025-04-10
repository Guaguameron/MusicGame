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

    public AudioClip buttonHoverSound; // 添加悬停音效

    void Start()
    {
        Debug.Log("开始初始化StartMenuController");
        
        // 1. 初始化音频
        InitializeAudio();
        
        // 2. 初始化颜色
        InitializeColors();

        // 3. 初始化按键设置（提前初始化按键文本）
        InitializeKeySettings();
        
        // 4. 初始化主菜单按钮
        InitializeMainMenuButtons();
        
        // 5. 初始化设置面板
        InitializeSettingsPanel();
    }

    private void InitializeAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;  // 设置按钮音效的音量

        // 初始化音量滑块
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners(); // 清除现有的监听器
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

    private void InitializeColors()
    {
        if (ColorUtility.TryParseHtmlString(hoverColorHex, out hoverColor))
        {
            // Debug.Log("成功将十六进制颜色转换为 Color 类型");
        }
        else
        {
            // Debug.LogError("十六进制颜色转换失败，请检查格式");
        }

        // 设置所有按钮文字的初始颜色为白色
        originalColor = Color.white;
        SetInitialButtonTextColor(startButton);
        SetInitialButtonTextColor(continueButton);
        SetInitialButtonTextColor(developmentTeamButton);
        SetInitialButtonTextColor(exitButton);
        SetInitialButtonTextColor(settingsButton);

        hoverBackgroundImage.gameObject.SetActive(false);
    }

    private void InitializeKeySettings()
    {
        Debug.Log("初始化按键设置");
        
        // 初始化按键值
        tempUpperKey = PlayerPrefs.GetString(UpperKeyPrefsKey, "J");
        tempLowerKey = PlayerPrefs.GetString(LowerKeyPrefsKey, "K");

        // 立即更新文本显示
        if (upperKeyText != null)
        {
            upperKeyText.text = tempUpperKey;
            Debug.Log($"上层按键文本已设置为: {tempUpperKey}");
        }
        else
        {
            Debug.LogError("upperKeyText 未赋值！");
        }

        if (lowerKeyText != null)
        {
            lowerKeyText.text = tempLowerKey;
            Debug.Log($"下层按键文本已设置为: {tempLowerKey}");
        }
        else
        {
            Debug.LogError("lowerKeyText 未赋值！");
        }

        // 设置按钮监听器
        if (upperKeyButton != null)
        {
            upperKeyButton.onClick.RemoveAllListeners();
            upperKeyButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                StartWaitingForUpperKey();
            });
        }

        if (lowerKeyButton != null)
        {
            lowerKeyButton.onClick.RemoveAllListeners();
            lowerKeyButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                StartWaitingForLowerKey();
            });
        }

        // 设置确认按钮
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                SaveAndCloseSettings();
            });
        }
    }

    private void InitializeMainMenuButtons()
    {
        Debug.Log("初始化主菜单按钮");

        // 设置按钮点击事件
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                StartGame();
            });
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                ContinueGame();
            });
        }

        if (developmentTeamButton != null)
        {
            developmentTeamButton.onClick.RemoveAllListeners();
            developmentTeamButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                OpenDevelopmentTeamScene();
            });
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                QuitGame();
            });
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => {
                PlayButtonClickSound();
                OpenSettings();
            });
        }

        // 添加悬停效果
        SafeAddHoverEffect(startButton);
        SafeAddHoverEffect(continueButton);
        SafeAddHoverEffect(developmentTeamButton);
        SafeAddHoverEffect(exitButton);
        SafeAddHoverEffect(settingsButton);
    }

    private void InitializeSettingsPanel()
    {
        // 为设置图片中的返回按钮添加点击事件
        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveAllListeners();
            settingsBackButton.onClick.AddListener(() => {
                SaveKeySettings(); // 先保存设置
                PlayButtonClickSound();
                CloseSettings(); // 然后关闭面板
            });
        }

        // 确保设置图片初始时是隐藏的
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
        }
    }

    void OnMusicVolumeChanged(float newVolume)
    {
        Debug.Log($"音量改变: {newVolume}"); // 添加调试日志
        musicVolume = newVolume;
        ApplyMusicVolumeSettings();
        PlayerPrefs.SetFloat(VolumePrefsKey, musicVolume);
        PlayerPrefs.Save();
    }

    void ApplyMusicVolumeSettings()
    {
        if (backgroundMusicSource != null)
        {
            Debug.Log($"应用音量设置: {musicVolume}"); // 添加调试日志
            backgroundMusicSource.volume = musicVolume;
        }
    }

    // 修改悬停效果方法
    private void SafeAddHoverEffect(Button button)
    {
        if (button == null) return;

        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText == null) return;

        // 设置初始颜色
        buttonText.color = originalColor;

        // 添加事件触发器
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // 清除现有的触发器
        trigger.triggers.Clear();

        // 添加进入事件
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            // 先播放音效
            if (buttonHoverSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buttonHoverSound, volume);
            }

            // 再改变颜色和背景
            buttonText.color = hoverColor;
            if (hoverBackgroundImage != null)
            {
                hoverBackgroundImage.gameObject.SetActive(true);
                hoverBackgroundImage.transform.SetParent(button.transform, false);
                hoverBackgroundImage.transform.SetSiblingIndex(0);
                
                RectTransform hoverRect = hoverBackgroundImage.rectTransform;
                hoverRect.anchorMin = Vector2.zero;
                hoverRect.anchorMax = Vector2.one;
                hoverRect.offsetMin = Vector2.zero;
                hoverRect.offsetMax = Vector2.zero;
            }
        });
        trigger.triggers.Add(enterEntry);

        // 添加退出事件
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            buttonText.color = originalColor;
            if (hoverBackgroundImage != null)
            {
                hoverBackgroundImage.transform.SetParent(transform);
                hoverBackgroundImage.gameObject.SetActive(false);
            }
        });
        trigger.triggers.Add(exitEntry);
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
            // 隐藏设置按钮
            if (settingsButton != null)
            {
                settingsButton.gameObject.SetActive(false);
            }
        }
    }

    // 关闭设置图片
    void CloseSettings()
    {
        Debug.Log("正在关闭设置面板");
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
            
            // 显示设置按钮
            if (settingsButton != null)
            {
                settingsButton.gameObject.SetActive(true);
            }
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

    // 新增保存并关闭设置的方法
    private void SaveAndCloseSettings()
    {
        // 保存设置
        PlayerPrefs.SetString(UpperKeyPrefsKey, tempUpperKey);
        PlayerPrefs.SetString(LowerKeyPrefsKey, tempLowerKey);
        PlayerPrefs.Save();
        
        // 直接关闭设置面板
        CloseSettings();
    }

    // 修改按键等待方法
    void StartWaitingForUpperKey()
    {
        isWaitingForUpperKey = true;
        isWaitingForLowerKey = false;
        if (upperKeyText != null)
        {
            upperKeyText.text = "  ";
        }
    }

    void StartWaitingForLowerKey()
    {
        isWaitingForLowerKey = true;
        isWaitingForUpperKey = false;
        if (lowerKeyText != null)
        {
            lowerKeyText.text = "  ";
        }
    }

    // 修改设置按键方法
    void SetUpperKey(KeyCode keyCode)
    {
        if (upperKeyText != null)
        {
            tempUpperKey = keyCode.ToString();
            upperKeyText.text = tempUpperKey;
            Debug.Log($"设置上层按键为: {tempUpperKey}");
        }
        isWaitingForUpperKey = false;
    }

    void SetLowerKey(KeyCode keyCode)
    {
        if (lowerKeyText != null)
        {
            tempLowerKey = keyCode.ToString();
            lowerKeyText.text = tempLowerKey;
            Debug.Log($"设置下层按键为: {tempLowerKey}");
        }
        isWaitingForLowerKey = false;
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
    }

    IEnumerator HideConflictText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(false);
        }
    }

    // 添加新的辅助方法
    private void SetInitialButtonTextColor(Button button)
    {
        if (button != null)
        {
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = originalColor;
            }
        }
    }
}
