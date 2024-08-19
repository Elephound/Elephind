using UnityEngine;
using UnityEngine.UI;
using System;

public class WebcamScreenshotCapture : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    public RawImage displayImage; // Assign the UI RawImage in the inspector to show the webcam feed
    public int screenshotWidth = 1920;  // Width of the screenshot (optional, can be adjusted)
    public int screenshotHeight = 1080; // Height of the screenshot (optional, can be adjusted)

    void Start()
    {
        // Initialize the webcam
        if (WebCamTexture.devices.Length > 0)
        {
            webcamTexture = new WebCamTexture();
            displayImage.texture = webcamTexture; // Display the webcam feed on the RawImage
            displayImage.material.mainTexture = webcamTexture; // Ensure the material uses the webcam texture
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("No webcam detected!");
        }
    }

    public string CaptureScreenshot()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            // Create a Texture2D from the webcam feed
            Texture2D screenshot = new Texture2D(webcamTexture.width, webcamTexture.height);
            screenshot.SetPixels(webcamTexture.GetPixels());
            screenshot.Apply();

            // Optionally, resize the screenshot
            if (screenshotWidth > 0 && screenshotHeight > 0)
            {
                Texture2D resizedScreenshot = ResizeTexture(screenshot, screenshotWidth, screenshotHeight);
                Destroy(screenshot);
                screenshot = resizedScreenshot;
            }

            // Convert the Texture2D to a JPG image
            byte[] jpgBytes = screenshot.EncodeToJPG();
            Destroy(screenshot);

            // Convert JPG to Base64
            string base64String = Convert.ToBase64String(jpgBytes);
            //Debug.Log($"Base64 String: {base64String}");
            return base64String;
        }
        else
        {
            Debug.LogError("Webcam is not available or not playing.");
        }
        return "";
    }

    private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    void Update()
    {
        // For testing purposes: capture screenshot when "S" key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureScreenshot();
        }
    }

    void OnDestroy()
    {
        // Stop the webcam when the script is destroyed
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
