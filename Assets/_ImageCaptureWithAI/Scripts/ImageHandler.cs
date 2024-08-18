using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class ImageHandler : MonoBehaviour
{
    [Tooltip("The Image component where the captured screenshot is displayed.")]
    [SerializeField] public Image displayImage;

    [Tooltip("The component that is responsible for processing the texture and sending it to OpenAI.")]
    [SerializeField] private OpenAIConnector openAIConnector;

    [Tooltip("The default image to display when no other image is selected.")]
    [SerializeField] private Sprite defaultImage;

    [Tooltip("The CanvasGroup component for fading in and out.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Whether to start the voice experience after taking an image.")]
    [SerializeField] private bool startVoiceExperienceAfterImage;
    [SerializeField] private bool deleteAfterProcessing;

    [HideInInspector] public Texture2D cachedTexture;

    private readonly string path = "/storage/emulated/0/Oculus/Screenshots/";
    private FileSystemWatcher fileSystemWatcher;
    private readonly Queue<string> imageQueue = new();

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
        }
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
        displayImage.sprite = Sprite.Create(emptyTexture, new Rect(0, 0, emptyTexture.width, emptyTexture.height), new Vector2(0.5f, 0.5f));
        openAIConnector.GetVoiceCommand();
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

        if (tex.LoadImage(fileData))
        {
            StartCoroutine(FadeImage(tex));
            cachedTexture = tex;
            if (startVoiceExperienceAfterImage)
            {
                openAIConnector.GetVoiceCommand();
            }
            FindObjectOfType<ImageGalleryManager>().AddNewImage(imagePath);

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

    public IEnumerator FadeImage(Texture2D newTexture)
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            canvasGroup.alpha = i;
            yield return null;
        }

        displayImage.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));

        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            canvasGroup.alpha = i;
            yield return null;
        }
    }

    public void SetDefaultImage()
    {
        displayImage.sprite = defaultImage;
        cachedTexture = null;
    }
}
