using UnityEngine;

public class BackgroundManager : MonoBehaviour
{ 
    public BackgroundLayer[] backgroundLayers;
    public GameObject endingImage;  // 场景中的结束图片
    public float endingImageSpeed = 2f;  // 结束图片移动速度
    private bool shouldScroll = false;  // 默认不滚动
    private float musicDuration;  // 音乐总时长
    private float musicStartTime;  // 音乐开始时间
    private bool isEndingStarted = false;  // 是否开始结束动画

    public void OnMusicStart()
    {
        shouldScroll = true;
        musicStartTime = Time.time;
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
    }

    private void Start()
    {
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

    private void Update()
    {
        if (!shouldScroll) return;  // 如果还没开始滚动，直接返回

        float playedTime = Time.time - musicStartTime;  // 已经播放的时间
        
        // 在音乐结束前10秒开始移动结束图片
        if (playedTime >= musicDuration - 8f && !isEndingStarted && endingImage != null)
        {
            isEndingStarted = true;
            // 设置结束图片位置并显示
            float screenRightEdge = Camera.main.orthographicSize * Camera.main.aspect;
            endingImage.transform.position = new Vector3(screenRightEdge * 2, 0, 0);
            endingImage.SetActive(true);
            Debug.Log($"开始移动结束图片，已播放时间：{playedTime}秒，总时长：{musicDuration}秒");
        }

        // 移动结束图片
        if (isEndingStarted && endingImage != null)
        {
            endingImage.transform.Translate(Vector3.left * endingImageSpeed * Time.deltaTime);
        }

        // 正常背景滚动
        if (shouldScroll && !PauseGame.isPaused)
        { 
            foreach (var layer in backgroundLayers)
            { 
                MoveBackgroundLayer(layer);
            }
        }
    }

    private void InitializeLayer(BackgroundLayer layer)
    {
        // �����ڶ�����������
        GameObject duplicate = Instantiate(layer.backgroundObject, layer.backgroundObject.transform.parent);
        layer.duplicateObject = duplicate;

        // ���õڶ������������λ��
        float width = GetBackgroundWidth(layer.backgroundObject);
        duplicate.transform.position = layer.backgroundObject.transform.position + new Vector3(width, 0, 0);
    }

    private void MoveBackgroundLayer(BackgroundLayer layer)
    {
        float width = GetBackgroundWidth(layer.backgroundObject);

        // �ƶ�������������
        MoveBackground(layer.backgroundObject, layer, width);
        MoveBackground(layer.duplicateObject, layer, width);
    }

    private void MoveBackground(GameObject bg, BackgroundLayer layer, float width)
    {
        // �ƶ�����
        bg.transform.Translate(Vector3.left * layer.scrollSpeed * Time.deltaTime);

        // ���������ȫ�Ƴ���Ļ��࣬�����ƶ�����һ���������Ҳ�
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
