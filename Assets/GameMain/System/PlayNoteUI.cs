using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class PlayNoteUI : MonoBehaviour
{
    public Text score;
    public Text tip;
    public Text comboText;
    public Image comboImage;
    public Camera mainCamera;
    public Canvas effectCanvas;
    public RawImage distortionOverlay;
    public RawImage staticImage;
    public RawImage blackScreen;  // 新增：黑屏图片
    public AudioSource staticSoundEffect; // 添加花屏音效组件

    // 添加设置相关组件
    [Header("设置功能")]
    public Button settingsButton;
    public GameObject settingsImage;
    public Button settingsBackButton;
    public Slider volumeSlider;
    private float musicVolume = 1f;
    private const string VolumePrefsKey = "MusicVolume";

    [Header("音效")]
    public AudioClip settingsPanelSound;  // 设置面板打开/关闭音效
    private AudioSource audioSource;  // 音效播放器

    [Header("按键设置")]
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

    private StartGameSequence gameSequence;
    private BackgroundManager backgroundManager;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;
    private float musicStartTime;
    private float musicLength;
    private Material distortionMaterial;
    
    [Header("扭曲效果参数")]
    [Tooltip("扭曲强度，值越大扭曲越强，建议范围：30-100")]
    public float maxDistortion = 50f;
    
    [Tooltip("扭曲频率，值越大扭曲速度越快，建议范围：1-5")]
    public float distortionSpeed = 1f;
    
    [Tooltip("扭曲持续时间（秒）")]
    public float distortionDuration = 0.5f;

    private float distortionIntensity = 0f;
    private bool isDistorting = false;
    private bool halfwayDistortionTriggered = false;
    private bool threeQuartersDistortionTriggered = false;
    private float currentDistortionTime = 0f;

    [Header("花屏效果参数")]
    [Tooltip("每次闪烁的持续时间")]
    public float flashDuration = 0.1f;
    [Tooltip("两次闪烁之间的间隔时间")]
    public float flashInterval = 0.1f;

    private bool twoThirdFlashTriggered = false;  // 2/3处花屏触发标记

     [Header("暂停功能")]
    public Button pauseButton;
    public Button playButton;
    public GameObject countdownParent;
    public Image[] countdownImages;
    public GameObject notes;
    public AudioSource musicSource;

    // 将 isPaused 改为公共静态变量
    public static bool isPaused = false;

    void Start()
    {
        tip.text = "";  // 初始时隐藏提示文字
        comboText.text = "";
        comboImage.gameObject.SetActive(false);
        
        // 初始化花屏图片和黑屏
        if (staticImage != null)
        {
            staticImage.gameObject.SetActive(false);
        }
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }

        // 初始化音效
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;

        // 初始化设置功能
        InitializeSettings();

        // 获取 StartGameSequence 组件
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
        }

        // 获取 BackgroundManager 组件
        backgroundManager = FindObjectOfType<BackgroundManager>();
        if (backgroundManager == null)
        {
            Debug.LogError("BackgroundManager not found!");
        }

        // 初始化扭曲效果
        InitializeDistortionEffect();

        // 暂停功能初始化
        pauseButton.onClick.AddListener(Pause);
        playButton.onClick.AddListener(ResumeWithCountdown);
        playButton.gameObject.SetActive(false);
        countdownParent.SetActive(false);
    }

    private void InitializeDistortionEffect()
    {
        if (distortionOverlay != null)
        {
            // 创建新的材质实例，避免多个对象共享同一个材质
            Shader distortionShader = Shader.Find("Unlit/UIDistortion");
            if (distortionShader != null)
            {
                distortionMaterial = new Material(distortionShader);
                distortionOverlay.material = distortionMaterial;
                
                // 设置初始参数
                distortionMaterial.SetFloat("_DistortionAmount", 0);
                distortionMaterial.SetFloat("_DistortionSpeed", distortionSpeed);
                
                // 初始时隐藏
                distortionOverlay.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("找不到扭曲着色器！");
            }
        }
        else
        {
            Debug.LogError("Distortion overlay not assigned!");
        }
    }

    private void InitializeSettings()
    {
        // 初始化音量
        musicVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = musicVolume;
            volumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // 初始化按键设置
        InitializeKeySettings();

        // 初始化设置按钮
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => {
                OpenSettings();
            });
        }

        // 初始化返回按钮
        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveAllListeners();
            settingsBackButton.onClick.AddListener(() => {
                CloseSettings();
            });
        }

        // 初始化确认按钮监听
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => {
                SaveAndCloseSettings();
            });
        }

        // 初始化按键设置按钮
        if (upperKeyButton != null)
        {
            upperKeyButton.onClick.RemoveAllListeners();
            upperKeyButton.onClick.AddListener(() => {
                StartWaitingForUpperKey();
            });
        }

        if (lowerKeyButton != null)
        {
            lowerKeyButton.onClick.RemoveAllListeners();
            lowerKeyButton.onClick.AddListener(() => {
                StartWaitingForLowerKey();
            });
        }

        // 确保设置面板初始时是隐藏的
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
        }
    }

    private void InitializeKeySettings()
    {
        // 初始化按键值
        tempUpperKey = PlayerPrefs.GetString(UpperKeyPrefsKey, "J");
        tempLowerKey = PlayerPrefs.GetString(LowerKeyPrefsKey, "K");

        // 更新文本显示
        if (upperKeyText != null)
        {
            upperKeyText.text = tempUpperKey;
        }
        if (lowerKeyText != null)
        {
            lowerKeyText.text = tempLowerKey;
        }

        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(false);
        }
    }

    private void StartWaitingForUpperKey()
    {
        isWaitingForUpperKey = true;
        isWaitingForLowerKey = false;
        if (upperKeyText != null)
        {
            upperKeyText.text = "  ";
        }
    }

    private void StartWaitingForLowerKey()
    {
        isWaitingForLowerKey = true;
        isWaitingForUpperKey = false;
        if (lowerKeyText != null)
        {
            lowerKeyText.text = "  ";
        }
    }

    private void SetUpperKey(KeyCode keyCode)
    {
        if (upperKeyText != null)
        {
            tempUpperKey = keyCode.ToString();
            upperKeyText.text = tempUpperKey;
        }
        isWaitingForUpperKey = false;
    }

    private void SetLowerKey(KeyCode keyCode)
    {
        if (lowerKeyText != null)
        {
            tempLowerKey = keyCode.ToString();
            lowerKeyText.text = tempLowerKey;
        }
        isWaitingForLowerKey = false;
    }

    private void SaveAndCloseSettings()
    {
        // 保存按键设置
        PlayerPrefs.SetString(UpperKeyPrefsKey, tempUpperKey);
        PlayerPrefs.SetString(LowerKeyPrefsKey, tempLowerKey);
        PlayerPrefs.Save();

        // 显示保存成功提示
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(true);
            conflictText.text = "设置已保存！";
            StartCoroutine(HideConflictText(1f));
        }

        CloseSettings();
    }

    private IEnumerator HideConflictText(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (conflictText != null)
        {
            conflictText.gameObject.SetActive(false);
        }
    }

    private void OnMusicVolumeChanged(float newVolume)
    {
        musicVolume = newVolume;
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat(VolumePrefsKey, musicVolume);
        PlayerPrefs.Save();
    }

    private void PlaySettingsPanelSound()
    {
        if (settingsPanelSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(settingsPanelSound);
        }
    }

    private void OpenSettings()
    {
        isPaused = true;
        Time.timeScale = 0f;
        musicSource.Pause();
        notes.SetActive(false);

        if (settingsImage != null)
        {
            settingsImage.SetActive(true);
            PlaySettingsPanelSound();
            if (volumeSlider != null)
            {
                volumeSlider.value = musicVolume;
            }
        }
    }

    private void CloseSettings()
    {
        if (settingsImage != null)
        {
            settingsImage.SetActive(false);
            PlaySettingsPanelSound();
        }

        // 恢复游戏
        ResumeWithCountdown();
    }

    void Update()
    {
        score.text = PlayNoteModel.score.ToString();
        PlayNoteModel.UpdateTip();
        tip.text = PlayNoteModel.tip;

        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;
            
            if (backgroundManager != null)
            {
                backgroundManager.OnMusicStart();
                backgroundManager.SetMusicDuration(musicLength);
            }
        }

        // 检查音乐是否已经结束
        if (musicHasStarted && !musicHasEnded && !gameSequence.GameMusic.isPlaying)
        {
            musicHasEnded = true;
            HideCombo();
            
            // 音乐结束时，停止背景滚动
            if (backgroundManager != null)
            {
                backgroundManager.StopScrolling();
            }
        }

        // 检查特定时间点触发效果
        if (musicHasStarted && !musicHasEnded && gameSequence != null && gameSequence.GameMusic.isPlaying)
        {
            float currentMusicTime = Time.time - musicStartTime;
            float twoFifthsPoint = musicLength * 0.4f;  // 2/5处
            float fourFifthsPoint = musicLength * 0.8f; // 4/5处
            
            // 在2/5处触发扭曲
            if (!halfwayDistortionTriggered && currentMusicTime >= twoFifthsPoint)
            {
                StartDistortion();
                halfwayDistortionTriggered = true;
                Debug.Log("触发2/5处扭曲效果");
            }
            
            // 在4/5处触发扭曲和花屏
            if (!threeQuartersDistortionTriggered && currentMusicTime >= fourFifthsPoint)
            {
                StartDistortion();
                threeQuartersDistortionTriggered = true;
                Debug.Log("触发4/5处扭曲效果");
            }
        }

        // 更新扭曲效果
        UpdateDistortionEffect();

        // 只在音乐播放时更新 combo 显示
        if (musicHasStarted && !musicHasEnded)
        {
            UpdateComboDisplay();
        }

        // 检查按键设置输入
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

    private void UpdateDistortionEffect()
    {
        if (isDistorting && distortionMaterial != null)
        {
            currentDistortionTime += Time.deltaTime;
            if (currentDistortionTime >= distortionDuration)
            {
                StopDistortion();
                return;
            }

            // 使用正弦波制造波动效果，增加扭曲的明显程度
            float wave = Mathf.Sin(Time.time * distortionSpeed * 10f);
            distortionIntensity = (Mathf.Abs(wave) * 0.8f + 0.2f) * maxDistortion; // 确保始终有一定强度的扭曲
            distortionMaterial.SetFloat("_DistortionAmount", distortionIntensity);
            //Debug.Log($"当前扭曲强度: {distortionIntensity}");
        }
    }

    private void StartDistortion()
    {
        if (!isDistorting && distortionMaterial != null)
        {
            isDistorting = true;
            currentDistortionTime = 0f;
            // 激活 RawImage
            distortionOverlay.gameObject.SetActive(true);
            Debug.Log("开始扭曲效果");
        }
    }

    private void StopDistortion()
    {
        if (isDistorting && distortionMaterial != null)
        {
            isDistorting = false;
            distortionIntensity = 0f;
            distortionMaterial.SetFloat("_DistortionAmount", 0);
            distortionOverlay.gameObject.SetActive(false);
            
            // 只在最后一次扭曲（4/5处）结束时触发花屏效果
            if (threeQuartersDistortionTriggered)
            {
                StartCoroutine(DelayedFlashEffect());
            }
            
            Debug.Log("结束扭曲效果");
        }
    }

    private IEnumerator DelayedFlashEffect()
    {
        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);
        Debug.Log("开始花屏闪烁");
        StartCoroutine(FlashStaticEffect());
    }

    private IEnumerator FlashStaticEffect()
    {
        if (staticImage != null)
        {
            // 第一次闪烁
            staticImage.gameObject.SetActive(true);
            if (staticSoundEffect != null)
            {
                staticSoundEffect.Play();
            }
            yield return new WaitForSeconds(flashDuration);
            staticImage.gameObject.SetActive(false);
            if (staticSoundEffect != null)
            {
                staticSoundEffect.Stop();
            }
            
            // 等待间隔
            yield return new WaitForSeconds(flashInterval);
            
            // 第二次闪烁
            staticImage.gameObject.SetActive(true);
            if (staticSoundEffect != null)
            {
                staticSoundEffect.Play();
            }
            yield return new WaitForSeconds(flashDuration);
            staticImage.gameObject.SetActive(false);
            if (staticSoundEffect != null)
            {
                staticSoundEffect.Stop();
            }

            // 如果是最后一次花屏（4/5处的扭曲后），添加黑屏效果
            if (threeQuartersDistortionTriggered)
            {
                // 等待0.5秒
                yield return new WaitForSeconds(0.5f);
                
                // 显示黑屏0.5秒
                if (blackScreen != null)
                {
                    blackScreen.gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.3f);
                    blackScreen.gameObject.SetActive(false);
                }
            }
        }
    }

    // 更新 combo 数
    private void UpdateComboDisplay()
    {
        int currentCombo = PlayNoteModel.GetCombo();
        if (currentCombo > 0)
        {
            comboText.text = " " + currentCombo.ToString();
            comboImage.gameObject.SetActive(true);
        }
        else
        {
            comboText.text = "";  //  Combo 为 0 时隐藏显示
            comboImage.gameObject.SetActive(false);
        }
    }

    private void HideCombo()
    {
        comboText.text = "";
        comboImage.gameObject.SetActive(false);
    }
    // 暂停方法
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        musicSource.Pause();  // 可替换为 gameSequence.GameMusic.Pause();
        notes.SetActive(false);

        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    // 恢复播放，带倒计时
    public void ResumeWithCountdown()
    {
        countdownParent.SetActive(true);
        ResetCountdownImages();
        StartCoroutine(ResumeAfterCountdown(3));
    }

    private IEnumerator ResumeAfterCountdown(int countdown)
    {
        for (int i = 0; i < countdownImages.Length; i++)
        {
            countdownImages[i].gameObject.SetActive(false);
        }

        while (countdown > 0)
        {
            if (countdown - 1 < countdownImages.Length)
            {
                for (int i = 0; i < countdownImages.Length; i++)
                {
                    countdownImages[i].gameObject.SetActive(false);
                }
                countdownImages[countdown - 1].gameObject.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(1);
            countdown--;
        }

        ResetCountdownImages();
        countdownParent.SetActive(false);

        Time.timeScale = 1f;
        musicSource.Play();
        notes.SetActive(true);
        isPaused = false;

        // 显式恢复背景滚动
        BackgroundManager bgManager = FindObjectOfType<BackgroundManager>();
        if (bgManager != null)
        {
            bgManager.ResumeScrolling();
        }

        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    // 重置倒计时图像
    private void ResetCountdownImages()
    {
        for (int i = 0; i < countdownImages.Length; i++)
        {
            countdownImages[i].gameObject.SetActive(false);
        }
    }
}
