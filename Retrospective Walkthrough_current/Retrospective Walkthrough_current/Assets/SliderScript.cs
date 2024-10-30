using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

// class for handling the video slider, including snapping to points and showing the annotation window when necessary
public class SliderScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{   
    public VideoPlayer videoPlayer;
    public Slider slider;
    public bool isSeeking = false;
    public bool videoPlaying = false;
    public bool SliderSelected = false;
    public PauseButton pauseButtonScript;
    public Canvas anyCanvas;
    public float xThreshold = 0.1f;
    public GameObject square;
    public Color green;
    public float lastUpdate = 0f;
    public float tolerance = 0.01f;
    public GameObject annotationPrefab;
    public AnnotationWindow annotationScript;
    public GameObject annotationObject;
    public Canvas canvas2;
    public float lastPosition;
    public Canvas[] canvasList;
    public float snappedPosition;
    public float snapThreshold = 1.5f;
    public Eraser eraserModeScript;
    public AnnotationWindow annotationModeScript;
    public PopUpWindow popupWindowScript;
    public KeyPoint keyPointScript;
    public Logging loggingScript;

    void Start()
    {   
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnSliderValueChanged(float value)
    {
        if (SliderSelected)
        {
            videoPlayer.time = videoPlayer.length * value;

            if (pauseButtonScript.IsPaused())
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject[] keypoints = GameObject.FindGameObjectsWithTag("Point");
                if (keypoints.Length > 0)
                {
                    bool stop = false;
                    foreach (GameObject kp in keypoints)
                    {
                        Vector3 kpPos = kp.transform.position;
                        if (Mathf.Abs(pos.x - kpPos.x) < xThreshold && !stop)
                        {
                            //Debug.Log("Slider snapped to keypoint");

                            snappedPosition = kpPos.x;
                            float scaledPosition = ScaleX(snappedPosition) / (float)videoPlayer.length;
                            slider.value = scaledPosition;
                            videoPlayer.time = scaledPosition * (float)videoPlayer.length;
                            stop = true;
                        }

                    }
                }   
            }
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        isSeeking = true;        
    }

    public GameObject GetAnnotationObject()
    {
        return annotationObject;
    }

    void Update()
    {        
        // Get slider to follow video progression
        if (isSeeking == true && !SliderSelected && !pauseButtonScript.IsPaused())
        {
            slider.value = (float)videoPlayer.time / (float)videoPlayer.length;

            if (videoPlayer.isPlaying)
            {
                videoPlaying = true;
            }
            else
            {
                videoPlaying = false;
            }
        }
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (pauseButtonScript.IsPaused() && IsMouseWithinCanvasArea() && !eraserModeScript.GetIsEraserMode() && !annotationModeScript.GetIsAnnotationMode() && !popupWindowScript.GetIsExportMode() && !keyPointScript.IsKeyPointMode())// && (Mathf.Abs(pos.x - snappedPosition) > snapThreshold))
        {
            // follow mouse
            if (Input.GetMouseButton(0))
            {
                float frac = ScaleX(pos.x) / (float)videoPlayer.length;
                slider.value = frac;
                videoPlayer.time = videoPlayer.length * frac;
            }
            
        }
        // look for 5 key points on slider line => turn points green and show annotation window
        if (pauseButtonScript.IsPaused() && (Time.time - lastUpdate >= 1f))
        {

            GameObject[] keypoints = GameObject.FindGameObjectsWithTag("Point");
            float sliderPos = square.transform.position.x;

            if (keypoints.Length > 0)
            {
                List<GameObject> validKp = new List<GameObject>();
                int counter = 0;
                foreach (GameObject kp in keypoints)
                {
                    float kpPos = kp.transform.position.x;

                    if (Mathf.Abs(kpPos - sliderPos) < tolerance)
                    {
                        counter += 1;
                        validKp.Add(kp);
                    }
                }

                if (counter == 5)
                {
                    //Debug.Log("5 is the magic number");

                    foreach (GameObject k in validKp)
                    {
                        green.a = 1f;
                        k.GetComponent<SpriteRenderer>().color = green;

                        GameObject[] annotations = GameObject.FindGameObjectsWithTag("Annotation");
                        bool alreadyThere = false;
                        foreach (GameObject obj in annotations)
                        {
                            if (Mathf.Abs(obj.transform.position.x - sliderPos) < tolerance)
                            {
                                alreadyThere = true;
                            }
                        }

                        if (!alreadyThere)
                        {
                            Vector3 squarePos = square.transform.position;
                            GameObject icon = Instantiate(annotationPrefab, new Vector3(squarePos.x, squarePos.y + 6.2f, 0f), Quaternion.identity);
                            icon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            icon.tag = "Annotation";
                            icon.transform.SetParent(canvas2.transform);
                            annotationScript.ShowPopupWindow();
                            annotationObject = icon;

                            loggingScript.logList.Add(loggingScript.GetFormat("5 valid points found annotation window shown", null , squarePos.x, 1000f));
                        }
                    }
                }
            }

            lastUpdate = Time.time;
            lastPosition = slider.value;
        }
    }

    public bool IsSliderSelected()
    {
        return SliderSelected;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("The slider handle has been selected!");
        loggingScript.logList.Add(loggingScript.GetFormat("Slider clicked"));

        SliderSelected = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("The slider handle has been deselected!");
        loggingScript.logList.Add(loggingScript.GetFormat("Slider released"));

        SliderSelected = false;
    }

    private float ScaleX(float originalX)
    {
        float videoDuration = (float)videoPlayer.length;
        RectTransform canvasRectTransform = anyCanvas.GetComponent<RectTransform>();

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3 bottomLeftCorner = canvasCorners[0];
        Vector3 topLeftCorner = canvasCorners[1];
        Vector3 topRightCorner = canvasCorners[2];
        Vector3 bottomRightCorner = canvasCorners[3];

        float scaledX = (originalX - bottomLeftCorner[0]) / (bottomRightCorner[0] - bottomLeftCorner[0]) * videoDuration;

        return scaledX;
    }

    private bool IsMouseWithinCanvasArea()
    {
        foreach (Canvas canvas in canvasList)
        {
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector2 mousePos = Input.mousePosition;
            bool inCanvas = RectTransformUtility.RectangleContainsScreenPoint(canvasRectTransform, mousePos, Camera.main);

            if (inCanvas)
            {
                return true;
            }
        }

        return false;
    }


}
