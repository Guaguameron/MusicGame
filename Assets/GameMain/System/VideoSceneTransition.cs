using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneTransition : MonoBehaviour
{
    public VideoPlayer videoPlayer; 

    void Start()
    {
    
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

      
        videoPlayer.loopPointReached += OnVideoFinished;
    }

  
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Level1");
    }
}
