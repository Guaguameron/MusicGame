using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int track; // 1��ʾ�ϲ㣬2��ʾ�²�
    public float noteSpeed = 5.0f; // �������ƶ��ٶ�
    private float StartJudge = 1.0f;
    private float PerfectJudge = 1.0f;
    private float GreatJudge = 1.0f;
    private float GoodJudge = 1.0f;
    private Transform upperJudgePoint; // �ϲ��ж����λ��
    private Transform lowerJudgePoint; // �²��ж����λ��

    void Start()
    {
        // ��ȡ���ж��㡱�� Transform
        upperJudgePoint = GameObject.Find("���ж���").transform;
        lowerJudgePoint = GameObject.Find("���ж���").transform;


        StartJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].StartJudge;
        PerfectJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].PerfectJudge;
        GreatJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GreatJudge;
        GoodJudge = PlayNoteModel.DataTables.TbHardSet.DataList[0].GoodJudge;
    }

    void Update()
    {
        // �ƶ�����
        transform.position += Vector3.left * noteSpeed * Time.deltaTime;

        // ��������Ƿ��ڡ��ж��㡱������
        CheckKeyPress();
    }

    void CheckKeyPress()
    {
        Transform targetJudgePoint = null;

        // ���ݹ��ѡ���Ӧ���ж���
        if (track == 1)
        {
            targetJudgePoint = upperJudgePoint;
        }
        else if (track == 2)
        {
            targetJudgePoint = lowerJudgePoint;
        }

        // ��ⰴ������
        if ((track == 1 && Input.GetKeyDown(KeyCode.J)) || (track == 2 && Input.GetKeyDown(KeyCode.K)))
        {
            float pos = Mathf.Abs(transform.position.x - targetJudgePoint.position.x);
            // �ж������Ƿ����ж�������
            if (targetJudgePoint != null && pos < StartJudge)
            {

                // �жϷ���
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
                if (pos < PerfectJudge)
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
        if(collision.gameObject.name == "Boudry")
        {
            Fail(PlayNoteModel.GetComboPoint(3));
        }
    }

    private void Fail(int score)
    {
        Debug.Log("miss��");
        Destroy(gameObject);// ��������
        PlayNoteModel.Fail(score);
    }

    private void Succeed(int score, string tips)
    {
        Destroy(gameObject);// ��������
        PlayNoteModel.Succeed(score, tips);
    }
}


