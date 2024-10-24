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

    private static float tipTimer = 0f;  // 计时器
    private static float tipDisplayDuration = 1f;  // tip 显示的时间为1秒

        public static void Succeed(int succeedscore, string tips)
    {
        combo++;
        score += succeedscore;
        tip = tips;
        tipTimer = 0f;  // 重置计时器，表示有新的判定
    }

    public static void Fail(int failscore)
    {
        combo = 0;
        score += failscore;
        tip = "miss";
        tipTimer = 0f;  // 重置计时器
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

    // 检查是否超过1秒没更新提示
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
