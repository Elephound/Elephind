using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using TMPro;
using Newtonsoft.Json;
//using UnityEngine.UIElements;

public class WebRequester : MonoBehaviour
{
    // Konstante für den "HelloWorld" Endpoint
    private const string ENDPOINT_HELLOWORLD = "/api/helloworld";
    private const string ENDPOINT_COMPLETION = "/api/completion";
    private const string ENDPOINT_STORAGEUNIT_FROMIMAGE = "/api/storageunit/fromimage"; 


    [SerializeField] private string baseWebAddress; // https://elephound-backend-git-preview-mario-deutschmanns-projects.vercel.app
                                                    // http://localhost:3000
    [SerializeField] private string sessionId;


    // Dictionary to store header key-value pairs
    [SerializeField] private List<Header> headers = new List<Header>();//x-vercel-protection-bypass=UqndmmoWezkSatYYW3KDvkg3inolbmJg

    [System.Serializable]
    public class Header
    {
        public string key;
        public string value;
    }

    // UI Elemente
    [SerializeField] private TMP_Text resultTextPanel;   // TextPanel für das Ergebnis

    static public WebRequester Instance;


    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }


    private void Start()
    {
      //  CallHelloWorld();
    }


   

    public void SendStorageUnit(StorageContainer storageContainer)
    {
        string base64String = Convert.ToBase64String(storageContainer.ScreenshotData);
        StartCoroutine(CaptureStorageUnitFromImage(storageContainer.ContainerID.ToString(), base64String, 
                storageContainer.Description, StorageContainerManager.Instance.room.RoomID.ToString()));
    }

    IEnumerator SendChatMessage(string _message)
    {
        // Create a JSON object with the message
        string jsonMessage = $"{{ \"message\": \"{_message}\" }}";
        Debug.Log(JsonUtility.ToJson(_message));
        Debug.Log("jsonMessage: "+ jsonMessage);

        using (UnityWebRequest www = UnityWebRequest.Post(baseWebAddress + ENDPOINT_COMPLETION, jsonMessage, "application/json"))
        {
            AddHeaders(www); // Header hinzufügen

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                resultTextPanel.text = "Error: " + www.error;
            }
            else
            {
                // Ergebnis im TextPanel anzeigen
                resultTextPanel.text = "Server: " + www.downloadHandler.text;
            }
        }
    }

    IEnumerator CaptureStorageUnitFromImage(string _storageUnitId, string _imageBase64Encoded, string _storageUnitName, string _sessionId)
    {
        // Create a JSON object with the message
        string jsonMessage = $"{{ \"storageUnitId\": \"{_storageUnitId}\", \"sessionId\": \"{_sessionId}\", \"storageUnitName\": \"{_storageUnitName}\", \"captureImage\": \"{_imageBase64Encoded}\" }}";
        Debug.Log("jsonMessage: " + jsonMessage);

        using (UnityWebRequest www = UnityWebRequest.Post(baseWebAddress + ENDPOINT_STORAGEUNIT_FROMIMAGE, jsonMessage, "application/json"))
        {
            AddHeaders(www); // Header hinzufügen

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                resultTextPanel.text = "Error: " + www.error;
            }
            else
            {
                // Ergebnis im TextPanel anzeigen
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Server: " + jsonResponse);

                GenericResponse responseObject = JsonConvert.DeserializeObject<GenericResponse>(jsonResponse);
                resultTextPanel.text = responseObject.chat_response;
                

                foreach (StorageUnit su in responseObject.storageunits)
                {
                    resultTextPanel.text += "\n " + su.name + " (" + su.items.Count + " - ID:"+su.id+", "+su.description+")";
                    foreach(Item item in su.items)
                    {
                        resultTextPanel.text += "\n - " + item.name + " (" + item.quantity + ", "+item.description+", "+item.category+")";
                    }
                }

                Debug.Log(responseObject);

            }
        }
    }

    public void CallHelloWorld()
    {
        Debug.Log("Start: Fuck (- where is) the hammer?!");
        StartCoroutine(GetRequest(baseWebAddress + ENDPOINT_HELLOWORLD));
    }


    public void SendTranscription(string text)
    {

    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            Debug.Log("GetRequest 1: " + uri);

            AddHeaders(webRequest); // Add headers to the request

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            Debug.Log("GetRequest 2: " + uri);

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    private void AddHeaders(UnityWebRequest request)
    {
        request.SetRequestHeader("sessionId", sessionId);
        foreach (Header header in headers)
        {
            request.SetRequestHeader(header.key, header.value);
        }
    }
}
