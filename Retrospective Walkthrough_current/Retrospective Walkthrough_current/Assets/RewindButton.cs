using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// class for handling the rewind button
public class RewindButton : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;
    public PauseButton pauseButtonScript;
    public Logging loggingScript;

    public void OnRewindButtonClicked()
    {
        double targetTime = videoPlayer.time - 10.0;
        if (targetTime < 0)
        {
            targetTime = 0;
        }
        videoPlayer.time = targetTime;

        if (pauseButtonScript.IsPaused())
        {
            slider.value = (float)((float)targetTime / videoPlayer.length);
        }

        loggingScript.logList.Add(loggingScript.GetFormat("Rewind button clicked"));
    }
}
