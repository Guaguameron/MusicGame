using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 主角的Transform
    public Vector3 offset; // 相对于主角的偏移量

    void Start()
    {
        // 如果没有指定目标，则尝试寻找标签为 "Player" 的对象
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 更新摄像机的位置，使其跟随主角
            transform.position = target.position + offset;
        }
    }
}
