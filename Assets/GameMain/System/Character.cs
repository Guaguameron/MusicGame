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

    private bool isJumping = false;

    void Start()
    {
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
            return;
        }

        characterImage.sprite = idleSprite;
        animator.enabled = false;
        Debug.Log("角色初始化为静止状态");
    }

    void Update()
    {
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;

            animator.enabled = true;
            animator.SetBool("IsRunning", true);
            Debug.Log("音乐开始，角色开始跑动");
        }

        if (musicHasStarted && !musicHasEnded && (Time.time - musicStartTime >= musicLength))
        {
            musicHasEnded = true;
            StopCharacter();
        }
    }

    private void StopCharacter()
    {
        animator.enabled = true;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsFlying", false);
        animator.Play("Player_Idle");
        Debug.Log("音乐结束，角色切换到待机状态");
    }

    // 判定成功 → 播放一次跳跃 → 自动转飞行
    public void TriggerJump()
    {
        if (animator.enabled && !isJumping)
        {
            isJumping = true;

            animator.SetBool("IsRunning", false);
            animator.SetTrigger("Player_Jump");
            Debug.Log("【动画状态】当前状态：" + animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            Debug.Log("【动作】触发跳跃 Player_Jump");
        }
    }


  
    /// Jump动画结束时调用（用Animation Event触发）
    public void OnJumpEnd()
    {
        if (isJumping)
        {
            animator.SetTrigger("Player_Fly");
            isJumping = false;
            Debug.Log("跳跃结束 → 进入飞行待机状态");
        }
    }

    public void ForceRunFromMiss()
    {
        if (animator.enabled)
        {
            animator.ResetTrigger("Player_Jump");
            animator.ResetTrigger("Player_Fly");

            // Reset jump state
            isJumping = false;

            animator.SetBool("IsFlying", false);
            animator.SetBool("IsRunning", true);
            Debug.Log("Miss → 强制角色回到跑步状态");
        }
    }


}
