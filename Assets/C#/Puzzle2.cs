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

    private Dictionary<GameObject, Coroutine> checkCoroutines = new Dictionary<GameObject, Coroutine>();

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

        // 检测点击关闭 HitPage
        if (HitPage != null && HitPage.activeSelf && Input.GetMouseButtonDown(0))
        {
            HitPage.SetActive(false);
            if (HitButton != null)
            {
                HitButton.gameObject.SetActive(true);   // 显示提示按钮
            }
           
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
            // 增加10000分
            PlayNoteModel.score += 10000;
            StartCoroutine(WaitForAudioAndLoadScene());
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
        SceneManager.LoadScene("MissGame");
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
        // 显示重启提示文本
        if (restartText != null)
        {
            restartText.SetActive(true);
            StartCoroutine(ReloadSceneWithDelay());
        }
        else
        {
            // 如果没有文本对象，直接重新加载场景
            ExecuteReload();
        }
    }

    // 添加新的协程方法来处理延迟重载
    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(1f); // 等待1秒
        if (restartText != null)
        {
            restartText.SetActive(false);
        }
        ExecuteReload();
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


    public void OpenPage()
    {
        HitPage.SetActive(true);
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(false);  // 隐藏提示按钮
        }

    }

    private void OnPointerUp(BaseEventData data)
    {
        if (heldPrefab != null)
        {
            isButtonHeld = false;
            heldPrefab = null;
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
}

