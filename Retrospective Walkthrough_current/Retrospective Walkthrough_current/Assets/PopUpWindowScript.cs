using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

// class handling the export button and data file generation/validation
public class PopUpWindow : MonoBehaviour
{   
    public GameObject popupPanel;
    private bool isExportMode = false;
    public Canvas canvas2;
    public List<Canvas> canvasList;
    public Camera camera2;
    public VideoPlayer videoPlayer;
    public float xProximity = 1.5f;
    public GameObject linePassed;
    public GameObject pointPassed;
    public GameObject lineFailed;
    public GameObject pointFailed;
    public float verticalLineInterval = 5f;
    public RenderTexture renderTexture;
    private Texture2D screenshot;
    public float tolerance = 0.01f;
    public Logging loggingScript;


    void Start()
    {
        popupPanel.SetActive(false);
    }

    public void ShowPopupWindow()
    {
        CaptureScreenshot();
        popupPanel.SetActive(true);
        isExportMode = true;
        canvas2.enabled = false;

        foreach (Canvas graph in canvasList)
        {
            graph.enabled = false;
        }
        Validate();
        // camera2.cullingMask = 1 << 3;

        loggingScript.logList.Add(loggingScript.GetFormat("Export window opened"));

    }

    public void ClosePopupWindow()
    {
        popupPanel.SetActive(false);
        isExportMode = false;
        //Debug.Log(isExportMode);
        canvas2.enabled = true;
        foreach (Canvas graph in canvasList)
        {
            graph.enabled = true;
        }
        lineFailed.SetActive(false);
        linePassed.SetActive(false);
        pointFailed.SetActive(false);
        pointPassed.SetActive(false);
        // camera2.cullingMask = 1 << 5;

        loggingScript.logList.Add(loggingScript.GetFormat("Export window closed"));
    }

    public bool GetIsExportMode()
    {
        return isExportMode;
    }

    public void ExportData()
    {
        string csvData = CSVGenerator();
        string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
        string csvName = "line_data_" + dateTime + ".csv";
        SaveCSVToFile(csvData, csvName);

        string csvData2 = CSVGenerator2();
        string csvName2 = "keypoint_data_" + dateTime + ".csv";
        SaveCSVToFile(csvData2, csvName2);

        string pngName = "screenshot_" + dateTime + ".png";
        SaveScreenshot(pngName);
    }

