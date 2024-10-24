using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class StartGameSequence : MonoBehaviour
{
    public Image ReadyImage; 
    public Image GoImage; 
    public float ReadyDisplayTime = 1.5f; 
    public float GoDisplayTime = 1.0f; 
    public GameObject GameController;
    public AudioSource GameMusic; 

    public Button showButton; // ʯ̨��ť
    public Image imageToShow; // ���ʯ̨����ʾ��ͼƬ
    public Button jumpSceneButton;

    void Start()
    {           
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();

        showButton.gameObject.SetActive(false); 
        imageToShow.gameObject.SetActive(false); 
        jumpSceneButton.gameObject.SetActive(false); 

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        ReadyImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(ReadyDisplayTime); 
        ReadyImage.gameObject.SetActive(false);


        GoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(GoDisplayTime); 
        GoImage.gameObject.SetActive(false);
          
        GameController.SetActive(true);
               
        GameMusic.Play();

        // ������ֲ���״̬��������ͣ�����
        yield return StartCoroutine(WaitForMusicToEnd());

        if (!PauseGame.isPaused)
        {
            showButton.gameObject.SetActive(true);
            showButton.onClick.AddListener(OnButtonClick);
        }
    }

    IEnumerator WaitForMusicToEnd()
    {
        while (GameMusic.isPlaying || PauseGame.isPaused)
        {
            yield return null;
        }
    }

    void OnButtonClick()
    {
        if (!PauseGame.isPaused)
        {
            imageToShow.gameObject.SetActive(true); 
            showButton.gameObject.SetActive(false); 

            jumpSceneButton.gameObject.SetActive(true);
            jumpSceneButton.onClick.AddListener(JumpToNextScene);
        }
    }


    void JumpToNextScene()
    {
        if (!PauseGame.isPaused)
        {
            SceneManager.LoadScene("Development Team");
        }
    }
}
