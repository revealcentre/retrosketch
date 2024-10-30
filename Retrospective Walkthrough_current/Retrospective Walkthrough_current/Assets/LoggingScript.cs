using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

// class for handling custom logging
public class Logging : MonoBehaviour
{
    public List<string> logList = new List<string>();
    public VideoPlayer videoPlayer;

    public void ExportLogs()
    {
        string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
        string path = "Custom_logs_" + dateTime + ".csv";
        string fileText = "frame, datetime, event, canvas, X, Y, mouseX, mouseY, video time\n";

        foreach (string item in logList)
        {
            fileText += (item + "\n");
        }

        File.WriteAllText(path, fileText);
    }

    public string GetFormat(string log, Canvas canvas=null, float xCoordinate=1000f, float yCoordinate=1000f)
    {
        string dateTime = System.DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.fff");
        int frame = Time.frameCount;

        string canvasString = "";
        if (canvas != null)
        {
            canvasString = canvas.name;
        }

        string xCoordinateString = "";
        string mouseXString = "";
        if (xCoordinate != 1000f && canvas != null)
        {
            xCoordinateString = ScaleX(xCoordinate, canvas).ToString();
        }
        if (xCoordinate != 1000f)
        {
            mouseXString = xCoordinate.ToString();
        }

        string yCoordinateString = "";
        string mouseYString = "";
        if (yCoordinate != 1000f && canvas != null)
        {
            yCoordinateString = ScaleY(yCoordinate, canvas).ToString();
        }
        if (yCoordinate != 1000f)
        {
            mouseYString = yCoordinate.ToString();
        }


        return frame.ToString() + "," + dateTime + "," + log + "," + canvasString + "," + xCoordinateString + "," + yCoordinateString + "," + mouseXString + "," + mouseYString + "," + videoPlayer.time.ToString();

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
}
