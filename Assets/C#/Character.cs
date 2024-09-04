using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 移动速度

    void Update()
    {
        // 以固定速度向前（右）移动
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}

