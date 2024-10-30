using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using SimpleFileBrowser;

// class handling the video uploader screen
public class VideoUploader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string filePathURL = "";
    public bool urlSet = false;
    public Camera camera2;

    void Awake()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
        ShowLoadDialogCoroutine();
    }

    void Update()
    {
        if (filePathURL != "" && urlSet == false)
        {
            videoPlayer.url = filePathURL;
            urlSet = true;
            videoPlayer.Prepare();
            videoPlayer.SetDirectAudioMute(0, false);
            videoPlayer.SetDirectAudioVolume(0, 1.0f);
            //camera2.cullingMask = 1 << 7;
        }
    }

    public bool GetUrlSet()
    {
        return urlSet;
    }

    void BrowseVideoFile()
    {
        string filePath = Application.streamingAssetsPath;

        if (Directory.Exists(filePath))
        {
            string[] videoFiles = Directory.GetFiles(filePath, "*.mp4");

            if (videoFiles.Length > 0)
            {
                string videoFileName = Path.GetFileName(videoFiles[0]);
                videoPlayer.url = Path.Combine(filePath, videoFileName);
            }
            else
            {
                //Debug.Log("No videos found");
            }
        }
        else
        {
            //Debug.Log("video folder not found");
            //Debug.Log(Application.streamingAssetsPath);
        }
    }

    // video selection window
    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        //Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
                Debug.Log(FileBrowser.Result[i]);

            filePathURL = FileBrowser.Result[0];
        }
    }
}
