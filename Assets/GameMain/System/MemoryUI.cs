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
        public int requiredScore;
    }

    public StoryImagePair[] storyImagePairs;
    public Button switchSceneButton;
    public string nextSceneName = "Animation-Lv1Mid";
    public Text scoreText; 

    private int currentScore = 0;
    private int[] requiredScores = new int[] { 1000, 10000, 20000, 25000 };

    private void Start()
    {
        SetRequiredScores();

        foreach (var pair in storyImagePairs)
        {
            pair.button.onClick.AddListener(() => ShowStoryImage(pair));
            
            if (pair.closeButton != null)
            {
                pair.closeButton.onClick.AddListener(() => HideStoryImage(pair));
            }
            
            pair.storyImage.SetActive(false);
        }

        if (switchSceneButton != null)
        {
            switchSceneButton.onClick.AddListener(SwitchScene);
        }
        else
        {
            Debug.LogWarning("Switch Scene Button is not assigned!");
        }

        UpdateScore(20563); // 初始化分数显示
    }

    private void SetRequiredScores()
    {
        for (int i = 0; i < storyImagePairs.Length && i < requiredScores.Length; i++)
        {
            storyImagePairs[i].requiredScore = requiredScores[i];
        }
    }

    public void UpdateScore(int newScore)
    {
        currentScore = newScore;
        if (scoreText != null)
        {
            scoreText.text = "前进路程: " + currentScore.ToString() + "M";
        }
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        foreach (var pair in storyImagePairs)
        {
            bool isActive = currentScore >= pair.requiredScore;
            pair.button.interactable = isActive;
            SetButtonHighlight(pair.button, isActive);
        }
    }

    private void SetButtonHighlight(Button button, bool highlight)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlight ? Color.yellow : Color.white;
        button.colors = colors;
    }

    private void ShowStoryImage(StoryImagePair pair)
    {
        if (currentScore >= pair.requiredScore)
        {
            foreach (var p in storyImagePairs)
            {
                p.storyImage.SetActive(false);
            }

            pair.storyImage.SetActive(true);
            Debug.Log($"Showing StoryImage for button: {pair.button.name}");
        }
    }

    private void HideStoryImage(StoryImagePair pair)
    {
        pair.storyImage.SetActive(false);
        Debug.Log($"Hiding StoryImage for button: {pair.button.name}");
    }

    private void SwitchScene()
    {
        Debug.Log($"Switching to scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    // 用于测试的方法，可以在 Unity 编辑器中调用
    public void AddScore(int amount)
    {
        UpdateScore(currentScore + amount);
    }
}
