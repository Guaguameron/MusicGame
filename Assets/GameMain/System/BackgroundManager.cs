using UnityEngine;

public class BackgroundManager : MonoBehaviour
{ 
    public BackgroundLayer[] backgroundLayers;
    public GameObject endingImage;  // 场景中的结束图片
    public float endingImageSpeed = 4f;  // 结束图片移动速度
    private bool shouldScroll = false;  // 默认不滚动
    private float musicDuration;  // 音乐总时长
    private float musicStartTime;  // 音乐开始时间
    private bool isEndingStarted = false;  // 是否开始结束动画
    private StartGameSequence gameSequence;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;
    private bool isPaused = false;  // 记录上一帧的暂停状态

    private void Start()
    {
        // 获取游戏序列组件
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
            return;
        }

        foreach (var layer in backgroundLayers)
        {
            InitializeLayer(layer);
        }
        
        // 确保结束图片一开始是隐藏的
        if (endingImage != null)
        {
            endingImage.SetActive(false);
        }
    }

    public void OnMusicStart()
    {
        shouldScroll = true;
        musicStartTime = Time.time;
        musicHasStarted = true;
        musicHasEnded = false;
        isPaused = false;
        Debug.Log("背景开始滚动");
    }

    public void SetMusicDuration(float duration)
    {
        musicDuration = duration;
        Debug.Log($"设置音乐时长: {duration}秒");
    }

    public void StopScrolling()
    {
        shouldScroll = false;
        musicHasEnded = true;
        Debug.Log("停止背景滚动");
    }

    // 恢复背景滚动
    public void ResumeScrolling()
    {
        shouldScroll = true;  // 无条件设为 true
        Debug.Log("背景恢复滚动");
    }

   /* public void ResumeScrolling()
    {
        if (!musicHasEnded)
        {
            shouldScroll = true;
            isPaused = false;
            Debug.Log("背景恢复滚动");
        }
    }*/

    private void Update()
    {
        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            shouldScroll = true;
            Debug.Log("检测到音乐开始播放，开始滚动背景");
        }

        // 处理暂停状态
        if (PlayNoteUI.isPaused)
        {
            shouldScroll = false;
            return;
        }
        if (!shouldScroll) return;
        Debug.Log("背景滚动中...");

        /*
        bool currentPauseState = PlayNoteUI.isPaused;
        if (currentPauseState != isPaused)
        {
            isPaused = currentPauseState;
            if (isPaused)
            {
                shouldScroll = false;
                Debug.Log("游戏暂停，停止背景滚动");
            }
            else if (!musicHasEnded)
            {
                shouldScroll = true;
                Debug.Log("游戏恢复，继续背景滚动");
            }
        }

        // 如果处于暂停状态或不应该滚动，直接返回
        if (isPaused || !shouldScroll) return;*/

        // 检查音乐是否已经结束
        if (musicHasStarted && !musicHasEnded && !gameSequence.GameMusic.isPlaying)
        {
            musicHasEnded = true;
            shouldScroll = false;
            Debug.Log("检测到音乐停止，停止背景滚动");
            return;
        }

        float playedTime = Time.time - musicStartTime;  // 已经播放的时间
        
        // 在音乐结束前2.81秒开始移动结束图片
        if (playedTime >= musicDuration - 2.81f && !isEndingStarted && endingImage != null)
        {
            isEndingStarted = true;
            // 直接显示结束图片，不改变其位置
            endingImage.SetActive(true);
            Debug.Log($"开始移动结束图片，已播放时间：{playedTime}秒，总时长：{musicDuration}秒");
        }

        // 移动结束图片
        if (isEndingStarted && endingImage != null)
        {
            endingImage.transform.Translate(Vector3.left * endingImageSpeed * Time.deltaTime);
        }

        // 正常背景滚动
        foreach (var layer in backgroundLayers)
        { 
            MoveBackgroundLayer(layer);
        }
    }

    private void InitializeLayer(BackgroundLayer layer)
    {
        GameObject duplicate = Instantiate(layer.backgroundObject, layer.backgroundObject.transform.parent);
        layer.duplicateObject = duplicate;

        float width = GetBackgroundWidth(layer.backgroundObject);
        duplicate.transform.position = layer.backgroundObject.transform.position + new Vector3(width, 0, 0);
    }

    private void MoveBackgroundLayer(BackgroundLayer layer)
    {
        float width = GetBackgroundWidth(layer.backgroundObject);
        MoveBackground(layer.backgroundObject, layer, width);
        MoveBackground(layer.duplicateObject, layer, width);
    }

    private void MoveBackground(GameObject bg, BackgroundLayer layer, float width)
    {
        bg.transform.Translate(Vector3.left * layer.scrollSpeed * Time.deltaTime);

        if (bg.transform.position.x <= -width)
        {
            bg.transform.position += new Vector3(width * 2, 0, 0);
        }
    }

    private float GetBackgroundWidth(GameObject bg)
    {
        Renderer renderer = bg.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }
        return 0;
    }
}
