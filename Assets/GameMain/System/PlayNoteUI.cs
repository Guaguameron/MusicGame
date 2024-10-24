using UnityEngine;
using UnityEngine.UI;

public class PlayNoteUI : MonoBehaviour
{
    public Text score;
    public Text tip;
    public Text comboText;  

    void Start()
    {
        tip.text = "";  // ��ʼʱ������ʾ����
        comboText.text = "";  // ��ʼ�� combo ��ʾ
    }

    // Update is called once per frame
    void Update()
    {

        score.text = PlayNoteModel.score.ToString();

        PlayNoteModel.UpdateTip();
        tip.text = PlayNoteModel.tip;

        // ���� combo ��
        int currentCombo = PlayNoteModel.GetCombo();
        if (currentCombo > 0)
        {
            comboText.text = " " + currentCombo.ToString();
        }
        else
        {
            comboText.text = "";  // Combo Ϊ 0 ʱ������ʾ
        }
    }
}   
