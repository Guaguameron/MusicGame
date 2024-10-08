using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgePointController : MonoBehaviour
{
    public Camera mainCamera;
    public Vector3 offset; // ������������λ��ƫ��

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // �����ж����������ǰ����ĳ���̶�λ��
        transform.position = mainCamera.transform.position + offset;
        //Debug.Log("�ж���λ��: " + transform.position);
    }
}

