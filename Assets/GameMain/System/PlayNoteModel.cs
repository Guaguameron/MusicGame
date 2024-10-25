using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNoteModel : MonoSingleton<PlayNoteModel>
{
    public static int combo = 0;
    public static int score = 0;
    public static string tip = "missing";

    public static void Succeed()
    {
        combo++;
        score += 100;
        tip = "succeed";
    }

    public static void Fail()
    {
        combo = 0;
        score -= 100;
        tip = "miss";
    }
}
