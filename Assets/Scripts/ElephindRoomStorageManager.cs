using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class ElephindRoomStorageManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Loaded Room :"+ MRUK.Instance.GetCurrentRoom().GetType().ToString() +"\n");
        Debug.Log("Loaded Room :"+ MRUK.Instance.GetCurrentRoom().GetRoomBounds().ToString() +"\n");

        var room = MRUK.Instance.GetCurrentRoom();

        foreach (var anchor in room.Anchors)
            {
                if (anchor.Label == MRUKAnchor.SceneLabels.STORAGE)
                {
                    Debug.Log(anchor.Label + " was found in room");
                    Debug.Log(anchor.name +" :name");
                }
                
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
