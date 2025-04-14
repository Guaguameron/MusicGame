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

    // 添加用于特效的变量
    public Image screenEffectImage; // 全屏特效图片
    public Image transitionImage; // 新添加的过渡图片
    private bool hasPlayedOneThirdEffect = false; // 标记特效是否已播放
    private float effectTriggerTime; // 触发两边变黑特效的时间点

    private List<NoteModel> noteList = new List<NoteModel>();
    float myTime = 0;

    void Start()
    {
        PlayNoteModel.Start();
        //暂时只拿第一个noteList
        noteList = PlayNoteModel.DataTables.TbNoteMap.DataList[0].NodeList;

        if (progressBarImage != null)
        {
            progressBarImage.type = Image.Type.Filled;
            progressBarImage.fillMethod = Image.FillMethod.Horizontal;
            progressBarImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            progressBarImage.fillAmount = 0;
            progressBarImage.gameObject.SetActive(false);
        }

        if (audioSource != null && audioSource.clip != null)
        {
            // 计算特效开始时间：音乐总长度减去特效总持续时间
            float effectTotalDuration = 11.4f; // 0.1(闪白) + 0.5(等待) + 0.3(变暗) + 0.2(等待) + 0.5(图片淡入) + 9.5(持续显示) + 0.3(淡出)
            effectTriggerTime = audioSource.clip.length - effectTotalDuration;
        }

        // 初始化屏幕特效图片
        if (screenEffectImage != null)
        {
            screenEffectImage.gameObject.SetActive(false);
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
                    CreateNote(model.Track, model.Speed, model.Type);
                }

                noteList.RemoveAt(i);
            }
        }

        if (audioSource != null && audioSource.clip != null)
        {
            // 检查是否到达触发时间点且特效未播放
            if (!hasPlayedOneThirdEffect && audioSource.time >= effectTriggerTime)
            {
                StartCoroutine(PlayOneThirdEffect());
                hasPlayedOneThirdEffect = true;
            }
        }
    }

    private void CreateNote(int track, float speed, int type)
    {
        // 选择一个随机的音符预制体
        GameObject notePrefab = noteVariants != null && noteVariants.Length > 0 
            ? noteVariants[Random.Range(0, noteVariants.Length)] 
            : Note;  // 如果没有变体，使用默认音符

        // 根据轨道选择生成位置
        Vector3 spawnPosition = track == 1 ? Note1.transform.position : Note2.transform.position;
        
        // 打印生成位置坐标
        Debug.Log($"Note spawn position - X: {spawnPosition.x}, Y: {spawnPosition.y}");

        // 生成音符
        GameObject instance = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
        
        // 打印实例化后的位置坐标
        Debug.Log($"Note instance position - X: {instance.transform.position.x}, Y: {instance.transform.position.y}");
        
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
            noteComponent.noteType = type;
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

    private IEnumerator PlayOneThirdEffect()
    {
        if (screenEffectImage == null) yield break;

        // 保存原始音调
        float originalPitch = audioSource.pitch;

        // 显示特效图片
        screenEffectImage.gameObject.SetActive(true);
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(false);
            transitionImage.color = new Color(1, 1, 1, 0);
        }
        
        // 1. 闪白效果（0.1秒）
        screenEffectImage.color = new Color(1, 1, 1, 0);
        float flashDuration = 0.1f;
        float timer = 0;
        
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / flashDuration);
            screenEffectImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // 闪白效果结束后，将颜色恢复为完全透明
        screenEffectImage.color = new Color(1, 1, 1, 0);
        
        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);

        // 2. 变暗效果（0.3秒）并快速降低音调（0.15秒）
        timer = 0;
        float darkDuration = 0.3f;
        float pitchChangeDuration = 0.15f;
        Color startColor = new Color(1, 1, 1, 0);
        Color targetColor = new Color(0, 0, 0, 0.25f);
        
        while (timer < darkDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / darkDuration;
            screenEffectImage.color = Color.Lerp(startColor, targetColor, progress);
            
            if (timer < pitchChangeDuration)
            {
                float pitchProgress = timer / pitchChangeDuration;
                audioSource.pitch = Mathf.Lerp(originalPitch, 0.5f, pitchProgress);
            }
            yield return null;
        }

        audioSource.pitch = 0.5f;

        // 等待0.2秒后显示过渡图片
        yield return new WaitForSeconds(0.2f);

        // 3. 显示过渡图片（0.5秒淡入）
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(true);
            timer = 0;
            float fadeInDuration = 0.5f;
            
            while (timer < fadeInDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
                transitionImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
        }

        // 4. 保持黑色遮罩和过渡图片显示（10秒）
        float totalDarkDuration = 10f; // 总共持续10秒
        float elapsedTime = 1f; // 已经过去的时间（0.3秒变暗 + 0.2秒等待 + 0.5秒淡入）
        yield return new WaitForSeconds(totalDarkDuration - elapsedTime);

        // 5. 同时淡出黑色遮罩和过渡图片，并快速恢复音调（0.15秒）
        timer = 0;
        float fadeOutDuration = 0.3f;
        pitchChangeDuration = 0.15f;
        Color screenStartColor = screenEffectImage.color;
        Color transitionStartColor = transitionImage != null ? transitionImage.color : Color.white;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;
            
            screenEffectImage.color = new Color(0, 0, 0, Mathf.Lerp(screenStartColor.a, 0, progress));
            if (transitionImage != null)
            {
                transitionImage.color = new Color(1, 1, 1, Mathf.Lerp(transitionStartColor.a, 0, progress));
            }
            
            if (timer < pitchChangeDuration)
            {
                float pitchProgress = timer / pitchChangeDuration;
                audioSource.pitch = Mathf.Lerp(0.5f, originalPitch, pitchProgress);
            }
            
            yield return null;
        }

        audioSource.pitch = originalPitch;

        // 关闭所有特效图片
        screenEffectImage.gameObject.SetActive(false);
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(false);
        }
    }
}
