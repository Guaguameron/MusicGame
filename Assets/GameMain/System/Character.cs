using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public Sprite idleSprite;
    public Image characterImage;
    public Animator animator;
    
    private StartGameSequence gameSequence;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;
    private float musicStartTime;
    private float musicLength;

    void Start()
    {
        //Debug.Log("角色开始跑动");
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
        }
        // 确保角色开始时处于运动状态
        animator.SetBool("IsRunning", true);
    }

    void Update()
    {
        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;
            //Debug.Log($"Music started. Length: {musicLength}");
        }

        // 检查音乐是否已经结束
        if (musicHasStarted && !musicHasEnded && (Time.time - musicStartTime >= musicLength))
        {
            musicHasEnded = true;
            StopCharacter();
        }
    }

    private void StopCharacter()
    {
        animator.SetBool("IsRunning", false);
        SetIdleState();
        Debug.Log("角色停止跑动"); 
    }

    private void SetIdleState()
    {
        characterImage.sprite = idleSprite;
      
    }
}
