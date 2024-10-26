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

        if (!PauseGame.isPaused) // ȷ����Ϸδ��ͣ�ż���
        {
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
    }

    IEnumerator WaitForMusicToEnd()
    {
        while (GameMusic.isPlaying || PauseGame.isPaused)
        {
            if (!PauseGame.isPaused)
            {
                yield return null; // ��Ϸ���ڽ���ʱ
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // ��ͣʱ�ȴ�
            }
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
            SceneManager.LoadScene("Puzzle2");//��������Ϸ
        }
    }

    // ����ʧȥ����ʱ����ͣ����
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && GameMusic.isPlaying)
        {
            // ���ʧȥ���㲢���������ڲ��ţ�����ͣ��Ϸ
            PauseGame.isPaused = true;
            GameMusic.Pause(); // ��ͣ����
            GameController.SetActive(false); // ��ͣ��Ϸ�е���������
        }
        else if (hasFocus)
        {
            // �ָ�����ʱ�Ĵ����߼�
            PauseGame.isPaused = false;
            GameMusic.Play(); // �ָ�����
            GameController.SetActive(true); // �ָ���Ϸ
        }
    }
}
