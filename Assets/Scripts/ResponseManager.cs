using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResponseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WebRequester.Instance.RequestCompleted.AddListener(DecodeGenericResponse);
    }

    void DecodeGenericResponse(GenericResponse response)
    {
        Debug.LogWarning("response ");
        switch(response.response_code)
        {
            case ResponseCode.OK:
            Debug.LogWarning("ok ");
                  VoiceSystem.Instance.SpeakText(response.chat_response);
            break;

            case ResponseCode.CAPTURE_RESULT:
            Debug.LogWarning("search ");
                VoiceSystem.Instance.SpeakText(response.chat_response);
                ForwardResponseData(response.storageunits);
            break;

            case ResponseCode.SEARCH_RESULT:
            Debug.LogWarning("cap ");
                VoiceSystem.Instance.SpeakText(response.chat_response);
                ShowContainersWithItem(response.storageunits);
            break;

            default:
                  VoiceSystem.Instance.SpeakText(response.chat_response);
            break;
        }

        string text = response.chat_response;
        VoiceSystem.Instance.SpeakText(text);
    }

    void ForwardResponseData(List<StorageUnit> storageUnits)
    {
        foreach(StorageUnit storageUnit in storageUnits)
        {
            StorageContainerManager.Instance.UpdateContainerData(Int32.Parse(storageUnit.id), storageUnit.name, storageUnit.items);
        }

    }

    void ShowContainersWithItem(List<StorageUnit> storageUnits)
    {
        Debug.LogWarning("Show Containers");
        List<int> ids = new List<int>();

        foreach(StorageUnit storageUnit in storageUnits)
        {
             ids.Add(Int32.Parse(storageUnit.id));
        }

        StorageContainerManager.Instance.HighlighSpecificContainer(ids);


    }

  
}
