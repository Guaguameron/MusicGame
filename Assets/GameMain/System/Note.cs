using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int track; // 1表示上层，2表示下层
    public float noteSpeed = 5.0f; 
    public AudioClip successSound; //判定音效
    private AudioSource audioSource; 

    private float StartJudge = 1.0f;
    private float PerfectJudge = 1.0f;
    private float GreatJudge = 1.0f;
    private float GoodJudge = 1.0f;

    private Transform upperJudgePoint; // 上层判定点的位置
    private Transform lowerJudgePoint; // 下层判定点的位置

    void Start()
    {
        upperJudgePoint = GameObject.Find("上判定点").transform;
        lowerJudgePoint = GameObject.Find("下判定点").transform;
           
        StartJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].StartJudge;
        PerfectJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].PerfectJudge;
        GreatJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GreatJudge;
        GoodJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GoodJudge;

        audioSource = GameObject.Find("判定点音效").GetComponent<AudioSource>();
    }

    void Update()
    {
        // 移动音符
        transform.position += Vector3.left * noteSpeed * Time.deltaTime;

        CheckKeyPress();
    }

    void CheckKeyPress()
    {
        Transform targetJudgePoint = null;

        if (track == 1)
        {
            targetJudgePoint = upperJudgePoint;
        }
        else if (track == 2)
        {
            targetJudgePoint = lowerJudgePoint;
        }

        if ((track == 1 && Input.GetKeyDown(KeyCode.J)) || (track == 2 && Input.GetKeyDown(KeyCode.K)))
        {
            float pos = Mathf.Abs(transform.position.x - targetJudgePoint.position.x);

            // 判断音符是否在判定区域内
            if (targetJudgePoint != null && pos < StartJudge)
            {
                // 判断分数
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

                // Miss情况下调用Fail()
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

    private void Fail(int score)
    {
        Debug.Log("miss了");
        Destroy(gameObject);
        PlayNoteModel.Fail(score);
    }

    private void Succeed(int score, string tips)
    {
        PlaySoundEffect(); 
        Destroy(gameObject); 
        PlayNoteModel.Succeed(score, tips);
    }

    private void PlaySoundEffect()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound); 
        }
    }
}
