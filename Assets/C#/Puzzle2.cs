using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Puzzle2 : MonoBehaviour
{ // 四个不同的预设体
    // 四个不同的预设体
    public GameObject circlePrefab;
    public GameObject crossesPrefab;
    public GameObject cubePrefab;
    public GameObject trianglePrefab;

    // 四个按钮
    public Button circleButton;
    public Button crossesButton;
    public Button cubeButton;
    public Button triangleButton;
    public Button playButton;  // 公共的Play_Button

    public Canvas canvas;  // 公有的Canvas
    public Transform Judgment_Point1;  // 判断点1
    public Transform Judgment_Point2;  // 判断点2
    public float moveSpeed = 50f;  // 公有的速度，控制预设体的移动速度

    // 计数器：分别跟踪Circle、Crosses、Cube和Triangle在Judgment_Point1和Judgment_Point2的销毁数量
    public static int Judgment_Point1_Circle = 0;
    public static int Judgment_Point2_Circle = 0;
    public static int Judgment_Point1_Crosses = 0;
    public static int Judgment_Point2_Crosses = 0;
    public static int Judgment_Point1_Cube = 0;
    public static int Judgment_Point2_Cube = 0;
    public static int Judgment_Point1_Triangle = 0;
    public static int Judgment_Point2_Triangle = 0;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();  // 存储所有生成的预设体
    private bool isButtonHeld = false;
    private bool isMoving = false;  // 用来判断预设体是否开始移动
    private Transform playSpace;  // Play_Space的引用
    private Camera canvasCamera;  // Canvas上的Camera引用

    private GameObject selectedPrefab;  // 当前选择的预设体

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

        AddEventTriggerListener(circleButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(crossesButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(cubeButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);
        AddEventTriggerListener(triangleButton.gameObject, EventTriggerType.PointerUp, OnPointerUp);

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
    }

    // 处理预设体跟随鼠标移动的逻辑
    private void HandlePrefabMovement()
    {
        if (isButtonHeld && selectedPrefab != null && spawnedPrefabs.Count > 0)
        {
            Vector2 mousePosition = Input.mousePosition;

            if (canvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    mousePosition,
                    canvasCamera,  // 使用Canvas的Camera
                    out Vector2 localPoint);

                // 获取最后一个生成的预设体（当前正在拖动的）
                GameObject currentPrefab = spawnedPrefabs[spawnedPrefabs.Count - 1];

                // 将生成的预设体跟随鼠标移动
                RectTransform prefabRect = currentPrefab.GetComponent<RectTransform>();
                prefabRect.anchoredPosition = localPoint;

                // 判断生成的预设体与Judgment_Point1的距离
                if (Mathf.Abs(prefabRect.anchoredPosition.y - Judgment_Point1.localPosition.y) < 20)
                {
                    prefabRect.anchoredPosition = new Vector2(prefabRect.anchoredPosition.x, Judgment_Point1.localPosition.y);
                }

                // 判断生成的预设体与Judgment_Point2的距离
                if (Mathf.Abs(prefabRect.anchoredPosition.y - Judgment_Point2.localPosition.y) < 20)
                {
                    prefabRect.anchoredPosition = new Vector2(prefabRect.anchoredPosition.x, Judgment_Point2.localPosition.y);
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
            AddEventTriggerListener(spawnedPrefab, EventTriggerType.PointerDown, OnPrefabPointerDown);
            AddEventTriggerListener(spawnedPrefab, EventTriggerType.PointerUp, OnPrefabPointerUp);

            isButtonHeld = true;
        }
    }

    // 处理预设体按下时跟随鼠标移动
    private void OnPrefabPointerDown(BaseEventData data)
    {
        isButtonHeld = true;
    }

    // 处理预设体松开时停止移动
    private void OnPrefabPointerUp(BaseEventData data)
    {
        isButtonHeld = false;
    }

    // 处理按钮松开时停止预设体的移动
    private void OnPointerUp(BaseEventData data)
    {
        isButtonHeld = false;
    }

    // Play_Button点击事件，开始移动所有预设体
    private void OnPlayButtonClick()
    {
        if (spawnedPrefabs.Count > 0)
        {
            isMoving = true;  // 开始移动
        }
    }

    // 移动所有预设体并检查是否靠近Judgment_Point1或Judgment_Point2
    private void MovePrefabs()
    {
        // 使用临时列表来存储需要移除的预设体
        List<GameObject> prefabsToRemove = new List<GameObject>();

        foreach (GameObject prefab in spawnedPrefabs)
        {
            RectTransform prefabRect = prefab.GetComponent<RectTransform>();

            // 按速度向左移动
            prefabRect.anchoredPosition += new Vector2(-moveSpeed * Time.deltaTime, 0);

            // 判断与Judgment_Point1的距离
            if (Vector2.Distance(prefabRect.anchoredPosition, Judgment_Point1.localPosition) < 1f)
            {
                StartCoroutine(ScaleAndFadeThenDestroy(prefab, 1));
                prefabsToRemove.Add(prefab);
            }
            // 判断与Judgment_Point2的距离
            else if (Vector2.Distance(prefabRect.anchoredPosition, Judgment_Point2.localPosition) < 1f)
            {
                StartCoroutine(ScaleAndFadeThenDestroy(prefab, 2));
                prefabsToRemove.Add(prefab);
            }
        }

        // 从列表中移除已销毁的预设体
        foreach (GameObject prefab in prefabsToRemove)
        {
            spawnedPrefabs.Remove(prefab);
        }

        // 如果所有预设体都处理完毕，停止移动
        if (spawnedPrefabs.Count == 0)
        {
            isMoving = false;
        }
    }

    // 协程处理放大和透明渐变效果，并根据判断点增加计数
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

            // 逐渐放大到1.3倍
            float scaleFactor = Mathf.Lerp(1f, 1.3f, elapsedTime / duration);
            rectTransform.localScale = originalScale * scaleFactor;

            // 逐渐变透明
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            targetImage.color = newColor;

            yield return null;  // 等待下一帧
        }

        // 销毁预设体前，更新计数器
        UpdateCounter(target, judgmentPoint);

        // 确保销毁预设体
        Destroy(target);
    }

    // 根据判断点和预设体类型更新计数器
    private void UpdateCounter(GameObject target, int judgmentPoint)
    {
        string prefabName = target.name.Replace("(Clone)", "").Trim();

        if (prefabName == circlePrefab.name)
        {
            if (judgmentPoint == 1)
            {
                Judgment_Point1_Circle++;
                Debug.Log("销毁Circle于 Judgment_Point1，当前计数: " + Judgment_Point1_Circle);
            }
            else if (judgmentPoint == 2)
            {
                Judgment_Point2_Circle++;
                Debug.Log("销毁Circle于 Judgment_Point2，当前计数: " + Judgment_Point2_Circle);
            }
        }
        else if (prefabName == crossesPrefab.name)
        {
            if (judgmentPoint == 1)
            {
                Judgment_Point1_Crosses++;
                Debug.Log("销毁Crosses于 Judgment_Point1，当前计数: " + Judgment_Point1_Crosses);
            }
            else if (judgmentPoint == 2)
            {
                Judgment_Point2_Crosses++;
                Debug.Log("销毁Crosses于 Judgment_Point2，当前计数: " + Judgment_Point2_Crosses);
            }
        }
        else if (prefabName == cubePrefab.name)
        {
            if (judgmentPoint == 1)
            {
                Judgment_Point1_Cube++;
                Debug.Log("销毁Cube于 Judgment_Point1，当前计数: " + Judgment_Point1_Cube);
            }
            else if (judgmentPoint == 2)
            {
                Judgment_Point2_Cube++;
                Debug.Log("销毁Cube于 Judgment_Point2，当前计数: " + Judgment_Point2_Cube);
            }
        }
        else if (prefabName == trianglePrefab.name)
        {
            if (judgmentPoint == 1)
            {
                Judgment_Point1_Triangle++;
                Debug.Log("销毁Triangle于 Judgment_Point1，当前计数: " + Judgment_Point1_Triangle);
            }
            else if (judgmentPoint == 2)
            {
                Judgment_Point2_Triangle++;
                Debug.Log("销毁Triangle于 Judgment_Point2，当前计数: " + Judgment_Point2_Triangle);
            }
        }
    }

    // 添加事件触发器（EventTrigger）监听器的辅助函数
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
}
public static class GlobalCounters
{
    // Judgment_Point1 的计数器
    public static int Judgment_Point1_Circle = 0;
    public static int Judgment_Point1_Crosses = 0;
    public static int Judgment_Point1_Cube = 0;
    public static int Judgment_Point1_Triangle = 0;

    // Judgment_Point2 的计数器
    public static int Judgment_Point2_Circle = 0;
    public static int Judgment_Point2_Crosses = 0;
    public static int Judgment_Point2_Cube = 0;
    public static int Judgment_Point2_Triangle = 0;
}