using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameSequence : MonoBehaviour
{
    public Image ReadyImage;
    public Image GoImage;
    public float ReadyDisplayTime = 1.5f;  // Ready 显示时间
    public float GoDisplayTime = 1.0f;     // Go 显示时间
    public GameObject GameController;      // 游戏控制器
    public AudioSource GameMusic;          // 游戏音乐音频源

    void Start()
    {
        // 初始化图像状态为隐藏
        ReadyImage.gameObject.SetActive(false);
        GoImage.gameObject.SetActive(false);
        GameController.SetActive(false);
        GameMusic.Stop();  // 确保游戏音乐未播放
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        // 显示 Ready 图像
        ReadyImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(ReadyDisplayTime);  // 等待 Ready 时间
        ReadyImage.gameObject.SetActive(false);

        // 显示 Go 图像
        GoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(GoDisplayTime);  // 等待 Go 时间
        GoImage.gameObject.SetActive(false);

        // 开始游戏逻辑
        GameController.SetActive(true);

        // 播放音乐
        GameMusic.Play();  // 此处开始播放游戏音乐
    }
}
