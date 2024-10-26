using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public BackgroundLayer[] backgroundLayers;
    private StartGameSequence gameSequence;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;
    private float musicStartTime;
    private float musicLength;

    private void Start()
    {
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
        }

        foreach (var layer in backgroundLayers)
        {
            InitializeLayer(layer);
        }
    }

    private void Update()
    {
        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;
            Debug.Log($"Music started for background. Length: {musicLength}");
        }

        // 检查音乐是否已经结束（基于音乐长度）
        if (musicHasStarted && !musicHasEnded && (Time.time - musicStartTime >= musicLength))
        {
            musicHasEnded = true;
            Debug.Log("Background scrolling stopped");
        }

        // 只有在音乐没有结束时才滚动背景
        if (!musicHasEnded)
        {
            foreach (var layer in backgroundLayers)
            {
                MoveBackgroundLayer(layer);
            }
        }
    }

    private void InitializeLayer(BackgroundLayer layer)
    {
        // 创建第二个背景对象
        GameObject duplicate = Instantiate(layer.backgroundObject, layer.backgroundObject.transform.parent);
        layer.duplicateObject = duplicate;

        // 设置第二个背景对象的位置
        float width = GetBackgroundWidth(layer.backgroundObject);
        duplicate.transform.position = layer.backgroundObject.transform.position + new Vector3(width, 0, 0);
    }

    private void MoveBackgroundLayer(BackgroundLayer layer)
    {
        float width = GetBackgroundWidth(layer.backgroundObject);

        // 移动两个背景对象
        MoveBackground(layer.backgroundObject, layer, width);
        MoveBackground(layer.duplicateObject, layer, width);
    }

    private void MoveBackground(GameObject bg, BackgroundLayer layer, float width)
    {
        // 移动背景
        bg.transform.Translate(Vector3.left * layer.scrollSpeed * Time.deltaTime);

        // 如果背景完全移出屏幕左侧，将其移动到另一个背景的右侧
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
