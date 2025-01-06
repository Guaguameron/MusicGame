using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string LastSceneKey = "LastScene";
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject quitPanelPrefab; // 退出确认面板的预制体
    private GameObject currentQuitPanel; // 当前显示的退出面板
    public bool allowEscQuit = true;
    private bool isPanelShown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (allowEscQuit && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuitPanel();
        }
    }

    private void ToggleQuitPanel()
    {
        if (isPanelShown)
        {
            HideQuitPanel();
        }
        else
        {
            ShowQuitPanel();
        }
    }

    private void ShowQuitPanel()
    {
        if (currentQuitPanel == null && quitPanelPrefab != null)
        {
            currentQuitPanel = Instantiate(quitPanelPrefab);
            
            Canvas canvas = currentQuitPanel.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 999;
            }
            
            isPanelShown = true;
            
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
    }

    private void HideQuitPanel()
    {
        if (currentQuitPanel != null)
        {
            Destroy(currentQuitPanel);
            currentQuitPanel = null;
            isPanelShown = false;
            
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    public void SaveAndQuit()
    {
        PlayerPrefs.SetString(LastSceneKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ContinueGame()
    {
        HideQuitPanel();
    }

    public void SetEscQuitEnabled(bool enabled)
    {
        allowEscQuit = enabled;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
} 