using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class WebRequester : MonoBehaviour
{
    // Konstante für den "HelloWorld" Endpoint
    private const string ENDPOINT_HELLOWORLD = "/api/helloworld";

    [SerializeField] private string baseWebAddress; // https://elephound-backend-git-preview-mario-deutschmanns-projects.vercel.app
                                                    // /api/helloworld
    [SerializeField] private string sessionId;

    // Dictionary to store header key-value pairs
    [SerializeField] private List<Header> headers = new List<Header>();//x-vercel-protection-bypass=UqndmmoWezkSatYYW3KDvkg3inolbmJg

    [System.Serializable]
    public class Header
    {
        public string key;
        public string value;
    }

    private void Start()
    {
        Debug.Log("Start: Fuck (- where is) the hammer?!");
        StartCoroutine(GetRequest(baseWebAddress+ ENDPOINT_HELLOWORLD));
    }

    public void TestCall()
    {
        GetRequest(baseWebAddress);
    }


    public void SendTranscription(string text)
    {

    }


    IEnumerator TestUpload()
    {
        using (UnityWebRequest www = UnityWebRequest.Post(baseWebAddress, "{ \"field1\": 1, \"field2\": 2 }", "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
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
        foreach (Header header in headers)
        {
            request.SetRequestHeader(header.key, header.value);
        }
    }
}
