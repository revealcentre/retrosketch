using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// class for handling the eraser functionality
public class Eraser : MonoBehaviour
{
    public GameObject eraserCirclePrefab;
    private GameObject eraserCircle;
    private bool isEraserMode = false;
    private float eraserRadius;
    private GraphPlotter graphPlotter;
    public RectTransform canvas1;
    public RectTransform canvas2;
    public RectTransform canvas3;
    public RectTransform canvas4;
    public RectTransform canvas5;
    public Button eraserButton;
    public Color eraserColor1;
    public Color eraserColor2;
    public Color red;
    public float tolerance = 0.01f;
    public Logging loggingScript;

    void Start()
    {
        graphPlotter = FindObjectOfType<GraphPlotter>();
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (isEraserMode && Input.GetMouseButtonDown(0))
        {
            loggingScript.logList.Add(loggingScript.GetFormat("Started erasing", null, mousePosition.x, mousePosition.y));
        }

        if (isEraserMode && Input.GetMouseButtonUp(0))
        {
            loggingScript.logList.Add(loggingScript.GetFormat("Stopped erasing", null, mousePosition.x, mousePosition.y));
        }

        if (isEraserMode && Input.GetMouseButton(0))
        {
            if (eraserCircle != null)
            {
                // Rubber icon follows mouse
                eraserCircle.transform.position = mousePosition;
                LineRenderer[] lines = FindObjectsOfType<LineRenderer>();

                foreach (LineRenderer line in lines)
                {   
                    if (line.CompareTag("Line"))
                    {
                        if (line.positionCount == 0 || line.positionCount == 1 || line.positionCount == 2 || line.positionCount == 3)
                        {
                            Destroy(line.gameObject);
                        }

                        else if (LineIntersectsCircle(line, mousePosition, eraserRadius))
                        {   
                            Vector2 linePoint = FindClosestPointOnLine(line, mousePosition);

                            ErasePoint(line, linePoint);
                        }
                    }
                }

                GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
                GameObject[] annotations = GameObject.FindGameObjectsWithTag("Annotation");

                foreach (GameObject point in points)
                {
                    // Does a key point intersect the eraser circle?
                    if (PointIntersectsCircle(point.transform.position, mousePosition, eraserRadius))
                    {
                        if (points.Length > 0)
                        {
                            List<GameObject> validKp = new List<GameObject>();
                            
                            foreach (GameObject kp in points)
                            {
                                if (kp != point)
                                {
                                    float kpPos = kp.transform.position.x;

                                    if (Mathf.Abs(kpPos - point.transform.position.x) < tolerance)
                                    {
                                        validKp.Add(kp);
                                    }
                                }
                                
                            }
                            
                            foreach (GameObject k in validKp)
                            {
                                red.a = 1f;
                                k.GetComponent<SpriteRenderer>().color = red;
                                
                            }

                            if (validKp.Count == 0)
                            {
                                foreach (GameObject ann in annotations)
                                {
                                    
                                    if (Mathf.Abs(ann.transform.position.x - point.transform.position.x) < tolerance)
                                    {
                                        Destroy(ann);
                                        //Debug.Log("Annotation removed");
                                        loggingScript.logList.Add(loggingScript.GetFormat("Previous annotation deleted", null, ann.transform.position.x, 1000f));
                                    }
                        
                                }
                            }
                            
                        }
                        Destroy(point);

                        loggingScript.logList.Add(loggingScript.GetFormat("Key Point deleted", null, point.transform.position.x, point.transform.position.y));
                    }
                }
            }
        }
    }
    bool LineIntersectsCircle(LineRenderer line, Vector2 circleCenter, float circleRadius)
    {
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector2 startPoint = line.GetPosition(i);
            Vector2 endPoint = line.GetPosition(i + 1);

            if (LineIntersectsCircleSegment(startPoint, endPoint, circleCenter, circleRadius))
            {
                return true;
            }
        }

