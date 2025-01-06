using UnityEngine;
using UnityEngine.UI;

public class QuitPanel : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // 设置按钮监听
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClick);
        }

        // 确保面板显示在最上层
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 999;
        }
    }

    // 防止点击面板背景时的穿透
    public void OnPanelClick()
    {
        // 阻止点击事件传递到下层
    }

    private void OnContinueClick()
    {
        GameManager.Instance.ContinueGame();
    }

    private void OnQuitClick()
    {
        GameManager.Instance.SaveAndQuit();
    }
} 