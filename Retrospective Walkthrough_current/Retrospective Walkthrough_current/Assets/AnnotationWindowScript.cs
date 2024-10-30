using UnityEngine;
using TMPro;

// class for handling the pop-up annotation window
public class AnnotationWindow : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_InputField inputField;
    public bool isAnnotationMode = false;
    public Canvas canvas2;
    public string lastText = "";
    public GameObject label;
    public SliderScript sliderScript;
    public float threshold = 1.0f;
    public bool clicked = false;
    public GameObject annotationObject = null;
    public Logging loggingScript;

    void Start()
    {
        popupPanel.SetActive(false);
    }

    public bool GetIsAnnotationMode()
    {
        return isAnnotationMode;
    }

    public string GetLastText()
    {
        return lastText;
    }

    public void WhenClicked()
    {
        annotationObject = ClosestAnnotationObject();
        inputField.text = annotationObject.GetComponent<TextMeshProUGUI>().text;
        clicked = true;
        //Debug.Log("Clicked");
        loggingScript.logList.Add(loggingScript.GetFormat("Previous annotation clicked"));
        ShowPopupWindow();
    }

    public void ShowPopupWindow()
    {
        popupPanel.SetActive(true);
        isAnnotationMode = true;
        //canvas2.enabled = false;
    }

    public void ClosePopupWindow()
    {
        if (inputField.text.Length >= 10)
        {
            if (!clicked)
            {
                annotationObject = sliderScript.GetAnnotationObject();
            }

            lastText = label.GetComponent<TextMeshProUGUI>().text;

            TextMeshProUGUI[] textObjects = annotationObject.GetComponentsInChildren<TextMeshProUGUI>();

            loggingScript.logList.Add(loggingScript.GetFormat("Annotation written: " + lastText));

            foreach (TextMeshProUGUI obj in textObjects)
            {
                if (obj.CompareTag("ShortLabel"))
                {
                    obj.text = lastText.Substring(0, 10);
                }
                else
                {
                    obj.text = lastText;
                }
            }

            popupPanel.SetActive(false);
            isAnnotationMode = false;
            inputField.text = "";
            clicked = false;
            //canvas2.enabled = true;
        }
    }

    private GameObject ClosestAnnotationObject()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject[] annotations = GameObject.FindGameObjectsWithTag("Annotation");

        if (annotations.Length > 0)
        {
            double mindistance = 100.0;
            Vector3 mincoords = new Vector3(100f, 100f, 0f);

            foreach (GameObject kp in annotations)
            {
                if (kp != null)
                {
                    Vector2 pos = kp.transform.position;

                    double distance = System.Math.Sqrt((Mathf.Abs(point.x - pos.x) * Mathf.Abs(point.x - pos.x)) + (Mathf.Abs(point.y - pos.y) * Mathf.Abs(point.y - pos.y)));

                    if (distance < mindistance)
                    {
                        mindistance = distance;
                        mincoords = new Vector3(pos.x, pos.y, -0.1f);
                    }
                }
                if (mindistance <= threshold)
                {
                    return kp;
                }
            }
        }

        return null;
    }

}