        return false;
    }

    bool LineIntersectsCircleSegment(Vector2 startPoint, Vector2 endPoint, Vector2 circleCenter, float circleRadius)
    {
        float sqrRadius = circleRadius * circleRadius;
        Vector2 lineDirection = endPoint - startPoint;
        Vector2 lineToCircle = circleCenter - startPoint;
        float dot = Vector2.Dot(lineToCircle, lineDirection);

        if (dot < 0f)
        {
            return lineToCircle.sqrMagnitude <= sqrRadius;
        }
        else if (dot > lineDirection.sqrMagnitude)
        {
            return (circleCenter - endPoint).sqrMagnitude <= sqrRadius;
        }
        else
        {
            float projectionFactor = dot / lineDirection.sqrMagnitude;
            Vector2 projection = startPoint + lineDirection * projectionFactor;
            return (circleCenter - projection).sqrMagnitude <= sqrRadius;
        }
    }

    bool PointIntersectsCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
    {
        return (point - circleCenter).sqrMagnitude <= circleRadius * circleRadius;
    }

    public void ToggleEraserMode()
    {
        isEraserMode = !isEraserMode;

        if (isEraserMode)
        {
            eraserCircle = Instantiate(eraserCirclePrefab, Vector3.zero, Quaternion.identity);
            eraserRadius = eraserCircle.GetComponent<SpriteRenderer>().bounds.extents.x;
            eraserColor1.a = 1f;
            eraserButton.image.color = eraserColor1;

            loggingScript.logList.Add(loggingScript.GetFormat("Eraser mode selected"));

        }
        else
        {
            Destroy(eraserCircle);
            eraserColor2.a = 1f;
            eraserButton.image.color = eraserColor2;

            loggingScript.logList.Add(loggingScript.GetFormat("Eraser mode deselected"));
        }  
    }

    public void ToggleEraserMode2()
    {
        if (isEraserMode)
        {
            isEraserMode = !isEraserMode;
            
            Destroy(eraserCircle);
            eraserColor2.a = 1f;
            eraserButton.image.color = eraserColor2;
        }
    }

    public bool GetIsEraserMode()
    {
        return isEraserMode;
    }

    void ErasePoint(LineRenderer line, Vector2 point)
    {
        if (line.positionCount > 0)
        {
            int pointIndex = -1;
            Transform originalParent = line.transform.parent;

            for (int i = 0; i < line.positionCount; i++)
            {
                Vector3 linePos = line.GetPosition(i);
                Vector2 linePos2D = new Vector2(linePos.x, linePos.y);
                if (linePos2D == point)
                {
                    pointIndex = i;
                    break;
                }
            }

            if (pointIndex != -1)
            {


                Vector3[] positionsBefore = new Vector3[pointIndex];
                Vector3[] positionsAfter = new Vector3[line.positionCount - pointIndex - 1];

                for (int i = 0; i < positionsBefore.Length; i++)
                {
                    positionsBefore[i] = line.GetPosition(i);
                }

                for (int i = pointIndex + 1; i < line.positionCount; i++)
                {
                    positionsAfter[i - pointIndex - 1] = line.GetPosition(i);
                }

                // New line before split
                GameObject firstLineSegment = Instantiate(graphPlotter.linePrefab);
                firstLineSegment.transform.SetParent(originalParent);
                LineRenderer firstLineRenderer = firstLineSegment.GetComponent<LineRenderer>();
                firstLineRenderer.positionCount = positionsBefore.Length;
                firstLineRenderer.SetPositions(positionsBefore);
                firstLineRenderer.tag = "Line";


                // New line after split
                GameObject secondLineSegment = Instantiate(graphPlotter.linePrefab);
                secondLineSegment.transform.SetParent(originalParent);
                LineRenderer secondLineRenderer = secondLineSegment.GetComponent<LineRenderer>();
                secondLineRenderer.positionCount = positionsAfter.Length;
                secondLineRenderer.SetPositions(positionsAfter);
                secondLineRenderer.tag = "Line";

                // Destroy original line
                Destroy(line.gameObject);
            }
        }
    }

    private Vector2 FindClosestPointOnLine(LineRenderer line, Vector2 point)
    {
        Vector2 closestPoint = Vector2.zero;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector2 startPoint = line.GetPosition(i);
            Vector2 endPoint = line.GetPosition(i + 1);

            Vector2 projectedPoint = ProjectPointOnLineSegment(startPoint, endPoint, point);
            float distance = Vector2.Distance(projectedPoint, point);

            if (distance < closestDistance)
            {
                closestPoint = projectedPoint;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }

    private Vector2 ProjectPointOnLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        Vector2 pointDirection = point - lineStart;
        float projection = Vector2.Dot(pointDirection, lineDirection);
        projection = Mathf.Clamp(projection, 0f, lineLength);

        return lineStart + lineDirection * projection;
    }

}