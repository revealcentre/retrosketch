using UnityEngine;
using UnityEngine.Video;
using TMPro;

// class for plotting grid background
public class GridGenerator : MonoBehaviour
{   
    public VideoPlayer videoPlayer;
    public int numRows = 10; // Emotion rating 0-10
    public float verticalLineInterval = 1f; // 1min intervals
    public float verticalLineInterval2 = 5f; // Bold line for 5min intervals
    public float width = 0.01f;
    public float width2 = 0.03f;
    public Color color;
    public TMP_FontAsset labelFont;
    public Color labelColor = Color.red;
    public Transform parent;
    private LineRenderer lineRenderer;
    public float labelFontSize = 0.39f;
    public Canvas referenceCanvas;

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepareCompleted;
    }

    private void OnVideoPrepareCompleted(VideoPlayer source)
    {
        float videoDuration = (float)videoPlayer.length;
        float verticalLineSpacing = videoDuration / (verticalLineInterval * 60f);
        float verticalLineSpacing2 = videoDuration / (verticalLineInterval2 * 60f);
        float rowHeight = 1f / numRows;
        float parentWidth = parent.lossyScale.x;
        float parentHeight = parent.lossyScale.y;

        Vector3 gridPosition = transform.localPosition;
        gridPosition.y = -0.5f; // Adjust grid position
        gridPosition.x = -0.5f;

        for (int i = 0; i <= numRows; i++)
        {
            float y = i * rowHeight;

            if (i == 5)
            {
                DrawHorizontalLine(y, parentWidth, parentHeight, gridPosition, width2);
            }
            else
            {
                DrawHorizontalLine(y, parentWidth, parentHeight, gridPosition, width);
            }

            // Axis labels
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(referenceCanvas.transform);

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.font = labelFont;
            label.color = labelColor;
            label.alignment = TextAlignmentOptions.Right;
            label.text = (i).ToString();
            label.fontSize = labelFontSize;

            RectTransform labelRectTransform = labelObj.GetComponent<RectTransform>();
            labelRectTransform.anchorMin = new Vector2(0f, y);
            labelRectTransform.anchorMax = new Vector2(0f, y);
            labelRectTransform.pivot = new Vector2(1f, 0.5f);
            // Adjust label position here
            labelRectTransform.anchoredPosition = new Vector2(-9f, -2f);
            labelRectTransform.localScale = new Vector3(1f / referenceCanvas.transform.lossyScale.x, 1f / referenceCanvas.transform.lossyScale.y, 1f);
        }

        for (int i = 0; i < verticalLineSpacing; i++)
        {
            float x = i * (1f / verticalLineSpacing);
            DrawVerticalLine(x, parentWidth, parentHeight, gridPosition, width);
        }

        for (int i = 0; i < verticalLineSpacing2; i++)
        {
            float x = i * (1f / verticalLineSpacing2);
            DrawVerticalLine(x, parentWidth, parentHeight, gridPosition, width2);

            if (i > 0)
            {
                GameObject labelObj = new GameObject("Label");
                labelObj.transform.SetParent(referenceCanvas.transform);

                TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
                label.font = labelFont;
                label.color = labelColor;
                label.alignment = TextAlignmentOptions.Right;
                label.text = (i * verticalLineInterval2).ToString("0") + " min";
                label.fontSize = labelFontSize;

                RectTransform labelRectTransform = labelObj.GetComponent<RectTransform>();
                labelRectTransform.anchorMin = new Vector2(x, 0f);
                labelRectTransform.anchorMax = new Vector2(x, 0f);
                labelRectTransform.pivot = new Vector2(1f, 0.5f);
                // Adjust label position here
                labelRectTransform.anchoredPosition = new Vector2(17f, -5f);
                labelRectTransform.localScale = new Vector3(1f / referenceCanvas.transform.lossyScale.x, 1f / referenceCanvas.transform.lossyScale.y, 1f);
            }
        } 

    }

    public void DrawHorizontalLine(float y, float parentWidth, float parentHeight, Vector3 gridPosition, float width)
    {
        GameObject line = new GameObject();
        line.transform.SetParent(parent);
        line.transform.localPosition = gridPosition;

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = color;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPosition(0, new Vector3(0f * parentWidth, y * parentHeight, -1f));
        lineRenderer.SetPosition(1, new Vector3(1f * parentWidth, y * parentHeight, -1f));
    }

    public void DrawVerticalLine(float x, float parentWidth, float parentHeight, Vector3 gridPosition, float width)
    {
        GameObject line = new GameObject();
        line.transform.SetParent(parent);
        line.transform.localPosition = gridPosition;
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = color;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPosition(0, new Vector3(x * parentWidth, 0f * parentHeight, -1f));
        lineRenderer.SetPosition(1, new Vector3(x * parentWidth, 1f * parentHeight, -1f));
    }

}
