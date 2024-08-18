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

public class OpenAIConnector : MonoBehaviour
{
    [SerializeField] private string APIKey = "YOUR_API_KEY";

    [Header("Managers")]
    [Tooltip("The wit AppVoiceExperience, responsible for capturing the voice command.")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;

    [FormerlySerializedAs("imageLoader")]
    [Tooltip("The component responsible for loading the image from the Meta Quest storage.")]
    [SerializeField] private ImageHandler imageHandler;

    [Tooltip("The component responsible for speaking the response from OpenAI.")]
    [SerializeField] private ResponseTTS voiceAssistant;

    [Tooltip("Text component that is responsible for displaying the input transcript.")]
    [SerializeField] private TextMeshProUGUI inputTranscriptText;

    [Space(20)]
    [Tooltip("Event being fired when the response from OpenAI has arrived.")]
    [SerializeField] private string baseCommand = "";
    
    [Space(20)]
    [Tooltip("Event being fired when the image is being sent to OpenAI.")]
    public UnityEvent onRequestSent;

    [Tooltip("Event being fired when the response from OpenAI has arrived.")]
    public UnityEvent<string> onResponseReceived;

    private string gptVisionModel = "gpt-4o";
    private string command = "";

    private void Awake()
    {
        appVoiceExperience.TranscriptionEvents.OnFullTranscription.AddListener(GetTranscriptAndSendImage);
    }

    private void OnDestroy()
    {
        appVoiceExperience.TranscriptionEvents.OnFullTranscription.RemoveListener(GetTranscriptAndSendImage);
    }

    private void SendImageToOpenAI() => StartCoroutine(SendImageRequest(imageHandler.cachedTexture));

    private IEnumerator SendImageRequest(Texture2D image)
    {
        var imageBytes = image.EncodeToJPG();
        var base64Image = Convert.ToBase64String(imageBytes);
        var payloadJson = PreparePayload(base64Image);

        var request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(payloadJson);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + APIKey);

        onRequestSent?.Invoke();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error sending request: {request.error}. Response code: {request.responseCode}");
        }
        else
        {
            var jsonResponse = JSON.Parse(request.downloadHandler.text);
            var contentBody = jsonResponse["choices"][0]["message"]["content"].Value;

            onResponseReceived?.Invoke(contentBody);
            voiceAssistant.Speak(contentBody);
            print(contentBody);
        }
    }

    private string PreparePayload(string base64Image)
    {
        return $"{{\"model\":\"{gptVisionModel}\",\"messages\":[{{\"role\":\"user\",\"content\":[{{\"type\":\"text\",\"text\":\"{command} {baseCommand}\"}},{{\"type\":\"image_url\",\"image_url\":{{\"url\":\"data:image/jpeg;base64,{base64Image}\"}}}}]}}],\"max_tokens\":300}}";
    }

    public void GetVoiceCommand() => appVoiceExperience.Activate();

    private void GetTranscriptAndSendImage(string transcript)
    {
        command = transcript;
        inputTranscriptText.text = transcript;
        SendImageToOpenAI();
    }
}
