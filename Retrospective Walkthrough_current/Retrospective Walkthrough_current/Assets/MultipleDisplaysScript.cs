using UnityEngine;

// class for activating secondary display support
public class MultipleDisplaysScript : MonoBehaviour
{
    void Awake()
    {
        //Debug.Log ("displays connected: " + Display.displays.Length);
    
        for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
    }

}
