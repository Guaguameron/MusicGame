using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���ǵ�Transform
    public Vector3 offset; // ��������ǵ�ƫ����

    void Start()
    {
        // ���û��ָ��Ŀ�꣬����Ѱ�ұ�ǩΪ "Player" �Ķ���
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // �����������λ�ã�ʹ���������
            transform.position = target.position + offset;
        }
    }
}
