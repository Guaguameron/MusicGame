using cfg;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject Note;  // 保留作为默认音符
    public GameObject[] noteVariants;  // 不同样式的音符预制体数组
    public GameObject Note1;
    public GameObject Note2;
    public GameObject longNotePrefab;

    public Image progressBarImage; // 替换原来的Slider，使用Image
    public AudioSource audioSource;

    private List<NoteModel> noteList = new List<NoteModel>();
    float myTime = 0;

    void Start()
    {
        PlayNoteModel.Start();
        //暂时只拿第一个noteList
        noteList = PlayNoteModel.DataTables.TbNoteMap.DataList[0].NodeList;

        if (progressBarImage != null)
        {
            progressBarImage.type = Image.Type.Filled;  // 设置图片类型为Filled
            progressBarImage.fillMethod = Image.FillMethod.Horizontal;  // 设置填充方式为水平
            progressBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;  // 从左向右填充
            progressBarImage.fillAmount = 0;  // 初始填充量为0
            progressBarImage.gameObject.SetActive(false); // 初始时隐藏进度条
        }

        if (audioSource != null)
        {
            // 不需要设置maxValue，因为fillAmount是0-1的值
        }
    }

    void Update()
    {
        // 更新进度条
        if (audioSource != null && audioSource.clip != null && progressBarImage != null)
        {
            // 当音乐开始播放时显示进度条
            if (audioSource.isPlaying)
            {
                if (!progressBarImage.gameObject.activeSelf)
                {
                    progressBarImage.gameObject.SetActive(true); // 显示进度条
                }
                float progress = audioSource.time / audioSource.clip.length;
                progressBarImage.fillAmount = progress;  // 设置填充量
            }
        }

        myTime += Time.deltaTime;

        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            NoteModel model = noteList[i];
            if (model.Time <= myTime)
            {
                if (model.Length != 0)
                {
                    CreateLongNote(model.Track, model.Length);
                }
                else
                {
                    CreateNote(model.Track, model.Speed);
                }

                noteList.RemoveAt(i);
            }
        }
    }

    private void CreateNote(int track, float speed)
    {
        // 选择一个随机的音符预制体
        GameObject notePrefab = noteVariants != null && noteVariants.Length > 0 
            ? noteVariants[Random.Range(0, noteVariants.Length)] 
            : Note;  // 如果没有变体，使用默认音符

        // 根据轨道选择生成位置
        Vector3 spawnPosition = track == 1 ? Note1.transform.position : Note2.transform.position;

        // 生成音符
        GameObject instance = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        
        // 保持预制体的原始缩放值
        instance.transform.localScale = notePrefab.transform.localScale;
        
        // 确保SpriteRenderer设置正确
        SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Default";  // 设置渲染层
            spriteRenderer.sortingOrder = 5;  // 提高渲染顺序，确保在前面
            spriteRenderer.color = new Color(1, 1, 1, 1);  // 确保完全不透明
        }

        // 设置音符组件
        Note noteComponent = instance.GetComponent<Note>();
        if (noteComponent != null)
        {
            noteComponent.track = track;
            noteComponent.noteSpeed = speed;
        }

        //Debug.Log($"生成音符: {notePrefab.name}, Scale: {instance.transform.localScale}, SpriteRenderer Order: {spriteRenderer?.sortingOrder}");
    }

    private void CreateLongNote(int track, float length)
    {
        GameObject noteObj = Instantiate(longNotePrefab);
        LongNote longNote = noteObj.GetComponent<LongNote>();
        
        if (longNote != null)
        {
            longNote.track = track;
            longNote.noteLength = length;
            
            // 设置初始位置
            Vector3 spawnPosition;
            if (track == 1)
            {
                spawnPosition = Note1.transform.position;
            }
            else
            {
                spawnPosition = Note2.transform.position;
            }
            
            noteObj.transform.position = spawnPosition;
        }
    }
}
