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
        tip.text = "";  // 初始时隐藏提示文字
    }

    // Update is called once per frame
    void Update()
    {
        score.text = PlayNoteModel.score.ToString();
  
        // 更新提示
        PlayNoteModel.UpdateTip();

        // 每次打击后更新提示文字，并显示1秒
        tip.text = PlayNoteModel.tip;
    }
}
