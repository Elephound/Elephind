using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResponseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        WebRequester.Instance.RequestCompleted.AddListener(DecodeGenericResponse);
    }

    void DecodeGenericResponse(GenericResponse response)
    {
        switch(response.response_code)
        {
            default:
                  VoiceSystem.Instance.SpeakText(response.chat_response);
            break;

            
            case ResponseCode.OK:
                  VoiceSystem.Instance.SpeakText(response.chat_response);
            break;


            case ResponseCode.SEARCH_RESULT:
                VoiceSystem.Instance.SpeakText(response.chat_response);
                ForwardResponseData(response.storageunits);
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

  
}
