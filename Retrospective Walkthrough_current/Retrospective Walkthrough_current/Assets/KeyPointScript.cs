using UnityEngine;
using UnityEngine.UI;
using TMPro;

// class for handling the line/key point button
public class KeyPoint : MonoBehaviour
{
    public bool KeyPointMode;
    public Button keyPointButton;
    public TMP_Text keyPointText;
    public Color keyPointColor1;
    public Color keyPointColor2;
    public Logging loggingScript;

    void Start()
    {
        KeyPointMode = false;
    }

    public void ToggleKeyPointMode()
    {
        KeyPointMode = !KeyPointMode;

        if (KeyPointMode)
        {   
            keyPointColor1.a = 1f;
            keyPointButton.image.color = keyPointColor1;
            keyPointText.text = "Key Point";
            keyPointText.color = new Color(0, 0, 0);

            loggingScript.logList.Add(loggingScript.GetFormat("Key Point mode selected"));
        }
        else
        {
            keyPointColor2.a = 1f;
            keyPointButton.image.color = keyPointColor2;
            keyPointText.text = "Line";
            keyPointText.color = new Color(1, 1, 1);

            loggingScript.logList.Add(loggingScript.GetFormat("Line drawing mode selected"));
        }
    }

    public bool IsKeyPointMode()
    {
        return KeyPointMode;
    }

}
