using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int track; // 1��ʾ�ϲ㣬2��ʾ�²�
    public float noteSpeed = 5.0f; // ������ƶ��ٶ�
    public float noteJudge = 1.0f;
    private Transform upperJudgePoint; // �ϲ��ж����λ��
    private Transform lowerJudgePoint; // �²��ж����λ��

    void Start()
    {
        // ��ȡ���ж��㡱�� Transform
        upperJudgePoint = GameObject.Find("���ж���").transform;
        lowerJudgePoint = GameObject.Find("���ж���").transform;
    }

    void Update()
    {
        // �ƶ����
        transform.position += Vector3.left * noteSpeed * Time.deltaTime;

        // �������Ƿ��ڡ��ж��㡱������
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

        // �ж�����Ƿ����ж�������
        if (targetJudgePoint != null && Mathf.Abs(transform.position.x - targetJudgePoint.position.x) < noteJudge)
        {
            // ��ⰴ������
            if (track == 1 && Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("�ϲ��������ȷ���У�");
                Succeed();
            }
            else if (track == 2 && Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("�²��������ȷ���У�");
                Succeed();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Boudry")
        {
            Fail();
        }
    }

    private void Fail()
    {
        Debug.Log("miss��");
        Destroy(gameObject);// �������
        PlayNoteModel.Fail();
    }

    private void Succeed()
    {
        Destroy(gameObject);// �������
        PlayNoteModel.Succeed();
    }
}