    private (List<float[]>, List<float[]>, List<string[]>) GenerateData()
    {
        int videoDuration = (int)System.Math.Floor(videoPlayer.length);

        float[] joyLine = new float[videoDuration];
        float[] fearLine = new float[videoDuration];
        float[] relaxationLine = new float[videoDuration];
        float[] boredomLine = new float[videoDuration];
        float[] presenceLine = new float[videoDuration];

        float[] joyPoint = new float[videoDuration];
        float[] fearPoint = new float[videoDuration];
        float[] relaxationPoint = new float[videoDuration];
        float[] boredomPoint = new float[videoDuration];
        float[] presencePoint = new float[videoDuration];

        string[] annotation = new string[videoDuration];

        foreach (Canvas canvas in canvasList)
        {
            LineRenderer[] lines = canvas.GetComponentsInChildren<LineRenderer>();
            
            foreach (LineRenderer line in lines)
            {
                if (line.CompareTag("Line"))
                {   
                    for (int i = 0; i < line.positionCount - 1; i++)
                    {
                        Vector3 point = line.GetPosition(i);
                        float scaledY = ScaleY(point.y, canvas);
                        float scaledX = ScaleX(point.x, canvas);
                        Vector2 scaledPoint = new Vector2(scaledX, scaledY);

                        Vector3 point2 = line.GetPosition(i+1);
                        float scaledY2 = ScaleY(point2.y, canvas);
                        float scaledX2 = ScaleX(point2.x, canvas);
                        Vector2 scaledPoint2 = new Vector2(scaledX2, scaledY2);

                        int samplesCount = (int)(System.Math.Floor(scaledX2) - System.Math.Ceiling(scaledX));

                        // need to sample integer positions, just see which integers are between two floats and add to position in array
                        for (int j = 0; j < samplesCount; j++)
                        {
                            float target = (float)(System.Math.Ceiling(scaledX) + j);
                            float t = (target - scaledX) / (scaledX2 - scaledX);
                            Vector2 samplePoint = Vector2.Lerp(scaledPoint, scaledPoint2, t);
                            
                            if (canvas.name == "Joy")
                            {
                                joyLine[Mathf.RoundToInt(samplePoint.x)] = samplePoint.y;
                            }
                            else if (canvas.name == "Fear")
                            {
                                fearLine[Mathf.RoundToInt(samplePoint.x)] = samplePoint.y;
                            }
                            else if (canvas.name == "Relaxation")
                            {
                                relaxationLine[Mathf.RoundToInt(samplePoint.x)] = samplePoint.y;
                            }
                            else if (canvas.name == "Boredom")
                            {
                                boredomLine[Mathf.RoundToInt(samplePoint.x)] = samplePoint.y;
                            }
                            else if (canvas.name == "Presence")
                            {
                                presenceLine[Mathf.RoundToInt(samplePoint.x)] = samplePoint.y;
                            }
                        }
                    }
                }
            }

            GameObject[] keypoints = GameObject.FindGameObjectsWithTag("Point");
            

            foreach (GameObject kp in keypoints)
            {
                if (kp.transform.IsChildOf(canvas.transform))
                {
                    Vector3 point = kp.transform.position;
                    float scaledY = ScaleY(point.y, canvas);
                    float scaledX = ScaleX(point.x, canvas);

                    if (canvas.name == "Joy")
                    {
                        joyPoint[Mathf.RoundToInt(scaledX)] = scaledY;
                    }
                    else if (canvas.name == "Fear")
                    {
                        fearPoint[Mathf.RoundToInt(scaledX)] = scaledY;
                    }
                    else if (canvas.name == "Relaxation")
                    {
                        relaxationPoint[Mathf.RoundToInt(scaledX)] = scaledY;
                    }
                    else if (canvas.name == "Boredom")
                    {
                        boredomPoint[Mathf.RoundToInt(scaledX)] = scaledY;
                    }
                    else if (canvas.name == "Presence")
                    {
                        presencePoint[Mathf.RoundToInt(scaledX)] = scaledY;
                    }
                }
            }
        }

        GameObject[] annotations = GameObject.FindGameObjectsWithTag("Annotation");

        foreach (GameObject icon in annotations)
        {
            string text = icon.GetComponent<TextMeshProUGUI>().text;
            float annPos = icon.transform.position.x;
            float scaledAnnPos = ScaleX(annPos, canvasList[0]);
            annotation[Mathf.RoundToInt(scaledAnnPos)] = text;
        }

        List<float[]> lineList = new List<float[]> { joyLine, fearLine, relaxationLine, boredomLine, presenceLine };
        List<float[]> pointList = new List<float[]> { joyPoint, fearPoint, relaxationPoint, boredomPoint, presencePoint };
        List <string[]> annotationList = new List<string[]> { annotation };

        return (lineList, pointList, annotationList);

    }

    private float ScaleY(float originalY, Canvas canvas)
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3 bottomLeftCorner = canvasCorners[0];
        Vector3 topLeftCorner = canvasCorners[1];
        Vector3 topRightCorner = canvasCorners[2];
        Vector3 bottomRightCorner = canvasCorners[3];

        float scaledY = (originalY - bottomLeftCorner[1]) / (topLeftCorner[1] - bottomLeftCorner[1]) * 10;

