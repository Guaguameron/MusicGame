using cfg;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class PlayNoteModel : MonoSingleton<PlayNoteModel>
{
    public static int combo = 0;
    public static int score = 0;
    public static string tip = "";
    public static Tables DataTables { get; private set; }

    private static float tipTimer = 0f;  // ��ʱ��
    private static float tipDisplayDuration = 1f;  // tip ��ʾ��ʱ��Ϊ1��

        public static void Succeed(int succeedscore, string tips)
    {
        combo++;
        score += succeedscore;
        tip = tips;
        tipTimer = 0f;  // ���ü�ʱ������ʾ���µ��ж�
    }

    public static void Fail(int failscore)
    {
        combo = 0;
        score += failscore;
        tip = "miss";
        tipTimer = 0f;  // ���ü�ʱ��
    }

    public static void Start()
    {
        LoadDataTables();
    }
    public static void LoadDataTables()
    {
        string gameConfDir = $"{Application.streamingAssetsPath}/DataTable";
        DataTables = new Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
    }

    // ����Ƿ񳬹�1��û������ʾ
    public static void UpdateTip()
    {
             if (tip != "")
        {
            tipTimer += Time.deltaTime;
            if (tipTimer >= tipDisplayDuration)
            {
                tip = "";  
            }
        }
    }
    public static int GetComboPoint(int level)
    {
        ComboPoint comboPoint = new ComboPoint();
        foreach(ComboPoint c in DataTables.TbHardSet.DataList[0].PointList)
        {
            if(combo >= c.Combo)
            {
                comboPoint = c;
            }
        }
        if(level == 0)
        {
            return comboPoint.PerfectPoint;
        }
        if (level == 1) 
        {
            return comboPoint.GreatPoint;
        }
        if (level == 2)
        {
            return comboPoint.GoodPoint;
        }
        if (level == 3)
        {
            return comboPoint.MissPoint;
        }
        return 0;
    }
    public static int GetCombo()
    {
        return combo;
    }


}
