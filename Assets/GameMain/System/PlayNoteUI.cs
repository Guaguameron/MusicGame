using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayNoteUI : MonoBehaviour
{
    public Text score;
    public Text tip;
    // Start is called before the first frame update

    private Coroutine tipCoroutine;
    void Start()
    {
        tip.text = "";  // ��ʼʱ������ʾ����
    }

    // Update is called once per frame
    void Update()
    {
        score.text = PlayNoteModel.score.ToString();
  
        // ������ʾ
        PlayNoteModel.UpdateTip();

        // ÿ�δ���������ʾ���֣�����ʾ1��
        tip.text = PlayNoteModel.tip;
    }
}