        return scaledY;
    }

    private float ScaleX(float originalX, Canvas canvas)
    {
        float videoDuration = (float)videoPlayer.length;
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3 bottomLeftCorner = canvasCorners[0];
        Vector3 topLeftCorner = canvasCorners[1];
        Vector3 topRightCorner = canvasCorners[2];
        Vector3 bottomRightCorner = canvasCorners[3];

        float scaledX = (originalX - bottomLeftCorner[0]) / (bottomRightCorner[0] - bottomLeftCorner[0]) * videoDuration;

        return scaledX;
    }

    private void SaveCSVToFile(string csvData, string fileName)
    {
        // string path = Path.Combine(Application.persistentDataPath, fileName);
        // Debug.Log(path);
        string path = fileName;
        File.WriteAllText(path, csvData);
    }

    private bool ValidateLine()
    {
        foreach (Canvas canvas in canvasList)
        {
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            Vector3 bottomLeftCorner = canvasCorners[0];
            Vector3 topLeftCorner = canvasCorners[1];
            Vector3 topRightCorner = canvasCorners[2];
            Vector3 bottomRightCorner = canvasCorners[3];

            LineRenderer[] lines = canvas.GetComponentsInChildren<LineRenderer>();
            List<LineRenderer> lineList = new List<LineRenderer>();

            foreach (LineRenderer line in lines)
            {
                if (line.CompareTag("Line") && line.positionCount >= 2)
                {
                    lineList.Add(line);
                }
            }

            int counter = lineList.Count - 1;
            bool startConnect = false;
            bool endConnect = false;

            foreach (LineRenderer line1 in lineList)
            {

                foreach (LineRenderer line2 in lineList)
                {

                    int positionCount1 = line1.positionCount;
                    int positionCount2 = line2.positionCount;

                    Vector3 start1 = line1.GetPosition(0);
                    Vector3 end1 = line1.GetPosition(positionCount1 - 1);
                    Vector3 start2 = line2.GetPosition(0);
                    Vector3 end2 = line2.GetPosition(positionCount2 - 1);

                    // check that the end of line1 is close to the start of line2
                    if (System.Math.Sqrt((Mathf.Abs(start2.x - end1.x) * Mathf.Abs(start2.x - end1.x)) + (Mathf.Abs(start2.y - end1.y) * Mathf.Abs(start2.y - end1.y))) <= xProximity)
                    {
                        counter -= 1;
                    }
                    // check that one of the lines is close to the start of the grid
                    if (Mathf.Abs(start1.x - bottomLeftCorner.x) <= xProximity)
                    {
                        startConnect = true;
                    }
                    // check that one of the lines is close to the end of the grid
                    if (Mathf.Abs(end1.x - bottomRightCorner.x) <= xProximity)
                    {
                        endConnect = true;
                    }

                }
            }
            // invalidate if any exceptions are found
            if (counter != 0 || startConnect == false || endConnect == false)
            {
                return false;
            }

        }
        loggingScript.logList.Add(loggingScript.GetFormat("Line validation passed"));
        return true;

    }

    private bool ValidatePoint()
    {
        foreach (Canvas canvas in canvasList)
        {
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            Vector3 bottomLeftCorner = canvasCorners[0];
            Vector3 topLeftCorner = canvasCorners[1];
            Vector3 topRightCorner = canvasCorners[2];
            Vector3 bottomRightCorner = canvasCorners[3];

            SpriteRenderer[] points = canvas.GetComponentsInChildren<SpriteRenderer>();
            List<SpriteRenderer> pointList = new List<SpriteRenderer>();

            foreach (SpriteRenderer point in points)
            {
                if (point.CompareTag("Point"))
                {
                    pointList.Add(point);
                }
            }

            float videoDuration = (float)videoPlayer.length;
            float verticalLineSpacing = videoDuration / (verticalLineInterval * 60f);
            Transform parent = canvas.transform;

            float parentWidth = bottomRightCorner.x - bottomLeftCorner.x;
            List<float> intervalList = new List<float>();
            intervalList.Add(bottomLeftCorner.x);
            for (int i = 0; i < verticalLineSpacing; i++)
            {
                float x = bottomLeftCorner.x + (i * (1f / verticalLineSpacing) * parentWidth);
                intervalList.Add(x);
            }
            intervalList.Add(bottomRightCorner.x);

            int counter = intervalList.Count - 3;
            //Debug.Log(counter);
            foreach (SpriteRenderer kp in pointList)
            {
                Vector3 coordinate = kp.transform.position;
                //Debug.Log(coordinate);
                int length = intervalList.Count - 1;

                for (int i = 0; i < length; i++)
                {
                    float x = coordinate.x;
                    //Debug.Log(intervalList[i]);
                    
                    
                    if (x > intervalList[i] && x <= intervalList[i + 1])
                    {
                        counter -= 1;
                        /*Debug.Log("accepted coordinate");
                        Debug.Log(coordinate);
                        Debug.Log(intervalList[i]);
                        Debug.Log(intervalList[i+1]);*/
                    }
                    
                }
            }
            if (counter > 0)
            {
                /*Debug.Log("failed point interval validation");
                Debug.Log(counter);
                Debug.Log(canvas);*/
                loggingScript.logList.Add(loggingScript.GetFormat("Failed key point interval validation", canvas));

                return false;
            }

            // validate that the points are on the lines
            LineRenderer[] lines = canvas.GetComponentsInChildren<LineRenderer>();
            List<LineRenderer> lineList = new List<LineRenderer>();

            foreach (LineRenderer line in lines)
            {
                if (line.CompareTag("Line") && line.positionCount >= 2)
                {
                    lineList.Add(line);
                }
            }

            int supercounter = pointList.Count;
            foreach (SpriteRenderer keyp in pointList)
            {
                int counter2 = 0;
                Vector3 kp = keyp.transform.position;
                foreach (LineRenderer line in lineList)
                {

                    for (int i = 0; i < line.positionCount; i++)
                    {
                        Vector2 lp = line.GetPosition(i);
                        if (System.Math.Sqrt((Mathf.Abs(kp.x - lp.x) * Mathf.Abs(kp.x - lp.x)) + (Mathf.Abs(kp.y - lp.y) * Mathf.Abs(kp.y - lp.y))) < tolerance)
                        {
                            counter2 += 1;
                            //Debug.Log("Point-Line connection found ");
                                
                        }
                    }
                }

                if (counter2 > 0)
                {
                    supercounter -= 1;
                }
                
            }

            if (supercounter != 0)
            {
                /*Debug.Log("Points not on line");
                Debug.Log(supercounter + canvas.name);*/
                loggingScript.logList.Add(loggingScript.GetFormat("Points found below/above line", canvas));
                return false;
            }


        }
        loggingScript.logList.Add(loggingScript.GetFormat("Point validation passed"));

        return true;

    }

    public void Validate()
    {
        (List<float[]> lineList, List<float[]> pointList, List<string[]> annotationList) = GenerateData();

        bool validateLineResult = ValidateLine();
        bool validatePointResult = ValidatePoint();


        if (validateLineResult == true && validatePointResult == true)
        {
            linePassed.SetActive(true);
            pointPassed.SetActive(true);
        }
        else
        {
            if (validateLineResult == false)
            {
                lineFailed.SetActive(true);
            }
            else if (validateLineResult == true)
            {
                linePassed.SetActive(true);
            }
            if (validatePointResult == false)
            {
                pointFailed.SetActive(true);
            }
            else if (validatePointResult == true)
            {
                pointPassed.SetActive(true);
            }
        }
    }

    public string CSVGenerator()
    {
        (List<float[]> lineList, List<float[]> pointList, List<string[]> annotationList) = GenerateData();

        // fix 0 values
        for (int i = 0; i < lineList.Count; i++)
        {
            for (int z = 0; z < lineList[i].Length - 1; z++)
            {
                if (lineList[i][z+1] == 0)
                {
                    lineList[i][z + 1] = lineList[i][z];
                }
            }
        }

        // generate csv
        string csvData = "Time (s),Joy Line,Joy KeyPoint,Fear Line,Fear KeyPoint,Relaxation Line,Relaxation KeyPoint,Boredom Line,Boredom KeyPoint,Presence Line,Presence KeyPoint, Annotation\n"; // CSV header row
        int length = lineList[0].Length;

        for (int j = 0; j < length; j++)
        {
            csvData += $"{j},{lineList[0][j]},{pointList[0][j]},{lineList[1][j]},{pointList[1][j]},{lineList[2][j]},{pointList[2][j]},{lineList[3][j]},{pointList[3][j]},{lineList[4][j]},{pointList[4][j]},{annotationList[0][j]}\n";
        }
        
        string cleanedCsvData = csvData.Replace(",0,", ",,");
        cleanedCsvData = cleanedCsvData.Replace("​", "");

        return cleanedCsvData;
    }

    // key point specific csv
    public string CSVGenerator2()
    {
        (List<float[]> lineList, List<float[]> pointList, List<string[]> annotationList) = GenerateData();

        string csvData2 = "Time (s),Joy KeyPoint,Fear KeyPoint,Relaxation KeyPoint,Boredom KeyPoint,Presence KeyPoint, Annotation\n"; // CSV header row

        for (int j = 0; j < pointList[0].Length; j++)
        {
            if (pointList[0][j] != 0)
            {
                csvData2 += $"{j},{pointList[0][j]},{pointList[1][j]},{pointList[2][j]},{pointList[3][j]},{pointList[4][j]},{annotationList[0][j]}\n";
            }
        }

        string cleanedCsvData2 = csvData2.Replace("​", "");

        return cleanedCsvData2;
    }

    private void CaptureScreenshot()
    {
        camera2.targetTexture = renderTexture;
        camera2.Render();

        screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        camera2.targetTexture = null;
        RenderTexture.active = null;
        //Destroy(renderTexture);
    }

    private void SaveScreenshot(string fileName)
    {
        if (screenshot != null)
        {
            byte[] imageData = screenshot.EncodeToPNG();
            if (imageData != null)
            {
                System.IO.File.WriteAllBytes(fileName, imageData);
            }
            else
            {
                Debug.Log("Screenshot failed to save");
            }
        }
        else
        {
            Debug.Log("Screenshot texture didn't save properly");
        }
    }

}
