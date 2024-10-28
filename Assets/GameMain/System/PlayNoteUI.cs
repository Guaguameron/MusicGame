using UnityEngine;
using UnityEngine.UI;

public class PlayNoteUI : MonoBehaviour
{
    public Text score;
    public Text tip;
    public Text comboText;
    public Image comboImage;

    private StartGameSequence gameSequence;
    private bool musicHasStarted = false;
    private bool musicHasEnded = false;
    private float musicStartTime;
    private float musicLength;

    void Start()
    {
        tip.text = "";  // 初始时隐藏提示文字
        comboText.text = "";
        comboImage.gameObject.SetActive(false);

        // 获取 StartGameSequence 组件
        gameSequence = FindObjectOfType<StartGameSequence>();
        if (gameSequence == null)
        {
            Debug.LogError("StartGameSequence not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        score.text = PlayNoteModel.score.ToString();
        PlayNoteModel.UpdateTip();
        tip.text = PlayNoteModel.tip;

        // 检查音乐是否已经开始播放
        if (!musicHasStarted && gameSequence.GameMusic.isPlaying)
        {
            musicHasStarted = true;
            musicStartTime = Time.time;
            musicLength = gameSequence.GameMusic.clip.length;
        }

        // 检查音乐是否已经结束
        if (musicHasStarted && !musicHasEnded && (Time.time - musicStartTime >= musicLength))
        {
            musicHasEnded = true;
            HideCombo();
        }

        // 只在音乐播放时更新 combo 显示
        if (musicHasStarted && !musicHasEnded)
        {
            UpdateComboDisplay();
        }
    }
    // 更新 combo 数
    private void UpdateComboDisplay()
    {
        int currentCombo = PlayNoteModel.GetCombo();
        if (currentCombo > 0)
        {
            comboText.text = " " + currentCombo.ToString();
            comboImage.gameObject.SetActive(true);
        }
        else
        {
            comboText.text = "";  //  Combo 为 0 时隐藏显示
            comboImage.gameObject.SetActive(false);
        }
    }

    private void HideCombo()
    {
        comboText.text = "";
        comboImage.gameObject.SetActive(false);
    }
}
