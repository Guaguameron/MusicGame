using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgePointController : MonoBehaviour
{
    public Camera mainCamera;
    public Vector3 offset; // 相对于摄像机的位置偏移

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // 保持判定点在摄像机前方的某个固定位置
        transform.position = mainCamera.transform.position + offset;
        //Debug.Log("判定点位置: " + transform.position);
    }
}

