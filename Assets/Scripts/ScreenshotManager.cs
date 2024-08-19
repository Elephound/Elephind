using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Android;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    
    [HideInInspector] public Texture2D cachedTexture;

    [SerializeField] private bool deleteAfterProcessing;

    [SerializeField] private ScreenshotSender _screenshotSender;

    private readonly string path = "/storage/emulated/0/Oculus/Screenshots/";
    private FileSystemWatcher fileSystemWatcher;
    private readonly Queue<string> imageQueue = new();

    // Get the path to the persistent data directory
    string persistentDataPath;



    private void Awake()
    {
        if (Application.isEditor)
        {
            CacheEmptyTexture();
        }
        else
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            else
            {
                InitializeFileSystemWatcher();
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
        }


        persistentDataPath = Application.persistentDataPath;
    }

    private void OnDestroy()
    {
        if (fileSystemWatcher == null) return;
        fileSystemWatcher.Created -= OnNewImageCreated;
        fileSystemWatcher.Dispose();
    }

    private void CacheEmptyTexture()
    {
        var emptyTexture = new Texture2D(2, 2);
        emptyTexture.SetPixels(new[] { Color.clear, Color.clear, Color.clear, Color.clear });
        emptyTexture.Apply();
        cachedTexture = emptyTexture;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (Application.isEditor) return;

        if (hasFocus && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            InitializeFileSystemWatcher();
        }
    }

    private void InitializeFileSystemWatcher()
    {
        if (fileSystemWatcher != null) return;
        fileSystemWatcher = new FileSystemWatcher
        {
            Path = path,
            Filter = "*.jpg",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        Debug.LogWarning("fileSystemWatcherCreateD");

        fileSystemWatcher.Created += OnNewImageCreated;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void OnNewImageCreated(object sender, FileSystemEventArgs e)
    {
        print("New image detected: " + e.FullPath);

        lock (imageQueue)
        {
            imageQueue.Enqueue(e.FullPath);
        }
    }

    private void Update()
    {
        if (Application.isEditor) return;

        lock (imageQueue)
        {
            if (imageQueue.Count <= 0) return;
        }
        string imagePath;

        lock (imageQueue)
        {
            imagePath = imageQueue.Dequeue();
        }

        if (imagePath == null) return;
        StartCoroutine(ProcessNewImage(imagePath));
    }

    private IEnumerator ProcessNewImage(string imagePath)
    {

        var fileData = File.ReadAllBytes(imagePath);
        Texture2D tex = new(2, 2);
        bool succesful = StorageContainerManager.Instance.UpdateContainerScreenshot(tex);
        Debug.LogError("yo");
        if(!succesful)
            Debug.LogError("Could not store Screenshot, there was no active Storage");

        SendImage(fileData);

        if (tex.LoadImage(fileData))
        {

            //TODO not only send image but also storage ID and session ID
            // _screenshotSender?.SendImageToBackend(tex);
            StoreImage(tex);

            cachedTexture = tex;
            if (deleteAfterProcessing)
            {
                DeleteImage(imagePath);
            }
        }
        else
        {
            Debug.LogError("Failed to load image from path: " + imagePath);
        }

        yield return null;
    }

    private void SendImage(byte[] fileData)
    {

    }

    private void DeleteImage(string imagePath)
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += permissionName =>
            {
                if (permissionName == Permission.ExternalStorageWrite)
                {
                    DeleteFile(imagePath);
                }
            };
            callbacks.PermissionDenied += permissionName =>
            {
                if (permissionName == Permission.ExternalStorageWrite)
                {
                    Debug.LogWarning("Permission to write external storage was denied.");
                }
            };
            callbacks.PermissionDeniedAndDontAskAgain += permissionName =>
            {
                if (permissionName == Permission.ExternalStorageWrite)
                {
                    Debug.LogWarning("Permission to write external storage was denied and don't ask again.");
                }
            };
            Permission.RequestUserPermission(Permission.ExternalStorageWrite, callbacks);
        }
        else
        {
            DeleteFile(imagePath);
        }
    }

    private void DeleteFile(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
            print($"File deleted: {imagePath}");
        }
        else
        {
            Debug.LogWarning($"File not found: {imagePath}");
        }
    }

    void StoreImage(Texture2D tex)
    {
        byte[] imageBytes = tex.EncodeToJPG(); 

        // Specify the file name
        string fileName = "MyImage.jpg"; // Use Storage Container ID as name to reallocate

        string folder = "/ourImages";
        // Combine the path and file name
        string filePath = System.IO.Path.Combine(path+folder, fileName);

        // Write the image bytes to the file
        File.WriteAllBytes(filePath, imageBytes);

        // Optionally, you can log the path to check where the file was saved
        Debug.LogWarning($"Image saved to: {filePath}");
    }



}
