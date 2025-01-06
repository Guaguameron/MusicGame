using UnityEngine;
using System.Collections;

public class LongNote : Note
{
    public float noteLength = 2.0f; // 长条音符的长度（持续时间）
    private bool isHolding = false; // 是否正在按住
    private bool isInJudgeArea = false; // 新增：是否在判定区域内
    private GameObject noteBody; // 长条音符的身体部分
    
    protected override void Start()
    {
        base.Start();
        // 创建长条音符的视觉效果
        CreateNoteBody();
    }

    private void CreateNoteBody()
    {
        noteBody = new GameObject("NoteBody");
        noteBody.transform.SetParent(transform);
        noteBody.transform.localPosition = Vector3.zero;

        // 添加 LineRenderer 组件
        LineRenderer lineRenderer = noteBody.AddComponent<LineRenderer>();
        
        // 设置线的材质和颜色
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        
        // 设置线的宽度
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        
        // 计算长度
        float visualLength = noteLength * noteSpeed;
        
        // 修改线的位置：从音符位置向右延伸
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + Vector3.right * visualLength);
        
        // 设置渲染顺序，确保显示在背景之上
        lineRenderer.sortingLayerName = "Notes"; // 使用与音符相同的 sorting layer
        lineRenderer.sortingOrder = 5; // 设置一个足够大的值，确保显示在背景上面
    }


    protected override void CheckKeyPress()
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

        // 使用自定义按键替换原来的固定按键
        if ((track == 1 && Input.GetKeyDown(upperKey)) || (track == 2 && Input.GetKeyDown(lowerKey)))
        {
            float pos = Mathf.Abs(transform.position.x - targetJudgePoint.position.x);

            // 判断音符是否在判定区域内
            if (targetJudgePoint != null && pos < StartJudge)
            {
                isInJudgeArea = true;

                if (pos > GoodJudge)
                {
                    // Miss情况下调用Fail()
                    Fail(PlayNoteModel.GetComboPoint(3));
                }
                   
            }
        }

        if (!isJudged && isInJudgeArea)
        {
            KeyCode targetKey = (track == 1) ? upperKey : lowerKey;

            if (!isHolding && Input.GetKeyDown(targetKey))
            {
                // 开始长按
                isHolding = true;
                StartHoldingNote();
            }
            else if (isHolding)
            {
                if (Input.GetKeyUp(targetKey))
                {
                    float visualLength = noteLength * noteSpeed;
                    // 提前松开判断一下距离
                    float pos = Mathf.Abs(transform.position.x + visualLength - targetJudgePoint.position.x);

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
                    Fail(PlayNoteModel.GetComboPoint(3));
                }
            }
        }
    }

    private void StartHoldingNote()
    {
        // 开始长按判定的相关效果
        // 可以添加视觉反馈
    }

    protected override void Update()
    {
        if (!isJudged)
        {
            transform.position += Vector3.left * noteSpeed * Time.deltaTime;
            
            // 更新线的位置
            if (noteBody != null)
            {
                LineRenderer lineRenderer = noteBody.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, transform.position + Vector3.right * (noteLength * noteSpeed));
                }
            }
            
            CheckKeyPress();
        }
    }

    // 在 Succeed 方法被调用时，添加线的渐变效果
    protected new void Succeed(int score, string tips)
    {
        isJudged = true;
        PlaySoundEffect();

        // 开始放大和透明渐变效果，同时处理线的渐变
        StartCoroutine(ScaleAndFadeThenDestroyWithLine(score, tips));
    }

    private IEnumerator ScaleAndFadeThenDestroyWithLine(int score, string tips)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        Vector3 originalScale = transform.localScale;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        LineRenderer lineRenderer = noteBody.GetComponent<LineRenderer>();
        
        if (spriteRenderer == null || lineRenderer == null)
        {
            yield break;
        }

        Color originalColor = spriteRenderer.color;
        Color originalLineColor = lineRenderer.startColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // 逐渐放大到1.3倍
            float scaleFactor = Mathf.Lerp(1f, 1.3f, elapsedTime / duration);
            transform.localScale = originalScale * scaleFactor;

            // 逐渐变透明
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            Color newColor = originalColor;
            newColor.a = alpha;
            spriteRenderer.color = newColor;

            // 线也逐渐变透明
            Color newLineColor = originalLineColor;
            newLineColor.a = alpha;
            lineRenderer.startColor = newLineColor;
            lineRenderer.endColor = newLineColor;

            yield return null;
        }

        PlayNoteModel.Succeed(score, tips);
        Destroy(gameObject);
    }
} 