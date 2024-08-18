using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageGalleryManager : MonoBehaviour
{
    [Tooltip("The prefab that will be instantiated for each image.")]
    [SerializeField] private GameObject imagePrefab;

    [Tooltip("The parent transform where the instantiated prefabs will be placed.")]
    [SerializeField] private RectTransform layoutParent;

    [Tooltip("The ImageHandler component responsible for handling the cached image.")]
    [SerializeField] private ImageHandler imageHandler;

    [Tooltip("The scroll rect for visibility checking.")]
    [SerializeField] private RectTransform scrollRect;

    [Tooltip("Margin for preloading images.")]
    [SerializeField] private float preloadMargin = 100f;

    [Tooltip("The button for deleting images.")]
    [SerializeField] private Button deleteButton;

    private List<GameObject> imageContainers = new();
    private List<string> imagePaths = new(); // Maintain a list of image paths
    private readonly string path = "/storage/emulated/0/Oculus/Screenshots/";
    private bool suppressToggleEvent = false;
    private GameObject selectedContainer = null;

    private void OnEnable()
    {
        deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        deleteButton.interactable = false; // Disable delete button initially
        LoadAllImages();
        StartCheckingVisibility();
    }

    private void OnDisable()
    {
        deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        StopCheckingVisibility();
    }

    private void LoadAllImages()
    {
        if (!Directory.Exists(path)) return;

        imagePaths = new List<string>(Directory.GetFiles(path, "*.jpg"));
        RefreshLayout();
    }

    private void RefreshLayout()
    {
        ClearLayout();

        foreach (var imagePath in imagePaths)
        {
            StartCoroutine(LoadImage(imagePath));
        }
    }

    private IEnumerator LoadImage(string imagePath)
    {
        var fileData = File.ReadAllBytes(imagePath);
        Texture2D tex = new(2, 2);

        if (tex.LoadImage(fileData))
        {
            InstantiateImage(tex, imagePath);
        }
        else
        {
            Debug.LogError("Failed to load image from path: " + imagePath);
        }

        yield return null;
    }

    private void InstantiateImage(Texture2D tex, string imagePath)
    {
        var newImage = Instantiate(imagePrefab, layoutParent, false);
        var imageComponent = newImage.GetComponentInChildren<Image>();
        var textComponent = newImage.GetComponentInChildren<TextMeshProUGUI>();

        if (imageComponent && textComponent)
        {
            imageComponent.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            textComponent.text = Path.GetFileName(imagePath);

            var toggle = newImage.GetComponentInChildren<Toggle>();
            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(isOn, newImage, tex, imagePath));

            imageContainers.Insert(0, newImage); // Add the new image at the top of the list
            newImage.transform.SetSiblingIndex(0); // Set the new image to the top of the layout
        }
        else
        {
            Debug.LogError("Prefab does not have the necessary components.");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutParent);
    }

    private void ClearLayout()
    {
        foreach (var container in imageContainers)
        {
            Destroy(container);
        }
        imageContainers.Clear();
    }

    private void StartCheckingVisibility()
    {
        InvokeRepeating(nameof(CheckVisibilityOfContainers), 0.5f, 0.5f);
    }

    private void StopCheckingVisibility()
    {
        CancelInvoke(nameof(CheckVisibilityOfContainers));
    }

    private void CheckVisibilityOfContainers()
    {
        foreach (var container in imageContainers)
        {
            container.SetActive(IsContainerVisible(container));
        }
    }

    private bool IsContainerVisible(GameObject container)
    {
        var corners = new Vector3[4];
        container.GetComponent<RectTransform>().GetWorldCorners(corners);

        var maxY = corners[1].y;
        var minY = corners[3].y;

        var viewMaxY = scrollRect.TransformPoint(new Vector3(0, scrollRect.rect.yMax + preloadMargin, 0)).y;
        var viewMinY = scrollRect.TransformPoint(new Vector3(0, scrollRect.rect.yMin - preloadMargin, 0)).y;

        return minY <= viewMaxY && maxY >= viewMinY;
    }

    private void OnToggleValueChanged(bool isOn, GameObject container, Texture2D texture, string imagePath)
    {
        if (suppressToggleEvent) return;

        if (isOn)
        {
            if (selectedContainer != null && selectedContainer != container)
            {
                var prevToggle = selectedContainer.GetComponentInChildren<Toggle>();
                if (prevToggle != null)
                {
                    suppressToggleEvent = true;
                    prevToggle.isOn = false;
                    suppressToggleEvent = false;
                }
            }

            selectedContainer = container;
            imageHandler.cachedTexture = texture;
            StartCoroutine(imageHandler.FadeImage(texture));
            deleteButton.interactable = true;
        }
        else if (selectedContainer == container)
        {
            selectedContainer = null;
            deleteButton.interactable = false; // Disable delete button when no image is selected
        }
    }

    private void OnDeleteButtonClicked()
    {
        if (selectedContainer != null)
        {
            var toggle = selectedContainer.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                string imagePath = Path.Combine(path, toggle.GetComponentInChildren<TextMeshProUGUI>().text);
                MarkImageForDeletion(selectedContainer, imagePath);
            }
        }
    }

    private void MarkImageForDeletion(GameObject container, string imagePath)
    {
        StartCoroutine(DeleteImage(container, imagePath));
    }

    private IEnumerator DeleteImage(GameObject container, string imagePath)
    {
        yield return StartCoroutine(FadeOut(container));

        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
            Debug.Log($"File deleted: {imagePath}");
        }
        else
        {
            Debug.LogWarning($"File not found: {imagePath}");
        }

        imageContainers.Remove(container);
        imagePaths.Remove(imagePath); // Remove the image path from the list
        Destroy(container); // Ensure the entire object is deleted

        deleteButton.interactable = false; // Disable delete button after deletion

        // Re-enable the parent object to force the layout update
        layoutParent.gameObject.SetActive(false);
        yield return null; // Wait for one frame
        layoutParent.gameObject.SetActive(true);

        if (imageContainers.Count > 0)
        {
            // Select the next image
            var nextContainer = imageContainers[0];
            var nextToggle = nextContainer.GetComponentInChildren<Toggle>();
            if (nextToggle != null)
            {
                suppressToggleEvent = true;
                nextToggle.isOn = true;
                suppressToggleEvent = false;

                var nextImageComponent = nextContainer.GetComponentInChildren<Image>();
                if (nextImageComponent != null)
                {
                    var nextTexture = nextImageComponent.sprite.texture;
                    imageHandler.cachedTexture = nextTexture;
                    StartCoroutine(imageHandler.FadeImage(nextTexture));
                }
            }
        }
        else
        {
            imageHandler.SetDefaultImage();
        }
    }

    private IEnumerator FadeOut(GameObject container)
    {
        var canvasGroup = container.GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = container.AddComponent<CanvasGroup>();
        }

        for (float t = 1f; t >= 0f; t -= Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
    }

    public void AddNewImage(string imagePath)
    {
        imagePaths.Add(imagePath); // Add the new image path to the list
        StartCoroutine(AddNewImageCoroutine(imagePath)); // Refresh the layout after adding the new image
    }

    private IEnumerator AddNewImageCoroutine(string imagePath)
    {
        yield return StartCoroutine(LoadImage(imagePath));

        // Re-enable the parent object to force the layout update
        layoutParent.gameObject.SetActive(false);
        yield return null; // Wait for one frame
        layoutParent.gameObject.SetActive(true);
    }
}
