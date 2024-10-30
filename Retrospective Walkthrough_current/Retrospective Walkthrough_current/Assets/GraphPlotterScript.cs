using System.Collections.Generic;
using UnityEngine;

// class for handling all line drawing and point plotting functionality
public class GraphPlotter : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject linePrefab;
    private LineRenderer currentLineRenderer;
    public Canvas canvas;
    public List<Canvas> canvasList;
    private GameObject currentLine;
    private Vector3 lastPointPosition;
    public Eraser eraserModeScript;
    public AnnotationWindow annotationModeScript;
    public PopUpWindow popupWindowScript;
    public float xProximityThreshold = 0.1f;
    public float xsProximityThreshold = 0.05f;
    public float xlProximityThreshold = 0.1f;
    public float gridProximityThreshold = 0.5f;
    public float gridProximityThreshold2 = 0.1f;
    public float pointProximityThreshold = 0.1f;
    public float keypointProximityThreshold = 0.1f;
    public float sliderPointThreshold = 0.1f;
    public VideoUploader videoUploaderScript;
    public GameObject square;
    public SliderScript sliderScript;
    public KeyPoint keyPointScript;
    public Logging loggingScript;


    void Update()
    {   
        // Left click
        if (Input.GetMouseButtonDown(0) && IsMouseWithinCanvasArea() && !eraserModeScript.GetIsEraserMode() && !annotationModeScript.GetIsAnnotationMode() && !popupWindowScript.GetIsExportMode() && videoUploaderScript.GetUrlSet() && !sliderScript.IsSliderSelected() && !keyPointScript.IsKeyPointMode())
        {   
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lastPointPosition = mousePos;
            if (currentLineRenderer != null)
            {
                int lastPositionIndex = currentLineRenderer.positionCount - 1;
                lastPointPosition = currentLineRenderer.GetPosition(lastPositionIndex);
            }
            Vector3 dummy = new Vector3(100f, 100f, 0f);
            Vector3 gridResult = CloseToGridStart();
            Vector3 closeToLineResult = IsCloseToLineEnd();
            Vector3 closeToPointResult = ClosestPointCoordinate();

            loggingScript.logList.Add(loggingScript.GetFormat("Left click initiated", canvas, mousePos.x, mousePos.y));
            Debug.Log("Left click initiated");

            if (!IsCloseToExistingLines(currentLineRenderer) && gridResult == dummy && closeToPointResult == dummy)
            {
                currentLine = Instantiate(linePrefab, mousePos, Quaternion.identity);
                currentLine.transform.SetParent(canvas.transform);
                currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                currentLine.GetComponent<LineRenderer>().positionCount = 2;
                currentLine.GetComponent<LineRenderer>().SetPosition(0, mousePos);
                currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                currentLine.tag = "Line";

                //Debug.Log("Line renderer added");
                loggingScript.logList.Add(loggingScript.GetFormat("Line drawn", canvas, mousePos.x, mousePos.y));
                Debug.Log("Line Drawn");

            }
            // close to point
            else if (closeToPointResult != dummy)
            {
                if (mousePos.x < closeToPointResult.x)
                {
                    mousePos.x = closeToPointResult.x;
                }
                currentLine = Instantiate(linePrefab, closeToPointResult, Quaternion.identity);
                currentLine.transform.SetParent(canvas.transform);
                currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                currentLine.GetComponent<LineRenderer>().positionCount = 2;
                currentLine.GetComponent<LineRenderer>().SetPosition(0, closeToPointResult);
                currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                currentLine.tag = "Line";

                //Debug.Log("Line renderer added from existing point");
                loggingScript.logList.Add(loggingScript.GetFormat("Line drawn from point", canvas, mousePos.x, mousePos.y));
                Debug.Log("Line Drawn from point");
            }
            // close to line end
            else if (closeToLineResult != dummy)
            {   
                if (mousePos.x < closeToLineResult.x)
                {
                    mousePos.x = closeToLineResult.x; 
                }
                currentLine = Instantiate(linePrefab, closeToLineResult, Quaternion.identity);
                currentLine.transform.SetParent(canvas.transform);
                currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                currentLine.GetComponent<LineRenderer>().positionCount = 2;
                currentLine.GetComponent<LineRenderer>().SetPosition(0, closeToLineResult);
                currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                currentLine.tag = "Line";

                //Debug.Log("Line renderer added from existing");
                loggingScript.logList.Add(loggingScript.GetFormat("Line drawn from existing line", canvas, mousePos.x, mousePos.y));
                Debug.Log("Line Drawn from existing line");
            }
            
            else if (gridResult != dummy)
            {
                currentLine = Instantiate(linePrefab, mousePos, Quaternion.identity);
                currentLine.transform.SetParent(canvas.transform);
                currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                currentLine.GetComponent<LineRenderer>().positionCount = 2;
                currentLine.GetComponent<LineRenderer>().SetPosition(0, gridResult);
                currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePos);
                currentLine.tag = "Line";

                //Debug.Log("Line connected to start");
                //Debug.Log(gridResult.ToString());
                loggingScript.logList.Add(loggingScript.GetFormat("Line auto-connected to start", canvas, mousePos.x, mousePos.y));
                Debug.Log("Line auto connected to start");
            }
        }
        // left click sustained
        else if (Input.GetMouseButton(0) && IsMouseWithinCanvasArea() && !eraserModeScript.GetIsEraserMode() && !annotationModeScript.GetIsAnnotationMode() && !popupWindowScript.GetIsExportMode() && videoUploaderScript.GetUrlSet() && !sliderScript.IsSliderSelected() && !keyPointScript.IsKeyPointMode())
        {   
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector3 dummy = new Vector3(100f, 100f, 0f);
            Vector3 closeToLineResult = IsCloseToLineStart();
            Vector3 gridResult = CloseToGridEnd();
            Vector3 closeToPointResult = ClosestPointCoordinate();

            Debug.Log("Line Sustained");

            //if (!IsCloseToExistingLines(currentLineRenderer) && gridResult == dummy && closeToPointResult == dummy)
            //{
            //    Debug.Log("Sustained Line Close to Start");
            //    currentLine = Instantiate(linePrefab, mousePosition, Quaternion.identity);
            //    currentLine.transform.SetParent(canvas.transform);
            //    currentLineRenderer = currentLine.GetComponent<LineRenderer>();
            //    currentLine.GetComponent<LineRenderer>().positionCount = 2;
            //    currentLine.GetComponent<LineRenderer>().SetPosition(0, mousePosition);
            //    currentLine.GetComponent<LineRenderer>().SetPosition(1, mousePosition);
            //    currentLine.tag = "Line";

            //    loggingScript.logList.Add(loggingScript.GetFormat("Line drawn", canvas, mousePosition.x, mousePosition.y));
            //    Debug.Log("Line Drawn");

            //}


            if (!IsCloseToExistingLines(currentLineRenderer) && closeToPointResult == dummy)
            {

                if (currentLineRenderer != null && mousePosition.x > lastPointPosition.x + 0.01f)
                {
                    // Add points to line following mouse
                    lastPointPosition = mousePosition;
                    currentLineRenderer.positionCount++;
                    currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, mousePosition);
                }
            }
            // close to point
            else if (closeToPointResult != dummy && closeToPointResult.x > mousePosition.x)
            {

                if (currentLineRenderer != null && mousePosition.x > lastPointPosition.x)
                {
                    // Add points to line following mouse
                    lastPointPosition = mousePosition;
                    currentLineRenderer.positionCount++;
                    currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, closeToPointResult);
                }


                //Debug.Log("Line renderer attached to existing");
            }
            // close to line start
            else if (closeToLineResult != dummy)
            {
                if (currentLineRenderer != null && mousePosition.x > lastPointPosition.x)
                {
                    Debug.Log("Adding points to line following mouse");
                    // Add points to line following mouse
                    lastPointPosition = mousePosition;
                    currentLineRenderer.positionCount++;
                    currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, closeToLineResult);
                }

                //Debug.Log("Line renderer attached to existing");
            }

            else if (gridResult != dummy)
            {
                if (currentLineRenderer != null && mousePosition.x > lastPointPosition.x)
                {
                    // Add points to line following mouse
                    lastPointPosition = mousePosition;
                    currentLineRenderer.positionCount++;
                    currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, gridResult);
                    currentLineRenderer = null;
                }
                //Debug.Log("Line connected to end of grid");
                //Debug.Log(gridResult.ToString());
                loggingScript.logList.Add(loggingScript.GetFormat("Line auto-connected to end", canvas, mousePosition.x, mousePosition.y));
            }
        }
        // left click released
        else if (Input.GetMouseButtonUp(0) && IsMouseWithinCanvasArea() && !eraserModeScript.GetIsEraserMode() && !annotationModeScript.GetIsAnnotationMode() && !popupWindowScript.GetIsExportMode() && videoUploaderScript.GetUrlSet() && !sliderScript.IsSliderSelected() && !keyPointScript.IsKeyPointMode())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Line - released");
            if (!IsCloseToExistingLines(currentLineRenderer))
            {
                if (currentLineRenderer != null && (currentLineRenderer.positionCount == 1 || currentLineRenderer.positionCount == 2))
                {
                    Destroy(currentLine.gameObject);
                }
                currentLine = null;
                currentLineRenderer = null;

                loggingScript.logList.Add(loggingScript.GetFormat("Line finished drawing", canvas, mousePos.x, mousePos.y));
            }
        }

        // keyPoint mode
        else if (Input.GetMouseButtonDown(0) && IsMouseWithinCanvasArea() && keyPointScript.IsKeyPointMode() && !eraserModeScript.GetIsEraserMode() && !annotationModeScript.GetIsAnnotationMode() && !popupWindowScript.GetIsExportMode() && videoUploaderScript.GetUrlSet() && !sliderScript.IsSliderSelected())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            mousePos.z = -0.1f;
            Vector3 sliderResult = CloseToSlider(mousePos);
            bool illegal = IllegalPoints(mousePos);

            Vector3 dummy = new Vector3(100f, 100f, 0f);

            if (hit.collider == null && !illegal)
            {   
                if (sliderResult != dummy)
                {
                    GameObject newPoint = Instantiate(pointPrefab, sliderResult, Quaternion.identity);
                    newPoint.transform.SetParent(canvas.transform);
                    newPoint.tag = "Point";
                }
                else if (ClosestLineCoordinate() != dummy)
                {
                    GameObject newPoint = Instantiate(pointPrefab, ClosestLineCoordinate(), Quaternion.identity);
                    newPoint.transform.SetParent(canvas.transform);
                    newPoint.tag = "Point";
                }
                else
                {
                    GameObject newPoint = Instantiate(pointPrefab, mousePos, Quaternion.identity);
                    newPoint.transform.SetParent(canvas.transform);
                    newPoint.tag = "Point";
                }

                loggingScript.logList.Add(loggingScript.GetFormat("Key Point plotted", canvas, mousePos.x, mousePos.y));
            }
            mousePos.z = 0f;
        }
    }

    // Method to clear the graph if needed
    public void ClearGraph()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        foreach (GameObject point in points)
        {
            Destroy(point);
        }

        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
    }

    // Check if the mouse is within the correct canvas area
    private bool IsMouseWithinCanvasArea()
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 mousePos = Input.mousePosition;

        return RectTransformUtility.RectangleContainsScreenPoint(canvasRectTransform, mousePos, Camera.main);
    }

    private bool IsCloseToExistingLines(LineRenderer currentLineRenderer)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (ChooseList().Count > 0)
        {
            foreach (LineRenderer line in ChooseList())
            {
            if (line != null && line != currentLineRenderer)
                {
                    int positionCount = line.positionCount;
                    if (positionCount >= 2)
                    {
                        Vector3 start = line.GetPosition(0);
                        Vector3 end = line.GetPosition(positionCount - 1);

                        if ((Mathf.Abs(point.x - start.x) <= xProximityThreshold) || (Mathf.Abs(point.x - end.x) <= xProximityThreshold) || (point.x > start.x && point.x < end.x))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
        
    }

    private Vector3 IsCloseToLineEnd()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ChooseList().Count > 0)
        {
            foreach (LineRenderer line in ChooseList())
            {
            if (line != null)
                {
                    int positionCount = line.positionCount;
                    if (positionCount >= 2)
                    {
                        Vector3 start = line.GetPosition(0);
                        Vector3 end = line.GetPosition(positionCount - 1);

                        if ((Mathf.Abs(point.x - end.x) <= xlProximityThreshold))
                        {
                            return end;
                        }
                    }
                }
            }
        }

        return new Vector3(100f, 100f, 0f);
    }

    private Vector3 IsCloseToLineStart()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ChooseList().Count > 0)
        {
            foreach (LineRenderer line in ChooseList())
            {
                if (line != null)
                {
                    int positionCount = line.positionCount;
                    if (positionCount >= 2)
                    {
                        Vector3 start = line.GetPosition(0);
                        Vector3 end = line.GetPosition(positionCount - 1);

                        if (((start.x - point.x) <= xsProximityThreshold) && ((start.x - point.x) > 0))
                        {
                            return start;
                        }
                    }
                }
            }
        }

        return new Vector3(100f, 100f, 0f);
    }

    private Vector3 ClosestLineCoordinate()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ChooseList().Count > 0)
        {
            double mindistance = 100.0;
            Vector3 mincoords = new Vector3(100f, 100f, 0f);

            foreach (LineRenderer line in ChooseList())
            {
                if (line != null)
                {
                    int positionCount = line.positionCount;
                    Vector3 start = line.GetPosition(0);
                    Vector3 end = line.GetPosition(positionCount - 1);

                    if (positionCount >= 2)
                    {
                        for (int position = 0; position < positionCount; position++)
                        {
                            Vector3 pos = line.GetPosition(position);

                            double distance = System.Math.Sqrt((Mathf.Abs(point.x - pos.x) * Mathf.Abs(point.x - pos.x)) + (Mathf.Abs(point.y - pos.y) * Mathf.Abs(point.y - pos.y)));

                            if (distance < mindistance)
                            {
                                mindistance = distance;
                                mincoords = new Vector3(pos.x, pos.y, -0.1f);
                            }
                        }
                    }
                }
                if (mindistance <= pointProximityThreshold)
                {
                    return mincoords;
                }
            }
        }

        //Debug.Log("No closest line found");
        return new Vector3(100f, 100f, 0f);
    }

    private Vector3 ClosestPointCoordinate()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (ChoosePoint().Count > 0)
        {
            double mindistance = 100.0;
            Vector3 mincoords = new Vector3(100f, 100f, 0f);

            foreach (SpriteRenderer kp in ChoosePoint())
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
                if (mindistance <= keypointProximityThreshold)
                {
                    return mincoords;
                }
            }
        }

        //Debug.Log("No closest point found");
        return new Vector3(100f, 100f, 0f);
    }

    // Too close to existing point
    private bool IllegalPoints(Vector3 mousePosition)
    {
        if (ChoosePoint().Count > 0)
        {
            foreach (SpriteRenderer kp in ChoosePoint())
            {
                if (kp != null)
                {
                    Vector2 pos = kp.transform.position;

                    if (Mathf.Abs(mousePosition.x - pos.x) < xProximityThreshold)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private List<LineRenderer> ChooseList()
    {
        LineRenderer[] lines = canvas.GetComponentsInChildren<LineRenderer>();
        List<LineRenderer> lineList = new List<LineRenderer>();

        foreach (LineRenderer line in lines)
        {
            if (line.CompareTag("Line"))
            {
                lineList.Add(line);
            }
        }

        return lineList;
    }

    private List<SpriteRenderer> ChoosePoint()
    {
        SpriteRenderer[] points = canvas.GetComponentsInChildren<SpriteRenderer>();
        List<SpriteRenderer> pointList = new List<SpriteRenderer>();

        foreach (SpriteRenderer kp in points)
        {
            if (kp.CompareTag("Point"))
            {
                pointList.Add(kp);
            }
        }

        return pointList;
    }

    private Vector2 CloseToGridStart()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3 bottomLeftCorner = canvasCorners[0];
        Vector3 topLeftCorner = canvasCorners[1];
        Vector3 topRightCorner = canvasCorners[2];
        Vector3 bottomRightCorner = canvasCorners[3];

        if (Mathf.Abs(point.x - bottomLeftCorner.x) <= gridProximityThreshold)
        {
            return new Vector3(bottomLeftCorner.x, point.y, 0f);
        }

        return new Vector3(100f, 100f, 0f);
    }

    private Vector3 CloseToGridEnd()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        Vector3 bottomLeftCorner = canvasCorners[0];
        Vector3 topLeftCorner = canvasCorners[1];
        Vector3 topRightCorner = canvasCorners[2];
        Vector3 bottomRightCorner = canvasCorners[3];

        if (Mathf.Abs(point.x - bottomRightCorner.x) <= gridProximityThreshold2)
        {
            return new Vector3(bottomRightCorner.x, point.y, 0f);
        }

        return new Vector3(100f, 100f, 0f);
    }

    private Vector3 CloseToSlider(Vector3 mousePosition)
    {
        float sliderX = square.transform.position.x;
        if (Mathf.Abs(mousePosition.x - sliderX) < sliderPointThreshold)
        {
            return new Vector3(sliderX, mousePosition.y, mousePosition.z);
        }

        return new Vector3(100f, 100f, 0f);
    }

}