using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// class for handling the fastforward button
public class FastForward : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;
    public PauseButton pauseButtonScript;
    public Logging loggingScript;

    public void OnFastForwardClicked()
    {
        double targetTime = videoPlayer.time + 10.0;
        if (targetTime > videoPlayer.length)
        {
            targetTime = videoPlayer.length;
        }
        videoPlayer.time = targetTime;

        if (pauseButtonScript.IsPaused())
        {
            slider.value = (float)((float)targetTime / videoPlayer.length);
        }

        loggingScript.logList.Add(loggingScript.GetFormat("Fast forward button clicked"));
    }
}
