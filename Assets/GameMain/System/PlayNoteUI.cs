using UnityEngine;
using UnityEngine.UI;

public class PlayNoteUI : MonoBehaviour
{
    public Text score;
    public Text tip;
    public Text comboText;  

    void Start()
    {
        tip.text = "";  // 初始时隐藏提示文字
        comboText.text = "";  // 初始化 combo 显示
    }

    // Update is called once per frame
    void Update()
    {

        score.text = PlayNoteModel.score.ToString();

        PlayNoteModel.UpdateTip();
        tip.text = PlayNoteModel.tip;

        // 更新 combo 数
        int currentCombo = PlayNoteModel.GetCombo();
        if (currentCombo > 0)
        {
            comboText.text = " " + currentCombo.ToString();
        }
        else
        {
            comboText.text = "";  // Combo 为 0 时隐藏显示
        }
    }
}   
