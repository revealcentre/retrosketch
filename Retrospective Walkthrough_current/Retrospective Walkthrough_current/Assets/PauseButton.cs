using UnityEngine;
using UnityEngine.Video;

// class for handling the pause button functionality
public class PauseButton : MonoBehaviour
{   
    public VideoPlayer videoPlayer;
    public GameObject pause;
    public bool isPaused;
    public GameObject triangle;
    public Logging loggingScript;

    public void OnPauseButtonClicked()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            pause.SetActive(false);
            triangle.SetActive(true);
            isPaused = true;
        }
        else
        {
            videoPlayer.Play();
            pause.SetActive(true);
            triangle.SetActive(false);
            isPaused = false;
        }

        loggingScript.logList.Add(loggingScript.GetFormat("Pause button clicked"));
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
