using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
using UnityEngine.EventSystems; 

public class StartMenuController : MonoBehaviour
{
    public Button startButton;
    public Button developmentTeamButton;
    public Button exitButton;

   
    public Image hoverBackgroundImage;

 
    public string hoverColorHex = "#50848B"; 
    private Color hoverColor;

    private Color originalColor;

    void Start()
    {
        if (ColorUtility.TryParseHtmlString(hoverColorHex, out hoverColor))
        {
            Debug.Log("成功将十六进制颜色转换为 Color 类型");
        }
        else
        {
            Debug.LogError("十六进制颜色转换失败，请检查格式");
        }

 
        hoverBackgroundImage.gameObject.SetActive(false);

        AddHoverEffect(startButton, StartGame);
        AddHoverEffect(developmentTeamButton, OpenDevelopmentTeamScene);
        AddHoverEffect(exitButton, ExitGame);
    }

    // 为按钮添加悬停效果并绑定点击事件
    void AddHoverEffect(Button button, UnityEngine.Events.UnityAction onClickAction)
    {      
        button.onClick.AddListener(onClickAction);
               
        originalColor = button.GetComponentInChildren<Text>().color;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // 鼠标进入时更改字体颜色并显示背景图片
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { OnHoverEnter(button); });
        trigger.triggers.Add(pointerEnter);

        // 鼠标移开时恢复字体颜色并隐藏背景图片
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { OnHoverExit(button); });
        trigger.triggers.Add(pointerExit);
    }

    // 悬停时改变字体颜色并显示背景图片
    void OnHoverEnter(Button button)
    {        
        button.GetComponentInChildren<Text>().color = hoverColor;

        hoverBackgroundImage.gameObject.SetActive(true);
        hoverBackgroundImage.rectTransform.position = button.transform.position - new Vector3(0, button.GetComponent<RectTransform>().sizeDelta.y / 2, 0);

        hoverBackgroundImage.rectTransform.sizeDelta = button.GetComponent<RectTransform>().sizeDelta;
    }

    // 鼠标移开时恢复原来的字体颜色并隐藏背景图片
    void OnHoverExit(Button button)
    {       
        button.GetComponentInChildren<Text>().color = originalColor;         
        hoverBackgroundImage.gameObject.SetActive(false);
    }

   
    void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }
      
    void OpenDevelopmentTeamScene()
    {        
        SceneManager.LoadScene("Development Team");
    }
       
    void ExitGame()
    {     
        Application.Quit();

        // 在Unity中测试
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
