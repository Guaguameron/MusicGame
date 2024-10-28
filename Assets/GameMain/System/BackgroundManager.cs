using UnityEngine;

public class BackgroundManager : MonoBehaviour
{ 
    public BackgroundLayer[] backgroundLayers;
    private bool shouldScroll = true;  // 添加控制滚动的标志

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
    }

    private void Update()
    {
        // 只有在应该滚动且未暂停时才移动背景
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
