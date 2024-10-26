using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryUI : MonoBehaviour
{
    [System.Serializable]
    public class StoryImagePair
    {
        public Button button;
        public GameObject storyImage;
        public Button closeButton;
    }

    public StoryImagePair[] storyImagePairs;
    public Button switchSceneButton;

    private void Start()
    {
        foreach (var pair in storyImagePairs)
        {
            pair.button.onClick.AddListener(() => ShowStoryImage(pair));
            
            if (pair.closeButton != null)
            {
                pair.closeButton.onClick.AddListener(() => HideStoryImage(pair));
            }
            
            // 确保所有 StoryImage 初始状态为隐藏
            pair.storyImage.SetActive(false);
        }

        // 为切换场景按钮添加监听器
        if (switchSceneButton != null)
        {
            switchSceneButton.onClick.AddListener(SwitchScene);
        }
        else
        {
            Debug.LogWarning("Switch Scene Button is not assigned!");
        }
    }

    private void ShowStoryImage(StoryImagePair pair)
    {
        // 隐藏所有 StoryImage
        foreach (var p in storyImagePairs)
        {
            p.storyImage.SetActive(false);
        }

        // 显示被点击的按钮对应的 StoryImage
        pair.storyImage.SetActive(true);
        Debug.Log($"Showing StoryImage for button: {pair.button.name}");
    }

    private void HideStoryImage(StoryImagePair pair)
    {
        pair.storyImage.SetActive(false);
        Debug.Log($"Hiding StoryImage for button: {pair.button.name}");
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene("Animation-Lv1Mid");
    }
}
