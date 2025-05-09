using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Puzzle2 : MonoBehaviour
{
    // 四个不同的预设体
    public GameObject circlePrefab;
    public GameObject crossesPrefab;
    public GameObject cubePrefab;
    public GameObject trianglePrefab;
    public GameObject spiderPrefab;
    // 四个按钮
    public Button circleButton;
    public Button crossesButton;
    public Button cubeButton;
    public Button triangleButton;
    public Button SpiderButton;
    public Button playButton;  // 公共的Play_Button
    public Button soundButton;

    public Canvas canvas;  // 公有的Canvas
    public Transform Judgment_Point1;  // 判断点1
    public Transform Judgment_Point2;  // 判断点2
    public float moveSpeed = 50f;  // 公有的速度，控制预设体的移动速度

    private List<GameObject> spawnedPrefabs = new List<GameObject>();  // 存储所有生成的预设体
    private bool isButtonHeld = false;
    private bool isMoving = false;  // 用来判断预设体是否开始移动
    public GameObject heldPrefab;  // 存储当前按下的 prefab
    private Transform playSpace;  // Play_Space的引用
    private Camera canvasCamera;  // Canvas上的Camera引用

    public AudioClip musicClip; // 要播放的音乐片段
    private GameObject selectedPrefab;  // 当前选择的预设体
    public AudioSource audioSource; // 公共 AudioSource
    public AudioClip music1; // MP3 音频剪辑 1
    public AudioClip music2; // MP3 音频剪辑 2
    public AudioClip music3; // MP3 音频剪辑 3
    public AudioClip music4; // MP3 音频剪辑 4
    public AudioClip music5; // MP3 音频剪辑 5

    public Button HitButton;
    public GameObject HitPage;
    public GameObject[] PositionPoint;
    
    public GameObject restartText; // 重启提示文本对象
    public GameObject newImage;  // 图片引用
    public Button ReButton;      //  ReButton 引用
    public Button closeButton;   // 关闭按钮引用

    public GameObject successText; // 成功提示文本对象

    private Dictionary<GameObject, Coroutine> checkCoroutines = new Dictionary<GameObject, Coroutine>();

    public static bool IsPuzzleSolved = false; // 用于跟踪解谜是否成功

    public Button skipButton; // 跳过游戏按钮

    public GameObject skipImage; // 跳过时显示的图片

    public Button okButton; // OK 按钮
    public Button noButton; // NO 按钮

    void Start()
    {
        // 获取Canvas的Camera
        canvasCamera = canvas.worldCamera;

        // 获取Play_Space的引用
        playSpace = canvas.transform.Find("Play_Space");
        if (playSpace == null)
        {
            Debug.LogError("未找到Play_Space，请确保Play_Space是Canvas的子物体");
        }

        // 为四个按钮添加事件监听器
        AddEventTriggerListener(circleButton.gameObject, EventTriggerType.PointerDown, (BaseEventData data) => OnPointerDown(data, circlePrefab));
        AddEventTriggerListener(crossesButton.gameObject, EventTriggerType.PointerDown, (BaseEventData data) => OnPointerDown(data, crossesPrefab));
        AddEventTriggerListener(cubeButton.gameObject, EventTriggerType.PointerDown, (BaseEventData data) => OnPointerDown(data, cubePrefab));
        AddEventTriggerListener(triangleButton.gameObject, EventTriggerType.PointerDown, (BaseEventData data) => OnPointerDown(data, trianglePrefab));
        AddEventTriggerListener(SpiderButton.gameObject, EventTriggerType.PointerDown, (BaseEventData data) => OnPointerDown(data, spiderPrefab));

        AddEventTriggerListener(circleButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(crossesButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(cubeButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(triangleButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(SpiderButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        soundButton.onClick.AddListener(OnSoundButtonClick);

        // 为Play_Button添加click事件监听器
        playButton.onClick.AddListener(OnPlayButtonClick);

        // 添加关闭按钮的点击事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePage);
        }

        // 为跳过按钮添加点击事件
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipGame);
        }

        // 为 OK 和 NO 按钮添加点击事件
        if (okButton != null)
        {
            okButton.onClick.AddListener(() => SceneManager.LoadScene("Memory")); // 替换为实际的下一场景名称
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(CloseSkipImage);
        }

        // 添加点击空白处关闭 skipImage 的事件
        if (skipImage != null)
        {
            EventTrigger trigger = skipImage.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = skipImage.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => CloseSkipImage());
            trigger.triggers.Add(entry);
        }
    }

    void Update()
    {
        // 调用函数处理生成的预设体跟随鼠标移动
        HandlePrefabMovement();

        // 如果预设体开始移动，按给定的速度向左移动
        if (isMoving)
        {
            MovePrefabs();
        }
    }

    // 处理预设体跟随鼠标移动的逻辑
    private void HandlePrefabMovement()
    {
        if (isButtonHeld && heldPrefab != null)
        {
            Vector2 mousePosition = Input.mousePosition;

            if (canvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mousePosition,
                    canvasCamera,  // 使用Canvas的Camera
                    out Vector2 localPoint);

                // 将按下的预设体跟随鼠标移动
                RectTransform prefabRect = heldPrefab.GetComponent<RectTransform>();
                prefabRect.anchoredPosition = localPoint;

                // 检查与 PositionPoint 中的物体距离
                foreach (GameObject positionPoint in PositionPoint)
                {
                    RectTransform pointRect = positionPoint.GetComponent<RectTransform>();
                    if (Vector2.Distance(prefabRect.anchoredPosition, pointRect.anchoredPosition) < 20f)
                    {
                        // 检查该位置是否已经有其他音符
                        bool positionOccupied = false;
                        foreach (GameObject existingPrefab in spawnedPrefabs)
                        {
                            if (existingPrefab != heldPrefab) // 不与自己比较
                            {
                                RectTransform existingRect = existingPrefab.GetComponent<RectTransform>();
                                if (Vector2.Distance(existingRect.anchoredPosition, pointRect.anchoredPosition) < 5f)
                                {
                                    positionOccupied = true;
                                    break;
                                }
                            }
                        }

                        // 只有当位置未被占用时才吸附
                        if (!positionOccupied)
                        {
                            prefabRect.anchoredPosition = pointRect.anchoredPosition;
                        }
                        break;
                    }
                }
            }
        }
    }

    // 处理按钮按下时生成预设体
    private void OnPointerDown(BaseEventData data, GameObject prefab)
    {
        if (prefab != null && playSpace != null)
        {
            selectedPrefab = prefab;

            // 生成的物体作为Play_Space的子物体
            GameObject spawnedPrefab = Instantiate(prefab, playSpace);
            spawnedPrefabs.Add(spawnedPrefab);

            // 为生成的预设体添加事件监听器
            AddEventTriggerListener(spawnedPrefab, EventTriggerType.PointerDown, (BaseEventData d) => OnPrefabPointerDown(d, spawnedPrefab));
            AddEventTriggerListener(spawnedPrefab, EventTriggerType.PointerUp, OnPrefabPointerUp);

            isButtonHeld = true;
            heldPrefab = spawnedPrefab; // 记录当前按下的 prefab

            // 存储协程引用
            Coroutine checkCoroutine = StartCoroutine(CheckNoteSnapping(spawnedPrefab));
            checkCoroutines[spawnedPrefab] = checkCoroutine;

            // 播放对应音乐
            if (prefab.name == circlePrefab.name)
                PlayMusic1();
            else if (prefab.name == crossesPrefab.name)
                PlayMusic2();
            else if (prefab.name == cubePrefab.name)
                PlayMusic3();
            else if (prefab.name == trianglePrefab.name)
                PlayMusic4();
            else if (prefab.name == spiderPrefab.name)
                PlayMusic5();
        }
    }

    // 添加新的协程来检查音符是否被吸附
    private IEnumerator CheckNoteSnapping(GameObject note)
    {
        yield return new WaitForSeconds(2f); // 等待2秒

        if (note != null && spawnedPrefabs.Contains(note))
        {
            bool isSnapped = false;
            RectTransform noteRect = note.GetComponent<RectTransform>();

            // 检查音符是否被吸附到任何位置点
            foreach (GameObject positionPoint in PositionPoint)
            {
                RectTransform pointRect = positionPoint.GetComponent<RectTransform>();
                if (Vector2.Distance(noteRect.anchoredPosition, pointRect.anchoredPosition) < 5f)
                {
                    isSnapped = true;
                    break;
                }
            }

            // 如果没有被吸附，销毁音符
            if (!isSnapped)
            {
                spawnedPrefabs.Remove(note);
                Destroy(note);
            }
        }
    }

    // 处理预设体按下时跟随鼠标移动
    private void OnPrefabPointerDown(BaseEventData data, GameObject prefab)
    {
        isButtonHeld = true;
        heldPrefab = prefab; // 直接更新为按下的 prefab
        if (prefab.name == circlePrefab.name + "(Clone)")
            PlayMusic1();
        else if (prefab.name == crossesPrefab.name + "(Clone)")
            PlayMusic2();
        else if (prefab.name == cubePrefab.name + "(Clone)")
            PlayMusic3();
        else if (prefab.name == trianglePrefab.name + "(Clone)")
            PlayMusic4();
        else if (prefab.name == spiderPrefab.name + "(Clone)")
            PlayMusic5();

        Debug.Log("11111");
    }

    // 处理预设体松开时停止移动
    private void OnPrefabPointerUp(BaseEventData data)
    {
        isButtonHeld = false;
        heldPrefab = null;
    }

    // Play_Button点击事件，开始移动所有预设体
    private void OnPlayButtonClick()
    {
        if (spawnedPrefabs.Count > 0)
        {
            // 停止所有正在运行的检查协程
            foreach (var pair in checkCoroutines)
            {
                if (pair.Value != null)
                {
                    StopCoroutine(pair.Value);
                }
            }
            checkCoroutines.Clear();

            // 立即检查并删除所有未吸附的音符
            List<GameObject> notesToRemove = new List<GameObject>();
            
            foreach (GameObject note in spawnedPrefabs)
            {
                bool isSnapped = false;
                RectTransform noteRect = note.GetComponent<RectTransform>();

                // 检查音符是否被吸附到任何位置点
                foreach (GameObject positionPoint in PositionPoint)
                {
                    RectTransform pointRect = positionPoint.GetComponent<RectTransform>();
                    if (Vector2.Distance(noteRect.anchoredPosition, pointRect.anchoredPosition) < 5f)
                    {
                        isSnapped = true;
                        break;
                    }
                }

                // 如果没有被吸附，加入待删除列表
                if (!isSnapped)
                {
                    notesToRemove.Add(note);
                }
            }

            // 立即删除所有未吸附的音符
            foreach (GameObject note in notesToRemove)
            {
                spawnedPrefabs.Remove(note);
                Destroy(note);
            }

            // 如果还有剩余的音符（被吸附的），开始移动
            if (spawnedPrefabs.Count > 0)
            {
                isMoving = true;  // 开始移动
            }
        }
    }

    // 移动所有预设体并检查是否靠近Judgment_Point1或Judgment_Point2
    private void MovePrefabs()
    {
        List<GameObject> prefabsToRemove = new List<GameObject>();

        foreach (GameObject prefab in spawnedPrefabs)
        {
            RectTransform prefabRect = prefab.GetComponent<RectTransform>();
            prefabRect.anchoredPosition += new Vector2(-moveSpeed * Time.deltaTime, 0);

            // 检查是否碰到 Judgment_Point1 或 Judgment_Point2
            if (Vector2.Distance(prefabRect.anchoredPosition, Judgment_Point1.localPosition) < 1f)
            {
                StartCoroutine(ScaleAndFadeThenDestroy(prefab, 1));
                prefabsToRemove.Add(prefab);
            }
            else if (Vector2.Distance(prefabRect.anchoredPosition, Judgment_Point2.localPosition) < 1f)
            {
                StartCoroutine(ScaleAndFadeThenDestroy(prefab, 2));
                prefabsToRemove.Add(prefab);
            }
        }

        foreach (GameObject prefab in prefabsToRemove)
        {
            spawnedPrefabs.Remove(prefab);
        }

        if (spawnedPrefabs.Count == 0)
        {
            isMoving = false;
        }
    }

    private List<int> destructionSequence = new List<int>();
    private string winSequence = "1234123212341255"; // 通关序列的数字字符串

    private IEnumerator ScaleAndFadeThenDestroy(GameObject target, int judgmentPoint)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        Image targetImage = target.GetComponent<Image>();
        Color originalColor = targetImage.color;
        Vector3 originalScale = rectTransform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(1f, 1.3f, elapsedTime / duration);
            rectTransform.localScale = originalScale * scaleFactor;

            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            targetImage.color = newColor;

            yield return null;
        }

        UpdateCounter(target, judgmentPoint);
        Destroy(target);
    }

    private void UpdateCounter(GameObject target, int judgmentPoint)
    {
        string prefabName = target.name.Replace("(Clone)", "").Trim();
        int destructionCode = 0;

        if (prefabName == circlePrefab.name)
        {
            destructionCode = 1;
            HandleJudgmentPoint(judgmentPoint, ref GlobalCounters.Judgment_Point1_Circle, ref GlobalCounters.Judgment_Point2_Circle);
            PlayMusic1();
        }
        else if (prefabName == crossesPrefab.name)
        {
            destructionCode = 2;
            HandleJudgmentPoint(judgmentPoint, ref GlobalCounters.Judgment_Point1_Crosses, ref GlobalCounters.Judgment_Point2_Crosses);
            PlayMusic2();
        }
        else if (prefabName == cubePrefab.name)
        {
            destructionCode = 3;
            HandleJudgmentPoint(judgmentPoint, ref GlobalCounters.Judgment_Point1_Cube, ref GlobalCounters.Judgment_Point2_Cube);
            PlayMusic3();
        }
        else if (prefabName == trianglePrefab.name)
        {
            destructionCode = 4;
            HandleJudgmentPoint(judgmentPoint, ref GlobalCounters.Judgment_Point1_Triangle, ref GlobalCounters.Judgment_Point2_Triangle);
            PlayMusic4();
        }
        else if (prefabName == spiderPrefab.name)
        {
            destructionCode = 5;
            HandleJudgmentPoint(judgmentPoint, ref GlobalCounters.Judgment_Point1_spider, ref GlobalCounters.Judgment_Point2_spider);
            PlayMusic5();
        }

        if (destructionCode != 0)
        {
            destructionSequence.Add(destructionCode);
            CheckWinCondition();
        }
    }

    private void HandleJudgmentPoint(int judgmentPoint, ref int point1Counter, ref int point2Counter)
    {
        if (judgmentPoint == 1)
        {
            point1Counter++;
        }
        else if (judgmentPoint == 2)
        {
            point2Counter++;
        }
    }

    private void CheckWinCondition()
    {
        string currentSequence = string.Join("", destructionSequence);
        if (currentSequence.Contains(winSequence))
        {
            Debug.Log("通关！");
            IsPuzzleSolved = true; // 设置解谜成功
            GlobalCounters.PuzzleSuccessCount++;
            // 增加10000分
            PlayNoteModel.score += 10000;

            // 显示成功提示文本并开始动画
            if (successText != null)
            {
                successText.SetActive(true);
                StartCoroutine(SuccessTextAnimation());
            }

            StartCoroutine(WaitForAudioAndLoadScene());
        }
        else
        {
            GlobalCounters.PuzzleFailCount++;
        }
    }

    // 等待音频播放完成后加载场景
    private IEnumerator WaitForAudioAndLoadScene()
    {
        // 等待当前音效播放完成
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        
        // 添加一个小延迟，确保体验流畅
        yield return new WaitForSeconds(0.5f);
        
        // 加载下一个场景
        SceneManager.LoadScene("Memory");
    }

    private void AddEventTriggerListener(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
    public void ReloadCurrentScene()
    {
        // 显示重启提示文本并开始动画
        if (restartText != null)
        {
            restartText.SetActive(true);
            StartCoroutine(RestartTextAnimation());
        }
        else
        {
            // 如果没有文本对象，直接重新加载场景
            ExecuteReload();
        }
    }

    // 新添加的文本动画协程
    private IEnumerator RestartTextAnimation()
    {
        float duration = 1f; // 动画持续时间
        float elapsedTime = 0f;
        
        RectTransform textRect = restartText.GetComponent<RectTransform>();
        Text textComponent = restartText.GetComponent<Text>();
        
        if (textRect == null || textComponent == null)
        {
            yield return new WaitForSeconds(1f);
            ExecuteReload();
            yield break;
        }

        Vector3 originalScale = textRect.localScale;
        Color originalColor = textComponent.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // 缓动效果
            float easeProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            
            // 放大效果 (从1倍到1.5倍)
            float scaleMultiplier = Mathf.Lerp(1f, 1.5f, easeProgress);
            textRect.localScale = originalScale * scaleMultiplier;
            
            // 透明度渐变 (从1到0)
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, easeProgress);
            textComponent.color = newColor;
            
            yield return null;
        }

        // 确保最终状态
        textRect.localScale = originalScale * 1.5f;
        Color finalColor = originalColor;
        finalColor.a = 0f;
        textComponent.color = finalColor;
        
        // 重置文本属性
        textRect.localScale = originalScale;
        textComponent.color = originalColor;
        
        restartText.SetActive(false);
        
        // 执行场景重载
        ExecuteReload();
    }

    // 新添加的成功提示文本动画协程
    private IEnumerator SuccessTextAnimation()
    {
        float duration = 1f; // 动画持续时间
        float elapsedTime = 0f;

        RectTransform textRect = successText.GetComponent<RectTransform>();
        Text textComponent = successText.GetComponent<Text>();

        if (textRect == null || textComponent == null)
        {
            yield return new WaitForSeconds(1f);
            ExecuteReload();
            yield break;
        }

        Vector3 originalScale = textRect.localScale;
        Color originalColor = textComponent.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 缓动效果
            float easeProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);

            // 放大效果 (从1倍到1.5倍)
            float scaleMultiplier = Mathf.Lerp(1f, 1.5f, easeProgress);
            textRect.localScale = originalScale * scaleMultiplier;

            // 透明度渐变 (从1到0)
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, easeProgress);
            textComponent.color = newColor;

            yield return null;
        }

        // 确保最终状态
        textRect.localScale = originalScale * 1.5f;
        Color finalColor = originalColor;
        finalColor.a = 0f;
        textComponent.color = finalColor;

        // 重置文本属性
        textRect.localScale = originalScale;
        textComponent.color = originalColor;

        successText.SetActive(false);
    }

    // 将重新加载场景的逻辑抽取到单独的方法
    private void ExecuteReload()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
        // 重置所有计数器
        GlobalCounters.Judgment_Point1_Circle = 0;
        GlobalCounters.Judgment_Point1_Crosses = 0;
        GlobalCounters.Judgment_Point1_Cube = 0;
        GlobalCounters.Judgment_Point1_Triangle = 0;
        GlobalCounters.Judgment_Point1_spider = 0;

        GlobalCounters.Judgment_Point2_Circle = 0;
        GlobalCounters.Judgment_Point2_Crosses = 0;
        GlobalCounters.Judgment_Point2_Cube = 0;
        GlobalCounters.Judgment_Point2_Triangle = 0;
        GlobalCounters.Judgment_Point2_spider = 0;
    }

    public void PlayMusic1() => PlayClip(music1);
    public void PlayMusic2() => PlayClip(music2);
    public void PlayMusic3() => PlayClip(music3);
    public void PlayMusic4() => PlayClip(music4);
    public void PlayMusic5() => PlayClip(music5);

    private void OnSoundButtonClick()
    {
        if (audioSource != null && musicClip != null)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            else
            {
                audioSource.clip = musicClip;
                audioSource.Play();
            }
        }
        else Debug.LogWarning("音频源或音乐片段未设置");
    }

    private float PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            return clip.length; // 返回音频片段的长度
        }
        else 
        {
            Debug.LogWarning("AudioSource 或 音频剪辑未设置！");
            return 0f;
        }
    }

    private void SkipGame()
    {
        Debug.Log("跳过游戏，显示图片");
        if (skipImage != null)
        {
            skipImage.SetActive(true); // 显示跳过图片
            HitPage.SetActive(false);
        }
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false); // 隐藏 skipButton
        }
        if (newImage != null)
        {
            newImage.SetActive(false);  // 隐藏 newImage
        }
        if (ReButton != null)
        {
            ReButton.gameObject.SetActive(false);  // 隐藏 ReButton
        }
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(false);  // 隐藏 HitButton
        }
    }

    private void CloseSkipImage()
    {
        if (skipImage != null)
        {
            skipImage.SetActive(false);
        }
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true); // 显示 skipButton
        }
        if (newImage != null)
        {
            newImage.SetActive(true);  // 显示 newImage
        }
        if (ReButton != null)
        {
            ReButton.gameObject.SetActive(true);  // 显示 ReButton
        }
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(true);  // 显示 HitButton
        }
    }

    private void OpenPage()
    {
        HitPage.SetActive(true);
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(false);  // 隐藏提示按钮
        }
        if (newImage != null)
        {
            newImage.SetActive(false);  // 隐藏新图片
        }
        if (ReButton != null)
        {
            ReButton.gameObject.SetActive(false);  // 隐藏 ReButton
        }
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false); // 隐藏 skipButton
        }

        // 添加点击空白处关闭 HitPage 的事件
        EventTrigger trigger = HitPage.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = HitPage.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => ClosePage());
        trigger.triggers.Add(entry);
    }

    private void OnPointerUp(BaseEventData data)
    {
        if (heldPrefab != null)
        {
            isButtonHeld = false;
            heldPrefab = null;
        }
    }

    // 新添加的关闭页面方法
    private void ClosePage()
    {
        if (HitPage != null)
        {
            HitPage.SetActive(false);
            if (HitButton != null)
            {
                HitButton.gameObject.SetActive(true);   // 显示提示按钮
            }
            if (newImage != null)
            {
                newImage.SetActive(true);  // 显示新图片
            }
            if (ReButton != null)
            {
                ReButton.gameObject.SetActive(true);  // 显示 ReButton
            }
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true); // 显示 skipButton
            }
        }
    }
}

public static class GlobalCounters
{
    public static int Judgment_Point1_Circle = 0;
    public static int Judgment_Point1_Crosses = 0;
    public static int Judgment_Point1_Cube = 0;
    public static int Judgment_Point1_Triangle = 0;
    public static int Judgment_Point1_spider = 0;

    public static int Judgment_Point2_Circle = 0;
    public static int Judgment_Point2_Crosses = 0;
    public static int Judgment_Point2_Cube = 0;
    public static int Judgment_Point2_Triangle = 0;
    public static int Judgment_Point2_spider = 0;

    public static int PuzzleSuccessCount = 0;
    public static int PuzzleFailCount = 0;
}

