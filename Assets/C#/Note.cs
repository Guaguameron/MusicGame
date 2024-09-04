using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int track; // 1表示上层，2表示下层
    public float noteSpeed = 5.0f; // 音符的移动速度
    private Transform upperJudgePoint; // 上层判定点的位置
    private Transform lowerJudgePoint; // 下层判定点的位置

    void Start()
    {
        // 获取“判定点”的 Transform
        upperJudgePoint = GameObject.Find("上判定点").transform;
        lowerJudgePoint = GameObject.Find("下判定点").transform;
    }

    void Update()
    {
        // 移动音符
        transform.position += Vector3.left * noteSpeed * Time.deltaTime;

        // 检测音符是否在“判定点”区域内
        CheckKeyPress();
    }

    void CheckKeyPress()
    {
        Transform targetJudgePoint = null;

        // 根据轨道选择对应的判定点
        if (track == 1)
        {
            targetJudgePoint = upperJudgePoint;
        }
        else if (track == 2)
        {
            targetJudgePoint = lowerJudgePoint;
        }

        // 判断音符是否在判定区域内
        if (targetJudgePoint != null && Mathf.Abs(transform.position.x - targetJudgePoint.position.x) < 0.5f)
        {
            // 检测按键输入
            if (track == 1 && Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("上层音符被正确击中！");
                Destroy(gameObject); // 销毁音符
            }
            else if (track == 2 && Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("下层音符被正确击中！");
                Destroy(gameObject); // 销毁音符
            }
        }
    }
}


