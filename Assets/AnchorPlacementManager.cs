using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using UnityEngine.UI;
using TMPro;


public class AnchorPlacementManager : MonoBehaviour
{

   

    private List<OVRSpatialAnchor> anchors  = new List<OVRSpatialAnchor>();
    public GameObject anchorPrefab;
    

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)){
            CreateSpatialAnchor();   
        }

    }

    private void CreateSpatialAnchor(){

        GameObject prefab = Instantiate(anchorPrefab,OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));
        //OVRSpatialAnchor workingAnchor = prefab.GetComponent<OVRSpatialAnchor>();



    }
}
