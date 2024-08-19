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

        //not working, for whatever reason
       // this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);
    }
}
