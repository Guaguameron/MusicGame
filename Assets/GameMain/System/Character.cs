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
        // 如果音乐已经结束，不再执行任何动画状态更新
        if (musicHasEnded || PlayNoteUI.isPaused)
        {
            return;
        }

        // 检查音乐是否开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;

            animator.enabled = true;
            animator.SetBool("IsRunning", true);
            Debug.Log("音乐开始，角色开始跑动");
        }

        // 检查音乐是否结束
        if (musicHasStarted && !musicHasEnded && !gameSequence.IsMusicPlaying())
        {
            musicHasEnded = true;
            StopCharacter();
            Debug.Log("检测到音乐停止，停止角色动画");
        }
    }

    private void StopCharacter()
    {
        // 重置所有动画状态
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsFlying", false);
        animator.ResetTrigger("Player_Jump");
        animator.ResetTrigger("Player_Fly");
        
        // 强制播放待机动画
        animator.Play("Player_Idle", 0, 0f);
        
        // 重置跳跃状态
        isJumping = false;

        // 确保角色保持静止
        characterImage.sprite = idleSprite;
        
        Debug.Log("音乐结束，角色切换到待机状态，使用静止图片");
    }

    // 判定成功 → 播放一次跳跃 → 自动转飞行
    public void TriggerJump()
    {
        // 如果音乐已结束，不再触发任何动作
        if (musicHasEnded || !animator.enabled || isJumping)
        {
            return;
        }

        isJumping = true;
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Player_Jump");
        Debug.Log("【动画状态】当前状态：" + animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        Debug.Log("【动作】触发跳跃 Player_Jump");
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
        // 如果音乐已结束，不再触发任何动作
        if (musicHasEnded)
        {
            return;
        }

        if (animator.enabled)
        {
            animator.ResetTrigger("Player_Jump");
            animator.ResetTrigger("Player_Fly");
            isJumping = false;
            animator.SetBool("IsFlying", false);
            animator.SetBool("IsRunning", true);
            Debug.Log("Miss → 强制角色回到跑步状态");
        }
    }
}
