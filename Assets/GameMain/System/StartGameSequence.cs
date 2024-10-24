using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameSequence : MonoBehaviour
{
    public Image ReadyImage;
    public Image GoImage;
    public float ReadyDisplayTime = 1.5f;  // Ready ��ʾʱ��
    public float GoDisplayTime = 1.0f;     // Go ��ʾʱ��
    public GameObject GameController;      // ��Ϸ������
    public AudioSource GameMusic;          // ��Ϸ������ƵԴ

    void Start()
    {
        // ��ʼ��ͼ��״̬Ϊ����
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();  // ȷ����Ϸ����δ����
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        // ��ʾ Ready ͼ��
        ReadyImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(ReadyDisplayTime);  // �ȴ� Ready ʱ��
        ReadyImage.gameObject.SetActive(false);

        // ��ʾ Go ͼ��
        GoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(GoDisplayTime);  // �ȴ� Go ʱ��
        GoImage.gameObject.SetActive(false);

        // ��ʼ��Ϸ�߼�
        GameController.SetActive(true);

        // ��������
        GameMusic.Play();  // �˴���ʼ������Ϸ����
    }
}
