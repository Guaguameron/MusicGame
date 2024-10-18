using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    public static bool isPaused = false;  

    public Button pauseButton;  
    public Button playButton;   
    public GameObject countdownParent;   //����ʱ
    public Image[] countdownImages;  
    public AudioSource musicSource;  
    public GameObject notes;   

    void Start()
    {
      
        pauseButton.onClick.AddListener(Pause);
        playButton.onClick.AddListener(ResumeWithCountdown);
        playButton.gameObject.SetActive(false);  
        countdownParent.SetActive(false);  
    }

   
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;  // ��ͣ��Ϸ
        musicSource.Pause();  // ��ͣ����
        notes.SetActive(false);  // ֹͣ�����ƶ�

        playButton.gameObject.SetActive(true);  // ��ʾ���Ű�ť
    }

    // �ָ���Ϸ
    public void ResumeWithCountdown()
    {
        countdownParent.SetActive(true);  
        ResetCountdownImages(); 
        StartCoroutine(ResumeAfterCountdown(3));  // ����ʱ3��
    }

    
    IEnumerator ResumeAfterCountdown(int countdown)
    {
        for (int i = 0; i < countdownImages.Length; i++)
        {
            countdownImages[i].gameObject.SetActive(false);  
        }

        
        while (countdown > 0)
        {
            Debug.Log("Countdown: " + countdown);  

            if (countdown - 1 < countdownImages.Length)
            {
                
                for (int i = 0; i < countdownImages.Length; i++)
                {
                    countdownImages[i].gameObject.SetActive(false);
                }
                countdownImages[countdown - 1].gameObject.SetActive(true); 
            }
            yield return new WaitForSecondsRealtime(1);  
            countdown--;
        }

        
        ResetCountdownImages();
        countdownParent.SetActive(false); 

        Time.timeScale = 1f;  // �ָ���Ϸʱ������
        musicSource.Play(); 
        notes.SetActive(true);  
        isPaused = false;

        pauseButton.gameObject.SetActive(true);  
        playButton.gameObject.SetActive(false);  
    }

    // ���õ���ʱͼƬΪ����״̬
    void ResetCountdownImages()
    {
        for (int i = 0; i < countdownImages.Length; i++)
        {
            countdownImages[i].gameObject.SetActive(false);
        }
    }
}


