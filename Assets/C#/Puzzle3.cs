using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class Puzzle3 : MonoBehaviour
{
    public GameObject[] Start_EndPoint;
    public GameObject[] PositionPoint;
    public GameObject[] Buttons;
    public Canvas canvas;

    private RectTransform draggingButtonRectTransform;
    private Vector2 offset;

    public GameObject HitPage;
    public Button HitButton;

    // 检测双击的时间限制（秒）
    private float doubleClickTimeLimit = 0.5f;

    // 存储每个按钮GameObject的上次点击时间的字典
    private Dictionary<GameObject, float> buttonLastClickTime = new Dictionary<GameObject, float>();

    // 用于存储名为'1'和'2'的子对象，以及Start_EndPoint对象的列表
    private List<GameObject> allObjectsList = new List<GameObject>();

    // 用于跟踪每个对象是否"已连接"的字典（true或false）
    private Dictionary<GameObject, bool> objectConnectionStatus = new Dictionary<GameObject, bool>();

    private bool allConnected = false;  // 用于跟踪是否所有对象都已连接

    public Button giveUpButton;
    public AudioSource audioSource; // 公共 AudioSource
    public AudioClip music1; // MP3 音频剪辑 1
    public AudioClip music2; // MP3 音频剪辑 2
    public AudioClip music3; // MP3 音频剪辑 3
    public AudioClip music4; // MP3 音频剪辑 4
    public AudioClip music5; // MP3 音频剪辑 5
    public AudioClip music6; // MP3 音频剪辑 6
    public AudioClip music7; // MP3 音频剪辑 7
    public AudioClip music8; // MP3 音频剪辑 8

    public Button button1;               // UI 按钮 1
    public Button button2;               // UI 按钮 2
    public Button button3;               // UI 按钮 3
    public Button button4;               // UI 按钮 4
    public Button button5;               // UI 按钮 5
    public Button button6;               // UI 按钮 6
    public Button button7;               // UI 按钮 7
    public Button button8;               // UI 按钮 8
    public GameObject instructionPanel; // 说明面板
    private CanvasGroup panelCanvasGroup; // 用于控制面板透明度
    private float fadeOutDuration = 0.5f; // 淡出动画持续时间

    public GameObject RulePage;
    public Button RuleButton;

    private bool hasAddedScore = false; // 添加一个标志来追踪是否已经加过分

    void Start()
    {
        // 为每个按钮GameObject添加点击监听器并初始化拖动事件
        foreach (GameObject btnObj in Buttons)
        {
            // 添加拖动和点击功能的事件触发器
            AddEventTriggers(btnObj);
        }

        // 收集命名为'1'和'2'的子对象以及Start_EndPoint对象
        CollectObjects();

        // 为放弃按钮添加点击事件监听器
        giveUpButton.onClick.AddListener(OnGiveUpButtonClick);

        AddEventTriggerListener(button1.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music1));
        AddEventTriggerListener(button2.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music2));
        AddEventTriggerListener(button3.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music3));
        AddEventTriggerListener(button4.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music4));
        AddEventTriggerListener(button5.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music5));
        AddEventTriggerListener(button6.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music6));
        AddEventTriggerListener(button7.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music7));
        AddEventTriggerListener(button8.gameObject, EventTriggerType.PointerDown, (data) => PlayMusic(music8));

        // 获取或添加CanvasGroup组件
        panelCanvasGroup = instructionPanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = instructionPanel.AddComponent<CanvasGroup>();
        }

        // 显示说明面板
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
            panelCanvasGroup.alpha = 1f; // 确保初始时完全不透明
        }

        // 添加点击监听
        AddInstructionPanelClickListener();
    }

    void Update()
    {
        // 每帧更新对象位置
        CheckObjectPositions();

        // 基于更新后的位置检查是否所有对象都已连接
        CheckIfAllConnected();

        // 根据连接状态处理场景跳转
        if (allConnected && !hasAddedScore)  // 只有在还没加过分的情况下才加分
        {
            Debug.Log("全连接了");
            if (!audioSource.isPlaying)
            {
                Debug.Log("Puzzle3 准备添加分数，当前分数：" + PlayNoteModel.score);
                // 增加2000分
                PlayNoteModel.score += 2000;
                hasAddedScore = true; // 标记已经加过分
                Debug.Log("Puzzle3 添加分数后，当前分数：" + PlayNoteModel.score);
                // 直接切换场景
                SceneManager.LoadScene("Memory");
            }
        }
    }


    // Method to add EventTrigger components and assign drag-related events
    void AddEventTriggers(GameObject buttonObj)
    {
        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<EventTrigger>();
        }

        // 按下按钮
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data, buttonObj); });
        trigger.triggers.Add(pointerDownEntry);

        // 拖动
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEntry);

        // 松开按钮
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data, buttonObj); });
        trigger.triggers.Add(pointerUpEntry);
    }

    // 开始拖动的方法
    public void OnPointerDown(PointerEventData eventData, GameObject buttonObj)
    {
        draggingButtonRectTransform = buttonObj.GetComponent<RectTransform>();

        // 计算鼠标位置到按钮中心的偏移量
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
        offset = (Vector2)draggingButtonRectTransform.localPosition - localPoint;
    }

    // 更新按钮位置的拖动方法
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingButtonRectTransform != null)
        {
            // 将屏幕位置转换为画布本地坐标并更新按钮位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
            draggingButtonRectTransform.localPosition = localPoint + offset;
        }
    }

    // 停止拖动，检查吸附，处理双击的方法
    public void OnPointerUp(PointerEventData eventData, GameObject buttonObj)
    {
        draggingButtonRectTransform = null;  // 清除按钮的RectTransform引用

        // 检查双击
        OnButtonClick(buttonObj);

        // 检查按钮是否足够靠近任何预置点以进行吸附
        CheckForSnapping(buttonObj);
    }

    // 检查拖动的按钮和所有预置点之间的距离
    void CheckForSnapping(GameObject buttonObj)
    {
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

        foreach (GameObject point in PositionPoint)
        {
            RectTransform pointRect = point.GetComponent<RectTransform>();

            // 计算按钮和预置点之间的距离
            float distance = Vector2.Distance(buttonRect.anchoredPosition, pointRect.anchoredPosition);

            // 如果距离小于40，将按钮吸附到预置点，并隐藏该预置点
            if (distance < 40f)
            {
                buttonRect.anchoredPosition = pointRect.anchoredPosition;
                point.SetActive(false);  // 隐藏预置点
                break;// 找到第一个符合条的点后停止检查
            }
        }
    }

    // 当按钮被点击时调用的方法（检查双击）
    public void OnButtonClick(GameObject buttonObj)
    {
        float timeSinceLastClick = 0f;

        if (buttonLastClickTime.ContainsKey(buttonObj))
        {
            timeSinceLastClick = Time.time - buttonLastClickTime[buttonObj];
        }
        else
        {
            timeSinceLastClick = Mathf.Infinity;
        }

        if (timeSinceLastClick <= doubleClickTimeLimit)
        {
            // Double-click detected
            RotateButton(buttonObj);
        }

        // Update the last click time
        buttonLastClickTime[buttonObj] = Time.time;
    }

    // 将按钮GameObject旋转90度的方法
    void RotateButton(GameObject buttonObj)
    {
        buttonObj.transform.Rotate(0f, 0f, 90f);
    }

    // 收集命名为'1'和'2'的子对象，以及Start_EndPoint对象的方法
    void CollectObjects()
    {
        // 遍历每个按钮并查找其命名为'1'和'2'的子对象
        foreach (GameObject button in Buttons)
        {
            Transform[] allChildren = button.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == "1" || child.name == "2")
                {
                    allObjectsList.Add(child.gameObject);
                    objectConnectionStatus[child.gameObject] = false;  // Initialize with false
                }
            }
        }

        // 将所有Start_EndPoint对象添加到列表中
        foreach (GameObject endPoint in Start_EndPoint)
        {
            allObjectsList.Add(endPoint);
            objectConnectionStatus[endPoint] = false;  // Initialize with false
        }

        // 检查列表中对象的位置
        CheckObjectPositions();
    }

    // 检查列表中每个对象与其他对象之间的距离
    void CheckObjectPositions()
    {
        // 首先显示所有预置点
        foreach (GameObject point in PositionPoint)
        {
            point.SetActive(true);
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // 检查每个按钮是否在预置点附近
        foreach (GameObject button in Buttons)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            
            foreach (GameObject point in PositionPoint)
            {
                RectTransform pointRect = point.GetComponent<RectTransform>();
                
                // 计算按钮和预置点之间的距离
                Vector2 buttonLocalPoint, pointLocalPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, 
                    RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, buttonRect.position), 
                    canvas.worldCamera, 
                    out buttonLocalPoint
                );
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, 
                    RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, pointRect.position), 
                    canvas.worldCamera, 
                    out pointLocalPoint
                );
                
                float distance = Vector2.Distance(buttonLocalPoint, pointLocalPoint);
                
                // 如果按钮足够靠近预置点，则隐藏该预置点
                if (distance < 30f)
                {
                    point.SetActive(false);
                    break;
                }
            }
        }

        // 原有的对象连接检查逻辑
        for (int i = 0; i < allObjectsList.Count; i++)
        {
            GameObject currentObject = allObjectsList[i];
            RectTransform currentRect = currentObject.GetComponent<RectTransform>();
            bool isCloseToObject = false;

            for (int j = 0; j < allObjectsList.Count; j++)
            {
                if (i == j) continue;

                GameObject otherObject = allObjectsList[j];
                RectTransform otherRect = otherObject.GetComponent<RectTransform>();

                Vector2 currentLocalPoint, otherLocalPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, currentRect.position), canvas.worldCamera, out currentLocalPoint);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, otherRect.position), canvas.worldCamera, out otherLocalPoint);

                float distance = Vector2.Distance(currentLocalPoint, otherLocalPoint);

                if (distance < 30f)
                {
                    isCloseToObject = true;
                    break;
                }
            }

            objectConnectionStatus[currentObject] = isCloseToObject;
        }
    }


    // 检查列表中的所有对象是否都已连接
    void CheckIfAllConnected()
    {
        allConnected = true;

        // 遍历每个按钮并检查其子对象'1'和'2'是否已连接
        foreach (GameObject button in Buttons)
        {
            bool buttonConnected = true; // 假设按钮的子对象都已连接

            // 获取子对象'1'和'2'
            Transform[] allChildren = button.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == "1" || child.name == "2")
                {
                    // 如果任何子对象'1'或'2'未连接，标记按钮为未连接状态
                    if (!objectConnectionStatus[child.gameObject])
                    {
                        buttonConnected = false;
                        Debug.Log(button.name + " 的 " + child.name + " 没有连接 (false)");
                    }
                }
            }

            // 如果按钮的任何子对象未连接，标记整体检查为false
            if (!buttonConnected)
            {
                allConnected = false;
            }
        }

        // 根据连接状态输出结果
        if (allConnected)
        {
            Debug.Log("全连接了");
        }
        else
        {
            Debug.Log("没有全连接");
        }
    }
    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    public void OpenPage()
    {
        HitPage.SetActive(true);
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(false);  // 隐藏提示按钮
        }
        if (RuleButton != null)
        {
            RuleButton.gameObject.SetActive(false); // 隐藏规则按钮
        }
        if (giveUpButton != null)
        {
            giveUpButton.gameObject.SetActive(false); // 隐藏放弃按钮
        }
    }

    public void ClosePage()
    {
        HitPage.SetActive(false);
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(true);   // 显示提示按钮
        }
        if (RuleButton != null)
        {
            RuleButton.gameObject.SetActive(true);  // 显示规则按钮
        }
        if (giveUpButton != null)
        {
            giveUpButton.gameObject.SetActive(true); // 显示放弃按钮
        }
    }

    // 放弃按钮点击事件处理方法
    private void OnGiveUpButtonClick()
    {
        // 检查是否有音效在播放
        if (audioSource.isPlaying)
        {
            StartCoroutine(WaitForAudioAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene("Memory");
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
        
        // 直接加载下一个场景，删除淡出效果
        SceneManager.LoadScene("Memory");
    }

    // 添加说明面板的点击监听
    private void AddInstructionPanelClickListener()
    {
        if (instructionPanel != null)
        {
            // 为说明面板添加点击事件触发器
            EventTrigger trigger = instructionPanel.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = instructionPanel.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => CloseInstructionPanel());
            trigger.triggers.Add(entry);
        }
    }

    // 关闭说明面板
    private void CloseInstructionPanel()
    {
        if (instructionPanel != null)
        {
            StartCoroutine(FadeOutPanel());
        }
    }

    // 淡出动画协程
    private IEnumerator FadeOutPanel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = 1 - (elapsedTime / fadeOutDuration);
            yield return null;
        }

        // 确保完全透明
        panelCanvasGroup.alpha = 0f;
        instructionPanel.SetActive(false);
    }

    private void AddEventTriggerListener(GameObject obj, EventTriggerType eventType, Action<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }

    // Rule相关方法
    public void OpenRulePage()
    {
        RulePage.SetActive(true);
        if (RuleButton != null)
        {
            RuleButton.gameObject.SetActive(false); // 隐藏规则按钮
        }
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(false);  // 隐藏提示按钮
        }
        if (giveUpButton != null)
        {
            giveUpButton.gameObject.SetActive(false); // 隐藏放弃按钮
        }
    }

    public void CloseRulePage()
    {
        RulePage.SetActive(false);
        if (RuleButton != null)
        {
            RuleButton.gameObject.SetActive(true);  // 显示规则按钮
        }
        if (HitButton != null)
        {
            HitButton.gameObject.SetActive(true);   // 显示提示按钮
        }
        if (giveUpButton != null)
        {
            giveUpButton.gameObject.SetActive(true); // 显示放弃按钮
        }
    }
}
