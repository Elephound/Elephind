using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using OVRSimpleJSON;
using Oculus.Voice;
using UnityEngine.Serialization;
using TMPro;

public class ScreenshotSender : MonoBehaviour
{
    public UnityEvent onRequestSent;
    public UnityEvent<string> onResponseReceived;

    [SerializeField] string _baseAdress;

    public void SendImageToBackend(Texture2D image) => StartCoroutine(SendImageRequest(image));

    private IEnumerator SendImageRequest(Texture2D image)
    {
        var imageBytes = image.EncodeToJPG();
        var base64Image = Convert.ToBase64String(imageBytes);
        var payloadJson = PreparePayload(base64Image);

        var request = new UnityWebRequest(_baseAdress, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(payloadJson);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

       // request.SetRequestHeader("Content-Type", "application/json");
       // request.SetRequestHeader("Authorization", "Bearer " + APIKey);

        onRequestSent?.Invoke();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error sending request: {request.error}. Response code: {request.responseCode}");
        }
        else
        {
          /*  var jsonResponse = JSON.Parse(request.downloadHandler.text);
            var contentBody = jsonResponse["choices"][0]["message"]["content"].Value;

            onResponseReceived?.Invoke(contentBody);
            print(contentBody); */
        }
    }

    private string PreparePayload(string base64Image)
    {
        return $"{base64Image}";
    }
}