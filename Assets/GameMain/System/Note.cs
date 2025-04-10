using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public int track; // 1表示上层，2表示下层
    public float noteSpeed = 5.0f;
    public AudioClip successSound; //判定音效
    protected AudioSource audioSource;

    protected float StartJudge = 1.0f;
    protected float PerfectJudge = 1.0f;
    protected float GreatJudge = 1.0f;
    protected float GoodJudge = 1.0f;

    protected Transform upperJudgePoint;
    protected Transform lowerJudgePoint;

    protected bool isJudged = false;

    private const string UpperKeyPrefsKey = "UpperKey";
    private const string LowerKeyPrefsKey = "LowerKey";

    protected KeyCode upperKey;
    protected KeyCode lowerKey;

    protected virtual void Start()
    {
        upperJudgePoint = GameObject.Find("上判定点").transform;
        lowerJudgePoint = GameObject.Find("下判定点").transform;

        StartJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].StartJudge;
        PerfectJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].PerfectJudge;
        GreatJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GreatJudge;
        GoodJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GoodJudge;

        audioSource = GameObject.Find("判定点音效").GetComponent<AudioSource>();

        LoadKeySettings();
    }

    void LoadKeySettings()
    {
        string upperKeyStr = PlayerPrefs.GetString(UpperKeyPrefsKey, "J");
        string lowerKeyStr = PlayerPrefs.GetString(LowerKeyPrefsKey, "K");

        if (System.Enum.TryParse(upperKeyStr, out KeyCode parsedUpperKey))
            upperKey = parsedUpperKey;
        else
            upperKey = KeyCode.J;

        if (System.Enum.TryParse(lowerKeyStr, out KeyCode parsedLowerKey))
            lowerKey = parsedLowerKey;
        else
            lowerKey = KeyCode.K;
    }

    protected virtual void Update()
    {
        if (!isJudged)
        {
            transform.position += Vector3.left * noteSpeed * Time.deltaTime;
            CheckKeyPress();
        }
    }

    protected virtual void CheckKeyPress()
    {
        Transform targetJudgePoint = track == 1 ? upperJudgePoint : lowerJudgePoint;

        if ((track == 1 && Input.GetKeyDown(upperKey)) || (track == 2 && Input.GetKeyDown(lowerKey)))
        {
            float pos = Mathf.Abs(transform.position.x - targetJudgePoint.position.x);

            if (targetJudgePoint != null && pos < StartJudge)
            {
                if (pos < PerfectJudge)
                {
                    Succeed(PlayNoteModel.GetComboPoint(0), "perfect");
                    return;
                }
                if (pos < GreatJudge)
                {
                    Succeed(PlayNoteModel.GetComboPoint(1), "great");
                    return;
                }
                if (pos < GoodJudge)
                {
                    Succeed(PlayNoteModel.GetComboPoint(2), "good");
                    return;
                }

                Fail(PlayNoteModel.GetComboPoint(3));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Boudry")
        {
            Fail(PlayNoteModel.GetComboPoint(3));
        }
    }

   protected void Fail(int score)
    {
        Debug.Log("miss了");
        PlayNoteModel.Fail(score);

        var chara = FindObjectOfType<Character>();
        chara.ForceRunFromMiss();

        AnimatorStateInfo info = chara.animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"[动画状态] Miss后状态：{info.fullPathHash}, 名称是否为 Run：{info.IsName("Player_Run")}");

        Destroy(gameObject);
    }


    protected void Succeed(int score, string tips)
    {
        isJudged = true;
        PlaySoundEffect();
        StartCoroutine(ScaleAndFadeThenDestroy(score, tips));

        FindObjectOfType<Character>().TriggerJump();//角色跳跃动画


        // 播放判定动画
        Animator anim = (track == 1)
            ? upperJudgePoint.GetComponent<Animator>()
            : lowerJudgePoint.GetComponent<Animator>();

        if (anim != null)
        {
            anim.ResetTrigger("Judgment Point");
            anim.SetTrigger("Judgment Point");
            StartCoroutine(VerifyAnimationPlay(anim));
        }
    }

    private IEnumerator VerifyAnimationPlay(Animator animator)
    {
        yield return null; // 等待一帧
        yield return null; // 再多等一帧，确保状态切换完成

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Judgment Point"))
        {
            Debug.Log("判定动画正在播放！");
        }
        else
        {
            Debug.LogError("动画触发失败：仍未进入 Judgment Point 状态！");
        }
    }

    private IEnumerator ScaleAndFadeThenDestroy(int score, string tips)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Vector3 originalScale = transform.localScale;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            yield break;
        }

        Color originalColor = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scaleFactor = Mathf.Lerp(1f, 1.3f, elapsedTime / duration);
            transform.localScale = originalScale * scaleFactor;

            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            spriteRenderer.color = newColor;

            yield return null;
        }

        PlayNoteModel.Succeed(score, tips);
        Destroy(gameObject);
    }

    protected void PlaySoundEffect()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }
}
