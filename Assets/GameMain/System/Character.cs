using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5.0f; // �ƶ��ٶ�

    void Update()
    {
        // �Թ̶��ٶ���ǰ���ң��ƶ�
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}

