using UnityEngine;

public class Monster : MonoBehaviour
{
    public Animator animator;
    private StartGameSequence gameSequence;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;

    // 动画状态的参数名
    private const string ANIM_IDLE = "Monster_Idel";  // 待机动画
    private const string ANIM_STATIC = "Monster_Static";  // 静止动画
    private const string ANIM_ATTACK = "Attack";   // 攻击动画
    private const string ANIM_HURT = "Hurt";      // 受伤动画
    private const string ANIM_DEATH = "Death";    // 死亡动画

    void Start()
    {
        // 获取游戏序列组件
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
            return;
        }

        // 获取动画器组件（如果没有在Inspector中指定）
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // 设置初始状态
        animator.Play(ANIM_STATIC);  // 播放静止动画
        Debug.Log("怪兽初始化为静止状态");
    }

    void Update()
    {
        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            PlayIdleAnimation();  // 使用封装好的方法
            Debug.Log("音乐开始，怪兽开始待机动画");
        }

        // 检查音乐是否已经结束
        if (musicHasStarted && !musicHasEnded && !gameSequence.GameMusic.isPlaying)
        {
            musicHasEnded = true;
            StopMonster();
        }
    }

    private void StopMonster()
    {
        animator.enabled = true;  // 保持动画器启用
        animator.Play(ANIM_STATIC);  // 切换到静止状态
        Debug.Log("音乐结束，怪兽切换到静止状态"); 
    }

    // 播放指定动画
    private void PlayAnimation(string animName)
    {
        animator.Play(animName);
        Debug.Log($"播放动画: {animName}");
    }

    // 公开方法，供外部调用
    public void PlayIdleAnimation()
    {
        PlayAnimation(ANIM_IDLE);
    }

    public void PlayAttackAnimation()
    {
        PlayAnimation(ANIM_ATTACK);
    }

    public void PlayHurtAnimation()
    {
        PlayAnimation(ANIM_HURT);
    }

    public void PlayDeathAnimation()
    {
        PlayAnimation(ANIM_DEATH);
    }
} 