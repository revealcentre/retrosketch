using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class PlaybackSpeedControl : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void btnPress()
    {
        if(vp.playbackSpeed == 1.0f)
        {
            vp.playbackSpeed = 1.5f;
            tmpText.text = "1.5x";
        }
        else if(vp.playbackSpeed == 1.5f)
        {
            vp.playbackSpeed = 2.0f;
            tmpText.text = "2.0x";
        }
        else if(vp.playbackSpeed == 2.0f)
        {
            vp.playbackSpeed = 2.5f;
            tmpText.text = "2.5x";
        }
        else if(vp.playbackSpeed == 2.5f)
        {
            vp.playbackSpeed = 1.0f;
            tmpText.text = "1.0x";
        }
    }
}
