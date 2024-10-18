using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    public static bool isPaused = false;  

    public Button pauseButton;  
    public Button playButton;   
    public GameObject countdownParent;   //µπº∆ ±
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
        Time.timeScale = 0f;  // ‘›Õ£”Œœ∑
        musicSource.Pause();  // ‘›Õ£“Ù¿÷
        notes.SetActive(false);  // Õ£÷π“Ù∑˚“∆∂Ø

        playButton.gameObject.SetActive(true);  // œ‘ æ≤•∑≈∞¥≈•
    }

    // ª÷∏¥”Œœ∑
    public void ResumeWithCountdown()
    {
        countdownParent.SetActive(true);  
        ResetCountdownImages(); 
        StartCoroutine(ResumeAfterCountdown(3));  // µπº∆ ±3√Î
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

        Time.timeScale = 1f;  // ª÷∏¥”Œœ∑ ±º‰¡˜ ≈
        musicSource.Play(); 
        notes.SetActive(true);  
        isPaused = false;

        pauseButton.gameObject.SetActive(true);  
        playButton.gameObject.SetActive(false);  
    }

    // ÷ÿ÷√µπº∆ ±Õº∆¨Œ™“˛≤ÿ◊¥Ã¨
    void ResetCountdownImages()
    {
        for (int i = 0; i < countdownImages.Length; i++)
        {
            countdownImages[i].gameObject.SetActive(false);
        }
    }
}


