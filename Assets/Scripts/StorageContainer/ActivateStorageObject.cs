using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateStorageObject : MonoBehaviour
{

    [SerializeField] GameObject _storageObject;
    [SerializeField] GameObject _anchorObject;


    public void ActivateNow()
    {
        _storageObject.SetActive(true);
        _anchorObject.SetActive(false);
    }
}
