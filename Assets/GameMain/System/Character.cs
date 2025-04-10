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
        // 获取游戏序列组件
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
            return;
        }

        // 设置初始状态
        characterImage.sprite = idleSprite;
        animator.enabled = false;  // 禁用动画器
        Debug.Log("角色初始化为静止状态");
    }

    void Update()
    {
        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;
            
            // 音乐开始时启用动画器并开始跑动
            animator.enabled = true;
            animator.SetBool("IsRunning", true);
            Debug.Log("音乐开始，角色开始跑动");
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
        animator.enabled = false;  // 禁用动画器
        characterImage.sprite = idleSprite;  // 设置为静止图片
        Debug.Log("角色停止跑动"); 
    }
}
