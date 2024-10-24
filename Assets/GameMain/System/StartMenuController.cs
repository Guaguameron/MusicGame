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
            Debug.Log("�ɹ���ʮ��������ɫת��Ϊ Color ����");
        }
        else
        {
            Debug.LogError("ʮ��������ɫת��ʧ�ܣ������ʽ");
        }

 
        hoverBackgroundImage.gameObject.SetActive(false);

        AddHoverEffect(startButton, StartGame);
        AddHoverEffect(developmentTeamButton, OpenDevelopmentTeamScene);
        AddHoverEffect(exitButton, ExitGame);
    }

    // Ϊ��ť�����ͣЧ�����󶨵���¼�
    void AddHoverEffect(Button button, UnityEngine.Events.UnityAction onClickAction)
    {      
        button.onClick.AddListener(onClickAction);
               
        originalColor = button.GetComponentInChildren<Text>().color;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // ������ʱ����������ɫ����ʾ����ͼƬ
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { OnHoverEnter(button); });
        trigger.triggers.Add(pointerEnter);

        // ����ƿ�ʱ�ָ�������ɫ�����ر���ͼƬ
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { OnHoverExit(button); });
        trigger.triggers.Add(pointerExit);
    }

    // ��ͣʱ�ı�������ɫ����ʾ����ͼƬ
    void OnHoverEnter(Button button)
    {        
        button.GetComponentInChildren<Text>().color = hoverColor;

        hoverBackgroundImage.gameObject.SetActive(true);
        hoverBackgroundImage.rectTransform.position = button.transform.position - new Vector3(0, button.GetComponent<RectTransform>().sizeDelta.y / 2, 0);

        hoverBackgroundImage.rectTransform.sizeDelta = button.GetComponent<RectTransform>().sizeDelta;
    }

    // ����ƿ�ʱ�ָ�ԭ����������ɫ�����ر���ͼƬ
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

        // ��Unity�в���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
